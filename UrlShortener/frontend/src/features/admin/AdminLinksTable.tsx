import { useState } from 'react';
import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { ChevronLeft, ChevronRight, Trash2, Power, ExternalLink } from 'lucide-react';
import api, { API_BASE_URL } from '../../services/api';

interface AdminLinkDto {
  id: string;
  originalUrl: string;
  shortCode: string;
  customAlias?: string;
  visitsCount: number;
  isActive: boolean;
  isDeleted: boolean;
  createdAt: string;
  userEmail?: string;
}

interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

const AdminLinksTable = () => {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const queryClient = useQueryClient();

  const { data, isLoading, isFetching } = useQuery({
    queryKey: ['adminLinks', page],
    queryFn: async () => {
      const response = await api.get<PagedResult<AdminLinkDto>>(`/admin/links?pageNumber=${page}&pageSize=${pageSize}`);
      return response.data;
    },
    placeholderData: keepPreviousData,
  });

  const toggleStatusMutation = useMutation({
    mutationFn: async ({ id, isActive }: { id: string; isActive: boolean }) => {
      await api.patch(`/links/${id}/status`, { isActive });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminLinks'] });
    }
  });

  const hardDeleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/admin/links/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminLinks'] });
    }
  });

  if (isLoading) {
    return <div className="animate-pulse h-64 bg-slate-800/50 rounded-xl w-full"></div>;
  }

  const baseUrl = API_BASE_URL.replace('/api', '/');

  return (
    <div className="space-y-4">
      <div className="bg-slate-900/50 border border-slate-800 rounded-xl overflow-hidden overflow-x-auto">
        <table className="w-full text-left text-sm text-slate-400">
          <thead className="text-xs text-slate-500 uppercase bg-slate-900/80 border-b border-slate-800">
            <tr>
              <th className="px-4 py-4">Short Code</th>
              <th className="px-4 py-4">Original URL</th>
              <th className="px-4 py-4">User ID</th>
              <th className="px-4 py-4">Visits</th>
              <th className="px-4 py-4">Status</th>
              <th className="px-4 py-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-800">
            {data?.items.map((link) => (
              <tr key={link.id} className={`hover:bg-slate-800/50 transition-colors ${link.isDeleted ? 'opacity-50' : ''}`}>
                <td className="px-4 py-4 font-mono text-indigo-400">
                  <a href={baseUrl + link.shortCode} target="_blank" rel="noopener noreferrer" className="hover:underline flex items-center gap-1">
                    {link.shortCode}
                    <ExternalLink size={12} />
                  </a>
                </td>
                <td className="px-4 py-4 max-w-[200px] truncate" title={link.originalUrl}>
                  {link.originalUrl}
                </td>
                <td className="px-4 py-4 truncate max-w-[150px]" title={link.userEmail}>
                  {link.userEmail || 'System'}
                </td>
                <td className="px-4 py-4">{link.visitsCount}</td>
                <td className="px-4 py-4">
                  <div className="flex gap-2">
                    {link.isDeleted ? (
                       <span className="px-2 py-1 text-xs rounded bg-red-500/10 text-red-500 border border-red-500/20">Deleted</span>
                    ) : (
                       <span className={`px-2 py-1 text-xs rounded border ${link.isActive ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20' : 'bg-amber-500/10 text-amber-500 border-amber-500/20'}`}>
                         {link.isActive ? 'Active' : 'Inactive'}
                       </span>
                    )}
                  </div>
                </td>
                <td className="px-4 py-4 text-right">
                  <div className="flex items-center justify-end gap-2">
                    {!link.isDeleted && (
                      <button
                        onClick={() => toggleStatusMutation.mutate({ id: link.id, isActive: !link.isActive })}
                        disabled={toggleStatusMutation.isPending}
                        className={`p-1.5 rounded transition-colors ${link.isActive ? 'text-slate-400 hover:text-white bg-slate-800 hover:bg-amber-600' : 'text-amber-500 bg-amber-500/10 hover:bg-amber-500 hover:text-white'}`}
                        title={link.isActive ? "Deactivate" : "Activate"}
                      >
                        <Power size={14} />
                      </button>
                    )}
                    <button
                      onClick={() => {
                        if(window.confirm('Are you sure you want to HARD DELETE this link permanently?')) {
                          hardDeleteMutation.mutate(link.id);
                        }
                      }}
                      disabled={hardDeleteMutation.isPending}
                      className="p-1.5 text-slate-400 hover:text-white bg-slate-800 hover:bg-red-600 rounded transition-colors"
                      title="Hard Delete"
                    >
                      <Trash2 size={14} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {data?.items.length === 0 && (
              <tr>
                <td colSpan={6} className="px-6 py-8 text-center text-slate-500">No links found.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <span className="text-sm text-slate-400">
            Page {data.pageNumber} of {data.totalPages}
          </span>
          <div className="flex items-center gap-2">
            <button
              onClick={() => setPage((old) => Math.max(old - 1, 1))}
              disabled={page === 1 || isFetching}
              className="p-2 bg-slate-800 hover:bg-indigo-600 disabled:opacity-50 disabled:hover:bg-slate-800 rounded-lg transition-colors text-white"
            >
              <ChevronLeft size={18} />
            </button>
            <button
              onClick={() => setPage((old) => (data.hasNextPage ? old + 1 : old))}
              disabled={!data.hasNextPage || isFetching}
              className="p-2 bg-slate-800 hover:bg-indigo-600 disabled:opacity-50 disabled:hover:bg-slate-800 rounded-lg transition-colors text-white"
            >
              <ChevronRight size={18} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminLinksTable;
