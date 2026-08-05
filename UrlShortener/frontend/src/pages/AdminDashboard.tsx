import { useAuth } from '../features/auth/AuthContext';
import { Navigate } from 'react-router-dom';
import AdminStats from '../features/admin/AdminStats';
import AdminUsersTable from '../features/admin/AdminUsersTable';
import AdminLinksTable from '../features/admin/AdminLinksTable';

const AdminDashboard = () => {
  const { isAuthenticated, isAdmin, logout } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!isAdmin) {
    return <Navigate to="/dashboard" replace />;
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white p-4 md:p-8 relative overflow-hidden">
      {/* Background gradients */}
      <div className="absolute top-0 left-0 w-[500px] h-[500px] bg-indigo-600/10 blur-[120px] rounded-full pointer-events-none" />
      <div className="absolute bottom-0 right-0 w-[500px] h-[500px] bg-rose-600/10 blur-[120px] rounded-full pointer-events-none" />

      <div className="max-w-7xl mx-auto relative z-10">
        <div className="flex justify-between items-center mb-10">
          <h1 className="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-rose-400 to-indigo-400">
            Admin Dashboard
          </h1>
          <div className="flex gap-4">
            <a 
              href="/dashboard"
              className="px-4 py-2 bg-indigo-600/20 text-indigo-400 hover:bg-indigo-600/30 rounded-lg transition-colors border border-indigo-500/20 backdrop-blur-sm"
            >
              User Dashboard
            </a>
            <button 
              onClick={logout}
              className="px-4 py-2 bg-slate-800/80 hover:bg-slate-700/80 rounded-lg transition-colors border border-slate-700/50 backdrop-blur-sm"
            >
              Logout
            </button>
          </div>
        </div>
        
        <div className="space-y-8">
          <section>
            <h2 className="text-xl font-semibold mb-4 text-slate-300">Overview</h2>
            <AdminStats />
          </section>

          <section>
            <h2 className="text-xl font-semibold mb-4 text-slate-300">User Management</h2>
            <AdminUsersTable />
          </section>

          <section>
            <h2 className="text-xl font-semibold mb-4 text-slate-300">Global Links Management</h2>
            <AdminLinksTable />
          </section>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
