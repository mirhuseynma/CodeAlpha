import React from 'react';
import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';
import { LayoutDashboard, Users, Calendar, LogOut } from 'lucide-react';

const AdminLayout: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    navigate('/login');
  };

  const isActive = (path: string) => {
    return location.pathname === path ? 'active' : '';
  };

  return (
    <div style={{ display: 'flex', minHeight: '100vh', backgroundColor: 'var(--bg-main)' }}>
      {/* Sidebar */}
      <aside style={{ 
        width: '250px', 
        backgroundColor: 'var(--bg-secondary)', 
        borderRight: '1px solid var(--border)',
        padding: '2rem 1rem',
        display: 'flex',
        flexDirection: 'column'
      }}>
        <div style={{ marginBottom: '3rem', paddingLeft: '1rem' }}>
          <h2 style={{ color: 'var(--accent)', fontSize: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <LayoutDashboard size={24} />
            Admin Panel
          </h2>
        </div>

        <nav style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
          <Link to="/admin" className={`nav-link ${isActive('/admin')}`} style={{ padding: '0.75rem 1rem', borderRadius: '0.5rem', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
            <LayoutDashboard size={20} />
            Dashboard
          </Link>
          <Link to="/admin/events" className={`nav-link ${isActive('/admin/events')}`} style={{ padding: '0.75rem 1rem', borderRadius: '0.5rem', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
            <Calendar size={20} />
            Events Management
          </Link>
          <Link to="/admin/users" className={`nav-link ${isActive('/admin/users')}`} style={{ padding: '0.75rem 1rem', borderRadius: '0.5rem', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
            <Users size={20} />
            Users Management
          </Link>
        </nav>

        <div>
          <Link to="/" className="nav-link" style={{ padding: '0.75rem 1rem', display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '1rem' }}>
            Go to Main Site
          </Link>
          <button onClick={handleLogout} className="btn btn-danger" style={{ width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem' }}>
            <LogOut size={16} /> Logout
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main style={{ flex: 1, padding: '2rem', overflowY: 'auto' }}>
        <Outlet />
      </main>
    </div>
  );
};

export default AdminLayout;
