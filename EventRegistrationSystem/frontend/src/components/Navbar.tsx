import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Calendar, LogOut } from 'lucide-react';

const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('role');

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <Link to="/" className="nav-brand">
        <Calendar size={24} color="var(--accent)" />
        EventSync
      </Link>
      
      <div className="nav-links">
        {token ? (
          <>
            <Link to="/events" className="nav-link">Events</Link>
            <Link to="/my-registrations" className="nav-link">My Registrations</Link>
            
            {(() => {
              try {
                if (token) {
                  const payload = JSON.parse(atob(token.split('.')[1]));
                  const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
                  if (role === 'Admin' || role === 'Organizer' || (Array.isArray(role) && (role.includes('Admin') || role.includes('Organizer')))) {
                    return <Link to="/admin" className="nav-link" style={{ color: 'var(--accent)' }}>Admin Panel</Link>;
                  }
                }
              } catch (e) {}
              return null;
            })()}

            <button onClick={handleLogout} className="btn btn-danger" style={{ padding: '0.5rem 1rem' }}>
              <LogOut size={16} /> Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login" className="nav-link">Login</Link>
            <Link to="/register" className="btn">Sign Up</Link>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
