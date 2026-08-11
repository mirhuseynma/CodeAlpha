import React from 'react';
import { Calendar, Users } from 'lucide-react';
import { Link } from 'react-router-dom';

const AdminDashboard: React.FC = () => {
  return (
    <div>
      <h2 style={{ marginBottom: '2rem' }}>Dashboard Overview</h2>
      
      <div className="grid">
        <div className="card" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: '1rem' }}>
          <div style={{ padding: '1rem', backgroundColor: 'rgba(59, 130, 246, 0.1)', borderRadius: '50%', color: 'var(--accent)' }}>
            <Calendar size={48} />
          </div>
          <h3>Events Management</h3>
          <p style={{ color: 'var(--text-secondary)' }}>Create, update, and manage all events across the platform.</p>
          <Link to="/admin/events" className="btn" style={{ marginTop: 'auto' }}>Go to Events</Link>
        </div>

        <div className="card" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: '1rem' }}>
          <div style={{ padding: '1rem', backgroundColor: 'rgba(16, 185, 129, 0.1)', borderRadius: '50%', color: '#10b981' }}>
            <Users size={48} />
          </div>
          <h3>Users Management</h3>
          <p style={{ color: 'var(--text-secondary)' }}>Manage user roles, grant permissions, or remove accounts.</p>
          <Link to="/admin/users" className="btn" style={{ marginTop: 'auto', backgroundColor: '#10b981' }}>Go to Users</Link>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
