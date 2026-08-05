import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { getAccessToken, clearTokens } from '../../services/api';

interface AuthContextType {
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: () => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isAdmin, setIsAdmin] = useState<boolean>(false);
  const [loading, setLoading] = useState(true);

  const checkAdminStatus = (token: string) => {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // In ASP.NET Core Identity, roles are usually under 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' or 'role'
      const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role;
      if (Array.isArray(roleClaim)) {
        setIsAdmin(roleClaim.includes('Admin'));
      } else {
        setIsAdmin(roleClaim === 'Admin');
      }
    } catch (e) {
      setIsAdmin(false);
    }
  };

  useEffect(() => {
    // Check initial auth state
    const token = getAccessToken();
    if (token) {
      setIsAuthenticated(true);
      checkAdminStatus(token);
    }
    setLoading(false);

    // Listen for unauthorized events from the API interceptor
    const handleUnauthorized = () => {
      setIsAuthenticated(false);
      setIsAdmin(false);
      clearTokens();
    };

    window.addEventListener('unauthorized', handleUnauthorized);
    return () => window.removeEventListener('unauthorized', handleUnauthorized);
  }, []);

  const login = () => {
    setIsAuthenticated(true);
    const token = getAccessToken();
    if (token) {
      checkAdminStatus(token);
    }
  };

  const logout = () => {
    clearTokens();
    setIsAuthenticated(false);
    setIsAdmin(false);
  };

  if (loading) {
    return null; // Or a beautiful loading spinner
  }

  return (
    <AuthContext.Provider value={{ isAuthenticated, isAdmin, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
