import React, { useEffect, useState } from 'react';
import api from '../../api';
import { Trash2 } from 'lucide-react';

interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
  createdAt: string;
}

const AdminUsers: React.FC = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/users');
      setUsers(response.data);
    } catch (err: any) {
      console.error(err);
      if (err.response?.status === 403) {
        alert("You do not have permission to view users.");
      }
    } finally {
      setLoading(false);
    }
  };

  const handleRoleChange = async (userId: string, newRole: string) => {
    if (!window.confirm(`Are you sure you want to change this user's role to ${newRole}?`)) return;

    try {
      await api.put(`/users/${userId}/role`, { role: newRole });
      alert('Role updated successfully.');
      fetchUsers();
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to update role');
    }
  };

  const handleDelete = async (userId: string) => {
    if (!window.confirm('Are you sure you want to completely delete this user? This action cannot be undone.')) return;

    try {
      await api.delete(`/users/${userId}`);
      alert('User deleted successfully.');
      fetchUsers();
    } catch (err: any) {
      alert(err.response?.data?.detail || 'Failed to delete user');
    }
  };

  if (loading) return <div>Loading users...</div>;

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <h2>Users Management</h2>
      </div>

      <div className="card" style={{ padding: '0', overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid var(--border)', backgroundColor: 'rgba(0,0,0,0.2)' }}>
              <th style={{ padding: '1rem' }}>Name</th>
              <th style={{ padding: '1rem' }}>Email</th>
              <th style={{ padding: '1rem' }}>Current Role</th>
              <th style={{ padding: '1rem' }}>Change Role</th>
              <th style={{ padding: '1rem', textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map(u => (
              <tr key={u.id} style={{ borderBottom: '1px solid var(--border)' }}>
                <td style={{ padding: '1rem' }}>{u.firstName} {u.lastName}</td>
                <td style={{ padding: '1rem' }}>{u.email}</td>
                <td style={{ padding: '1rem' }}>
                  <span style={{ 
                    padding: '0.25rem 0.75rem', 
                    borderRadius: '9999px', 
                    fontSize: '0.875rem',
                    backgroundColor: u.roles.includes('Admin') ? 'rgba(239, 68, 68, 0.2)' : u.roles.includes('Organizer') ? 'rgba(16, 185, 129, 0.2)' : 'rgba(59, 130, 246, 0.2)',
                    color: u.roles.includes('Admin') ? '#ef4444' : u.roles.includes('Organizer') ? '#10b981' : '#3b82f6'
                  }}>
                    {u.roles[0] || 'User'}
                  </span>
                </td>
                <td style={{ padding: '1rem' }}>
                  <select 
                    className="input"
                    style={{ padding: '0.5rem', width: 'auto', display: 'inline-block' }}
                    value={u.roles[0] || 'User'}
                    onChange={(e) => handleRoleChange(u.id, e.target.value)}
                    disabled={u.roles.includes('Admin')}
                  >
                    <option value="User">User</option>
                    <option value="Organizer">Organizer</option>
                    <option value="Admin">Admin</option>
                  </select>
                </td>
                <td style={{ padding: '1rem', textAlign: 'right' }}>
                  <button 
                    onClick={() => handleDelete(u.id)}
                    style={{ 
                      background: 'none', 
                      border: 'none', 
                      color: u.roles.includes('Admin') ? '#9ca3af' : '#ef4444', 
                      cursor: u.roles.includes('Admin') ? 'not-allowed' : 'pointer', 
                      padding: '0.5rem' 
                    }}
                    title={u.roles.includes('Admin') ? "Cannot delete admin" : "Delete User"}
                    disabled={u.roles.includes('Admin')}
                  >
                    <Trash2 size={20} />
                  </button>
                </td>
              </tr>
            ))}
            {users.length === 0 && (
              <tr>
                <td colSpan={5} style={{ padding: '2rem', textAlign: 'center' }}>No users found.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AdminUsers;
