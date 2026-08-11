import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../api';

const Login: React.FC = () => {
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
      const response = await api.post('/auth/login', { email, password });
      localStorage.setItem('token', response.data.accessToken);
      // localStorage.setItem('role', response.data.role); // Optional: decode from JWT if needed
      navigate('/events');
    } catch (err: any) {
      setError(err.response?.data?.detail || err.response?.data?.title || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container card">
      <h2 className="text-center" style={{ marginBottom: '1.5rem' }}>Welcome Back</h2>
      {error && <div className="error-msg text-center" style={{ marginBottom: '1rem' }}>{error}</div>}
      
      <form onSubmit={handleSubmit}>
        <div className="input-group">
          <label>Email</label>
          <input 
            type="email" 
            className="input" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            required 
          />
        </div>
        <div className="input-group">
          <label>Password</label>
          <input 
            type="password" 
            className="input" 
            value={password} 
            onChange={(e) => setPassword(e.target.value)} 
            required 
          />
        </div>
        <button type="submit" className="btn" style={{ width: '100%', marginTop: '1rem' }} disabled={loading}>
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
      <p className="text-center" style={{ marginTop: '1.5rem' }}>
        Don't have an account? <Link to="/register" style={{ color: 'var(--accent)' }}>Sign up</Link>
      </p>
    </div>
  );
};

export default Login;
