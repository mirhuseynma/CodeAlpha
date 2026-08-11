import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../../api';
import { ArrowLeft } from 'lucide-react';

interface RegistrationDto {
  id: string;
  eventId: string;
  userId: string;
  userFullName: string;
  userEmail: string;
  registeredAt: string;
}

const EventRegistrations: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [registrations, setRegistrations] = useState<RegistrationDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchRegistrations();
  }, [id]);

  const fetchRegistrations = async () => {
    try {
      const response = await api.get(`/events/${id}/registrations`);
      setRegistrations(response.data);
    } catch (err: any) {
      console.error(err);
      if (err.response?.status === 403) {
        alert("You do not have permission to view registrations for this event.");
      }
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Loading registrations...</div>;

  return (
    <div>
      <div style={{ marginBottom: '2rem' }}>
        <Link to="/admin/events" style={{ display: 'inline-flex', alignItems: 'center', gap: '0.5rem', color: 'var(--text-secondary)', textDecoration: 'none', marginBottom: '1rem' }}>
          <ArrowLeft size={16} /> Back to Events
        </Link>
        <h2>Event Registrations</h2>
        <p style={{ color: 'var(--text-secondary)' }}>Total Registrations: {registrations.length}</p>
      </div>

      <div className="card" style={{ padding: '0', overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid var(--border)', backgroundColor: 'rgba(0,0,0,0.2)' }}>
              <th style={{ padding: '1rem' }}>Name</th>
              <th style={{ padding: '1rem' }}>Email</th>
              <th style={{ padding: '1rem' }}>Registration Date</th>
            </tr>
          </thead>
          <tbody>
            {registrations.map(r => (
              <tr key={r.id} style={{ borderBottom: '1px solid var(--border)' }}>
                <td style={{ padding: '1rem' }}>{r.userFullName}</td>
                <td style={{ padding: '1rem' }}>{r.userEmail}</td>
                <td style={{ padding: '1rem' }}>{new Date(r.registeredAt).toLocaleString()}</td>
              </tr>
            ))}
            {registrations.length === 0 && (
              <tr>
                <td colSpan={3} style={{ padding: '2rem', textAlign: 'center' }}>No one has registered for this event yet.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default EventRegistrations;
