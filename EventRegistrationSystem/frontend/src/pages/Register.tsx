import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../api';

const Register: React.FC = () => {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await api.post('/auth/register', { firstName, lastName, email, password });
      alert('Registration successful. Please login.');
      navigate('/login');
    } catch (err: any) {
      if (err.response?.data?.errors) {
        const firstError = Object.values(err.response.data.errors)[0] as string[];
        setError(firstError[0]);
      } else {
        setError(err.response?.data?.detail || err.response?.data?.title || 'Registration failed');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container card">
      <h2 className="text-center" style={{ marginBottom: '1.5rem' }}>Create Account</h2>
      {error && <div className="error-msg text-center" style={{ marginBottom: '1rem' }}>{error}</div>}
      
      <form onSubmit={handleSubmit}>
        <div style={{ display: 'flex', gap: '1rem' }}>
          <div className="input-group" style={{ flex: 1 }}>
            <label>First Name</label>
            <input type="text" className="input" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
          </div>
          <div className="input-group" style={{ flex: 1 }}>
            <label>Last Name</label>
            <input type="text" className="input" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
          </div>
        </div>
        <div className="input-group">
          <label>Email</label>
          <input type="email" className="input" value={email} onChange={(e) => setEmail(e.target.value)} required />
        </div>
        <div className="input-group">
          <label>Password</label>
          <input type="password" className="input" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </div>
        <button type="submit" className="btn" style={{ width: '100%', marginTop: '1rem' }} disabled={loading}>
          {loading ? 'Creating account...' : 'Sign Up'}
        </button>
      </form>
      <p className="text-center" style={{ marginTop: '1.5rem' }}>
        Already have an account? <Link to="/login" style={{ color: 'var(--accent)' }}>Login</Link>
      </p>
    </div>
  );
};

export default Register;
