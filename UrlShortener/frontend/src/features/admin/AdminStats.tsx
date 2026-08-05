import { useQuery } from '@tanstack/react-query';
import { Users, Link as LinkIcon, MousePointerClick } from 'lucide-react';
import api from '../../services/api';

interface AdminStatsDto {
  totalUsers: number;
  totalLinks: number;
  totalClicks: number;
}

const AdminStats = () => {
  const { data, isLoading } = useQuery({
    queryKey: ['adminStats'],
    queryFn: async () => {
      const response = await api.get<AdminStatsDto>('/admin/stats');
      return response.data;
    },
  });

  if (isLoading) {
    return <div className="animate-pulse flex space-x-4 h-24 bg-slate-800/50 rounded-xl w-full"></div>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div className="bg-slate-900/50 border border-slate-800 p-6 rounded-xl flex items-center justify-between">
        <div>
          <p className="text-slate-400 text-sm">Total Users</p>
          <h3 className="text-3xl font-bold text-white mt-1">{data?.totalUsers || 0}</h3>
        </div>
        <div className="p-3 bg-indigo-500/20 rounded-lg text-indigo-400">
          <Users size={24} />
        </div>
      </div>
      
      <div className="bg-slate-900/50 border border-slate-800 p-6 rounded-xl flex items-center justify-between">
        <div>
          <p className="text-slate-400 text-sm">Total Links (Inc. Deleted)</p>
          <h3 className="text-3xl font-bold text-white mt-1">{data?.totalLinks || 0}</h3>
        </div>
        <div className="p-3 bg-emerald-500/20 rounded-lg text-emerald-400">
          <LinkIcon size={24} />
        </div>
      </div>
      
      <div className="bg-slate-900/50 border border-slate-800 p-6 rounded-xl flex items-center justify-between">
        <div>
          <p className="text-slate-400 text-sm">Total Global Clicks</p>
          <h3 className="text-3xl font-bold text-white mt-1">{data?.totalClicks || 0}</h3>
        </div>
        <div className="p-3 bg-cyan-500/20 rounded-lg text-cyan-400">
          <MousePointerClick size={24} />
        </div>
      </div>
    </div>
  );
};

export default AdminStats;
