import React, { useEffect, useState } from 'react';
import api from '../../api';
import { Calendar, Trash2, Edit, Users } from 'lucide-react';
import { Link } from 'react-router-dom';

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

const AdminEvents: React.FC = () => {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    location: '',
    startDate: '',
    endDate: '',
    capacity: 0
  });

  useEffect(() => {
    fetchEvents();
  }, []);

  const fetchEvents = async () => {
    try {
      const response = await api.get('/events');
      setEvents(response.data);
    } catch (err: any) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const formatForInput = (dateString: string) => {
    const d = new Date(dateString);
    const tzoffset = d.getTimezoneOffset() * 60000;
    return new Date(d.getTime() - tzoffset).toISOString().slice(0, 16);
  };

  const handleOpenForm = (e?: EventDto) => {
    if (e) {
      setEditingId(e.id);
      setFormData({
        title: e.title,
        description: e.description,
        location: e.location,
        startDate: formatForInput(e.startDate),
        endDate: formatForInput(e.endDate),
        capacity: e.capacity
      });
    } else {
      setEditingId(null);
      setFormData({
        title: '',
        description: '',
        location: '',
        startDate: '',
        endDate: '',
        capacity: 0
      });
    }
    setIsFormOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const payload = {
        ...formData,
        startDate: new Date(formData.startDate).toISOString(),
        endDate: new Date(formData.endDate).toISOString()
      };

      if (editingId) {
        await api.put(`/events/${editingId}`, { id: editingId, ...payload });
        alert('Event updated successfully');
      } else {
        await api.post('/events', payload);
        alert('Event created successfully');
      }
      setIsFormOpen(false);
      fetchEvents();
    } catch (err: any) {
      alert(err.response?.data?.detail || err.response?.data?.title || 'Failed to save event');
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this event?')) return;
    try {
      await api.delete(`/events/${id}`);
      fetchEvents();
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to delete event');
    }
  };

  if (loading) return <div>Loading events...</div>;

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <h2>Events Management</h2>
        <button className="btn" onClick={() => handleOpenForm()}>+ Create Event</button>
      </div>

      {isFormOpen && (
        <div className="card" style={{ marginBottom: '2rem', border: '1px solid var(--accent)' }}>
          <h3>{editingId ? 'Edit Event' : 'Create New Event'}</h3>
          <form onSubmit={handleSubmit} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginTop: '1rem' }}>
            <div className="input-group">
              <label>Title</label>
              <input type="text" className="input" value={formData.title} onChange={e => setFormData({...formData, title: e.target.value})} required />
            </div>
            <div className="input-group">
              <label>Location</label>
              <input type="text" className="input" value={formData.location} onChange={e => setFormData({...formData, location: e.target.value})} required />
            </div>
            <div className="input-group" style={{ gridColumn: '1 / -1' }}>
              <label>Description</label>
              <textarea className="input" rows={3} value={formData.description} onChange={e => setFormData({...formData, description: e.target.value})} required />
            </div>
            <div className="input-group">
              <label>Start Date</label>
              <input type="datetime-local" className="input" value={formData.startDate} onChange={e => setFormData({...formData, startDate: e.target.value})} required />
            </div>
            <div className="input-group">
              <label>End Date</label>
              <input type="datetime-local" className="input" value={formData.endDate} onChange={e => setFormData({...formData, endDate: e.target.value})} required />
            </div>
            <div className="input-group">
              <label>Capacity</label>
              <input type="number" className="input" value={formData.capacity} onChange={e => setFormData({...formData, capacity: parseInt(e.target.value)})} required min="1" />
            </div>
            <div style={{ gridColumn: '1 / -1', display: 'flex', gap: '1rem', marginTop: '1rem' }}>
              <button type="submit" className="btn">Save Event</button>
              <button type="button" className="btn" style={{ background: 'var(--bg-secondary)', color: 'var(--text-main)' }} onClick={() => setIsFormOpen(false)}>Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="grid">
        {events.map((e) => (
          <div key={e.id} className="card" style={{ display: 'flex', flexDirection: 'column' }}>
            <h3 style={{ color: 'var(--text-main)', marginBottom: '0.5rem' }}>{e.title}</h3>
            <p style={{ fontSize: '0.875rem', marginBottom: '1rem', flex: 1 }}>{e.description}</p>
            
            <div style={{ marginBottom: '1.5rem', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
              <div><Calendar size={14} style={{ marginRight: '0.5rem', display: 'inline' }}/>{new Date(e.startDate).toLocaleString()}</div>
              <div>Capacity: {e.capacity}</div>
              <div>Organizer: {e.organizerName || 'Unknown'}</div>
            </div>

            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <Link to={`/admin/events/${e.id}/registrations`} className="btn" style={{ flex: 1, textAlign: 'center', backgroundColor: '#3b82f6', padding: '0.5rem' }}>
                <Users size={16} style={{ display: 'inline', marginRight: '0.25rem' }}/> Registrations
              </Link>
              <button className="btn" style={{ padding: '0.5rem' }} onClick={() => handleOpenForm(e)}>
                <Edit size={16} />
              </button>
              <button className="btn btn-danger" style={{ padding: '0.5rem' }} onClick={() => handleDelete(e.id)}>
                <Trash2 size={16} />
              </button>
            </div>
          </div>
        ))}
        {events.length === 0 && <p>No events found.</p>}
      </div>
    </div>
  );
};

export default AdminEvents;
