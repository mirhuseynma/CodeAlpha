import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import api from '../api';
import { Calendar, MapPin, Users, ArrowLeft } from 'lucide-react';

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

const EventDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [event, setEvent] = useState<EventDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [registering, setRegistering] = useState(false);
  const [myRegistrationId, setMyRegistrationId] = useState<string | null>(null);

  useEffect(() => {
    fetchEvent();
  }, [id]);

  const fetchEvent = async () => {
    try {
      const response = await api.get(`/events/${id}`);
      setEvent(response.data);
      
      try {
        const regResponse = await api.get('/registrations/me');
        const activeRegs = regResponse.data.filter((r: any) => r.status === 'Registered');
        const myReg = activeRegs.find((r: any) => r.eventId === id);
        setMyRegistrationId(myReg ? myReg.id : null);
      } catch (e) {}

    } catch (err: any) {
      console.error(err);
      if (err.response?.status === 404) {
        alert('Event not found.');
        navigate('/events');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async () => {
    if (!event) return;
    setRegistering(true);
    try {
      await api.post(`/events/${event.id}/registrations`);
      alert('Successfully registered for the event!');
      fetchEvent(); // Refresh capacity
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to register');
    } finally {
      setRegistering(false);
    }
  };

  const handleCancelRegistration = async () => {
    if (!myRegistrationId) return;
    if (!window.confirm('Are you sure you want to cancel your registration?')) return;
    
    setRegistering(true);
    try {
      await api.delete(`/registrations/${myRegistrationId}`);
      alert('Registration cancelled successfully.');
      fetchEvent(); // Refresh capacity and registration status
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to cancel registration');
    } finally {
      setRegistering(false);
    }
  };

  if (loading) return <div className="text-center" style={{ marginTop: '4rem' }}>Loading event details...</div>;
  if (!event) return null;

  return (
    <div style={{ maxWidth: '800px', margin: '0 auto' }}>
      <Link to="/events" style={{ display: 'inline-flex', alignItems: 'center', gap: '0.5rem', color: 'var(--text-secondary)', textDecoration: 'none', marginBottom: '2rem' }}>
        <ArrowLeft size={16} /> Back to Events
      </Link>

      <div className="card">
        <h1 style={{ color: 'var(--text-main)', marginBottom: '1rem', fontSize: '2rem' }}>{event.title}</h1>
        
        <div style={{ display: 'flex', gap: '2rem', marginBottom: '2rem', borderBottom: '1px solid var(--border)', paddingBottom: '2rem' }}>
          <div className="event-meta" style={{ fontSize: '1.1rem' }}>
            <Calendar size={20} />
            {new Date(event.startDate).toLocaleString()}
          </div>
          <div className="event-meta" style={{ fontSize: '1.1rem' }}>
            <MapPin size={20} />
            {event.location}
          </div>
          <div className="event-meta" style={{ fontSize: '1.1rem' }}>
            <Users size={20} />
            Capacity: {event.capacity > 0 ? event.capacity : 'Full'}
          </div>
        </div>

        <div style={{ marginBottom: '3rem' }}>
          <h3 style={{ marginBottom: '1rem' }}>About this Event</h3>
          <p style={{ color: 'var(--text-secondary)', lineHeight: '1.8', whiteSpace: 'pre-wrap' }}>
            {event.description}
          </p>
        </div>

        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', backgroundColor: 'var(--bg-secondary)', padding: '1.5rem', borderRadius: '0.5rem' }}>
          <div>
            <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem' }}>Organized by</p>
            <p style={{ fontWeight: 'bold' }}>{event.organizerName}</p>
          </div>
          {myRegistrationId ? (
            <button 
              className="btn btn-danger" 
              onClick={handleCancelRegistration}
              disabled={registering}
              style={{ padding: '0.75rem 2rem', fontSize: '1.1rem' }}
            >
              {registering ? 'Wait...' : 'Cancel Registration'}
            </button>
          ) : (
            <button 
              className="btn" 
              onClick={handleRegister}
              disabled={event.capacity <= 0 || registering}
              style={{ padding: '0.75rem 2rem', fontSize: '1.1rem' }}
            >
              {registering ? 'Wait...' : (event.capacity > 0 ? 'Register Now' : 'Sold Out')}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default EventDetails;
