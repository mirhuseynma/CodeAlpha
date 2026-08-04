import { useQuery } from '@tanstack/react-query';
import api from '../../services/api';

interface UserStats {
  totalClicks: number;
  activeLinks: number;
}

const QuickStats = () => {
  const { data: stats, isLoading, isError } = useQuery({
    queryKey: ['links', 'stats'],
    queryFn: async () => {
      const response = await api.get<UserStats>('/links/stats');
      return response.data;
    }
  });

  return (
    <div className="space-y-4">
      <div className="bg-slate-900/50 p-4 rounded-xl border border-slate-800/50 relative overflow-hidden group">
        <div className="absolute top-0 right-0 w-24 h-24 bg-cyan-500/10 rounded-full blur-2xl -mr-10 -mt-10 group-hover:bg-cyan-500/20 transition-colors"></div>
        <p className="text-slate-400 text-sm relative z-10">Total Clicks</p>
        <p className="text-4xl font-bold text-white mt-1 relative z-10">
          {isLoading ? <span className="text-slate-600 animate-pulse">--</span> : isError ? '0' : stats?.totalClicks || 0}
        </p>
      </div>
      
      <div className="bg-slate-900/50 p-4 rounded-xl border border-slate-800/50 relative overflow-hidden group">
        <div className="absolute top-0 right-0 w-24 h-24 bg-indigo-500/10 rounded-full blur-2xl -mr-10 -mt-10 group-hover:bg-indigo-500/20 transition-colors"></div>
        <p className="text-slate-400 text-sm relative z-10">Active Links</p>
        <p className="text-4xl font-bold text-white mt-1 relative z-10">
          {isLoading ? <span className="text-slate-600 animate-pulse">--</span> : isError ? '0' : stats?.activeLinks || 0}
        </p>
      </div>
    </div>
  );
};

export default QuickStats;