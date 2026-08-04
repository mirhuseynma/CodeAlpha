import { useAuth } from '../features/auth/AuthContext';
import { Navigate } from 'react-router-dom';
import CreateLinkForm from '../features/links/CreateLinkForm';
import RecentLinksList from '../features/links/RecentLinksList';
import QuickStats from '../features/links/QuickStats';

const Dashboard = () => {
  const { isAuthenticated, logout } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white p-4 md:p-8 relative overflow-hidden">
      {/* Background gradients */}
      <div className="absolute top-0 left-0 w-[500px] h-[500px] bg-indigo-600/10 blur-[120px] rounded-full pointer-events-none" />
      <div className="absolute bottom-0 right-0 w-[500px] h-[500px] bg-cyan-600/10 blur-[120px] rounded-full pointer-events-none" />

      <div className="max-w-6xl mx-auto relative z-10">
        <div className="flex justify-between items-center mb-10">
          <h1 className="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-indigo-400 to-cyan-400">
            LinkForge Dashboard
          </h1>
          <button 
            onClick={logout}
            className="px-4 py-2 bg-slate-800/80 hover:bg-slate-700/80 rounded-lg transition-colors border border-slate-700/50 backdrop-blur-sm"
          >
            Logout
          </button>
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-8">
            <div className="glass-panel p-6 sm:p-8 rounded-2xl">
              <h2 className="text-xl font-semibold mb-6">Create New Short Link</h2>
              <CreateLinkForm />
            </div>
            
            <div className="glass-panel p-6 sm:p-8 rounded-2xl">
              <h2 className="text-xl font-semibold mb-6">Your Recent Links</h2>
              <RecentLinksList />
            </div>
          </div>
          
          <div className="space-y-8">
            <div className="glass-panel p-6 sm:p-8 rounded-2xl">
              <h2 className="text-xl font-semibold mb-6">Quick Stats</h2>
              <QuickStats />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
