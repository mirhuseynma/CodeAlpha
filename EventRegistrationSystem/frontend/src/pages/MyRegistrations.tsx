import React, { useEffect, useState } from 'react';
import { Calendar, Trash2 } from 'lucide-react';
import api from '../api';

interface RegistrationDto {
  id: string;
  eventId: string;
  eventTitle: string;
  eventStartDate: string;
  registeredAt: string;
  status: string;
}

const MyRegistrations: React.FC = () => {
  const [registrations, setRegistrations] = useState<RegistrationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState<string | null>(null);

  useEffect(() => {
    fetchRegistrations();
  }, []);

  const fetchRegistrations = async () => {
    try {
      const response = await api.get('/registrations/me');
      setRegistrations(response.data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (id: string) => {
    if (!window.confirm('Are you sure you want to cancel this registration?')) return;
    
    setCancelling(id);
    try {
      const res = await api.delete(`/registrations/${id}`);
      alert(res.data.message || 'Successfully cancelled!');
      fetchRegistrations();
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to cancel');
    } finally {
      setCancelling(null);
    }
  };

  if (loading) return <div className="text-center" style={{ marginTop: '4rem' }}>Loading registrations...</div>;

  return (
    <div>
      <h2 style={{ marginBottom: '2rem' }}>My Registrations</h2>

      {registrations.length === 0 ? (
        <div className="card text-center" style={{ padding: '3rem' }}>
          <p>You haven't registered for any events yet.</p>
        </div>
      ) : (
        <div className="grid">
          {registrations.map((r) => (
            <div key={r.id} className="card" style={{ display: 'flex', flexDirection: 'column' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                <h3 style={{ color: 'var(--text-main)', marginBottom: '1rem' }}>{r.eventTitle}</h3>
                <span className="badge">{r.status}</span>
              </div>
              
              <div style={{ marginBottom: '1.5rem', flex: 1 }}>
                <div className="event-meta">
                  <Calendar size={16} />
                  Event Date: {new Date(r.eventStartDate).toLocaleDateString()}
                </div>
                <div className="event-meta" style={{ fontSize: '0.75rem', marginTop: '1rem' }}>
                  Registered on: {new Date(r.registeredAt).toLocaleDateString()}
                </div>
              </div>

              <button 
                className="btn btn-danger" 
                style={{ width: '100%' }}
                onClick={() => handleCancel(r.id)}
                disabled={cancelling === r.id || r.status === 'Cancelled'}
              >
                {cancelling === r.id ? 'Cancelling...' : (
                  <><Trash2 size={16} /> Cancel Registration</>
                )}
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default MyRegistrations;
