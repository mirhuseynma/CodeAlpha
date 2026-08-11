import React, { useEffect, useState } from 'react';
import { Calendar, MapPin, Users } from 'lucide-react';
import { Link } from 'react-router-dom';
import api from '../api';

interface EventDto {
  id: string;
  title: string;
  description: string;
  location: string;
  startDate: string;
  endDate: string;
  capacity: number;
  organizerName: string;
}

const Events: React.FC = () => {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [registering, setRegistering] = useState<string | null>(null);
  const [myRegistrations, setMyRegistrations] = useState<Record<string, string>>({});

  useEffect(() => {
    fetchEvents();
  }, []);

  const fetchEvents = async () => {
    try {
      const response = await api.get('/events');
      setEvents(response.data);
      // Fetch user's registrations to know which ones they are registered for
      try {
        const regResponse = await api.get('/registrations/me');
        const activeRegs = regResponse.data.filter((r: any) => r.status === 'Registered');
        
        const regMap: Record<string, string> = {};
        activeRegs.forEach((r: any) => {
          regMap[r.eventId] = r.id; // Map eventId -> registrationId
        });
        setMyRegistrations(regMap);
      } catch (e) {
        // user might not be logged in or doesn't have permission, ignore
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (eventId: string) => {
    setRegistering(eventId);
    try {
      await api.post(`/events/${eventId}/registrations`);
      alert('Successfully registered for the event!');
      fetchEvents(); // Refresh capacity and registrations
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to register');
    } finally {
      setRegistering(null);
    }
  };

  const handleCancelRegistration = async (eventId: string) => {
    const regId = myRegistrations[eventId];
    if (!regId) return;
    
    if (!window.confirm('Are you sure you want to cancel your registration?')) return;
    
    setRegistering(eventId);
    try {
      await api.delete(`/registrations/${regId}`);
      alert('Registration cancelled successfully.');
      fetchEvents(); // Refresh capacity and registrations
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to cancel registration');
    } finally {
      setRegistering(null);
    }
  };

  if (loading) return <div className="text-center" style={{ marginTop: '4rem' }}>Loading events...</div>;

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <h2>Upcoming Events</h2>
      </div>

      <div className="grid">
        {events.map((e) => {
          const isRegistered = !!myRegistrations[e.id];
          
          return (
          <div key={e.id} className="card" style={{ display: 'flex', flexDirection: 'column' }}>
            <h3 style={{ color: 'var(--text-main)', marginBottom: '0.5rem' }}>{e.title}</h3>
            <p style={{ fontSize: '0.875rem', marginBottom: '1rem', flex: 1 }}>{e.description}</p>
            
            <div style={{ marginBottom: '1.5rem' }}>
              <div className="event-meta">
                <Calendar size={16} />
                {new Date(e.startDate).toLocaleDateString()}
              </div>
              <div className="event-meta">
                <MapPin size={16} />
                {e.location}
              </div>
              <div className="event-meta">
                <Users size={16} />
                Capacity: {e.capacity > 0 ? e.capacity : 'Full'}
              </div>
            </div>

            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <Link to={`/events/${e.id}`} className="btn" style={{ flex: 1, textAlign: 'center', backgroundColor: 'var(--bg-secondary)', color: 'var(--text-main)', border: '1px solid var(--border)' }}>
                View Details
              </Link>
              
              {isRegistered ? (
                <button 
                  className="btn btn-danger" 
                  style={{ flex: 1 }}
                  onClick={() => handleCancelRegistration(e.id)}
                  disabled={registering === e.id}
                >
                  {registering === e.id ? 'Wait...' : 'Cancel'}
                </button>
              ) : (
                <button 
                  className="btn" 
                  style={{ flex: 1 }}
                  onClick={() => handleRegister(e.id)}
                  disabled={e.capacity <= 0 || registering === e.id}
                >
                  {registering === e.id ? 'Wait...' : (e.capacity > 0 ? 'Register' : 'Sold Out')}
                </button>
              )}
            </div>
          </div>
        )})}
      </div>
    </div>
  );
};

export default Events;
