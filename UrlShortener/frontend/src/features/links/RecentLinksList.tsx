import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { Copy, ExternalLink, Activity, ChevronLeft, ChevronRight, Trash2, Power, BarChart2 } from 'lucide-react';
import api from '../../services/api';
import type { ShortLinkResponse } from './CreateLinkForm';
import { useState } from 'react';
import AnalyticsModal from './AnalyticsModal';

interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

const RecentLinksList = () => {
  const [copiedId, setCopiedId] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const pageSize = 5; // Reduced back to 5 so pagination is visible
  
  const [analyticsLink, setAnalyticsLink] = useState<{id: string, url: string} | null>(null);
  const queryClient = useQueryClient();

  const { data, isLoading, isError, isFetching } = useQuery({
    queryKey: ['links', page],
    queryFn: async () => {
      const response = await api.get<PagedResult<ShortLinkResponse>>(`/links?pageNumber=${page}&pageSize=${pageSize}`);
      return response.data;
    },
    placeholderData: keepPreviousData,
  });

  const toggleStatusMutation = useMutation({
    mutationFn: async ({ id, isActive }: { id: string; isActive: boolean }) => {
      await api.patch(`/links/${id}/status`, { isActive });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['links'] });
    }
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/links/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['links'] });
    }
  });

  const handleCopy = async (id: string, url: string) => {
    try {
      await navigator.clipboard.writeText(url);
      setCopiedId(id);
      setTimeout(() => setCopiedId(null), 2000);
    } catch (err) {
      console.error('Failed to copy', err);
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-10">
        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  if (isError) {
    return (
      <p className="text-red-400 text-center py-10">
        Failed to load recent links.
      </p>
    );
  }

  const links = data?.items || [];

  if (links.length === 0) {
    return (
      <p className="text-slate-400 text-center py-10">
        You haven't created any links yet.
      </p>
    );
  }

  return (
    <div className="space-y-4">
      {links.map((link) => (
        <div key={link.id} className="bg-slate-900/50 border border-slate-800/50 p-4 rounded-xl flex items-center justify-between group hover:border-indigo-500/50 transition-colors">
          
          <div className="flex-1 min-w-0 pr-4 space-y-1">
            <a 
              href={link.shortUrl} 
              target="_blank" 
              rel="noopener noreferrer"
              className="text-lg font-mono text-indigo-400 hover:text-indigo-300 truncate block"
            >
              {link.shortUrl}
            </a>
            <div className="flex items-center gap-2 text-sm text-slate-500 truncate">
              <span className="truncate" title={link.originalUrl}>{link.originalUrl}</span>
            </div>
          </div>

          <div className="flex items-center gap-3">
            {!link.isActive && (
               <span className="text-xs font-semibold px-2 py-1 bg-amber-500/10 text-amber-500 rounded-md border border-amber-500/20 mr-2">
                 Inactive
               </span>
            )}
            <div className="flex items-center gap-1.5 px-3 py-1 bg-slate-800/80 rounded-md border border-slate-700/50" title="Total Clicks">
              <Activity size={14} className="text-cyan-400" />
              <span className="text-sm font-medium text-slate-300">{link.totalClicks}</span>
            </div>
            
            <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              <button
                onClick={() => setAnalyticsLink({ id: link.id, url: link.shortUrl })}
                className="p-2 text-slate-400 hover:text-white bg-slate-800 hover:bg-emerald-600 rounded-lg transition-colors"
                title="View Analytics"
              >
                <BarChart2 size={16} />
              </button>
              
              <button
                onClick={() => handleCopy(link.id, link.shortUrl)}
                className="p-2 text-slate-400 hover:text-white bg-slate-800 hover:bg-indigo-600 rounded-lg transition-colors"
                title="Copy to clipboard"
              >
                {copiedId === link.id ? <span className="text-xs font-bold text-white">Copied</span> : <Copy size={16} />}
              </button>
              <a
                href={link.shortUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="p-2 text-slate-400 hover:text-white bg-slate-800 hover:bg-cyan-600 rounded-lg transition-colors"
                title="Open link"
              >
                <ExternalLink size={16} />
              </a>
              
              <div className="w-px h-6 bg-slate-700 mx-1"></div>
              
              <button
                onClick={() => toggleStatusMutation.mutate({ id: link.id, isActive: !link.isActive })}
                disabled={toggleStatusMutation.isPending}
                className={`p-2 rounded-lg transition-colors ${link.isActive ? 'text-slate-400 hover:text-white bg-slate-800 hover:bg-amber-600' : 'text-amber-500 bg-amber-500/10 hover:bg-amber-500 hover:text-white'}`}
                title={link.isActive ? "Deactivate link" : "Activate link"}
              >
                <Power size={16} />
              </button>
              
              <button
                onClick={() => {
                  if(window.confirm('Are you sure you want to delete this link?')) {
                    deleteMutation.mutate(link.id);
                  }
                }}
                disabled={deleteMutation.isPending}
                className="p-2 text-slate-400 hover:text-white bg-slate-800 hover:bg-rose-600 rounded-lg transition-colors"
                title="Delete link"
              >
                <Trash2 size={16} />
              </button>
            </div>
          </div>
          
        </div>
      ))}
      
      {/* Pagination Controls */}
      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between pt-4 mt-4 border-t border-slate-800/50">
          <span className="text-sm text-slate-400">
            Page {data.pageNumber} of {data.totalPages}
          </span>
          <div className="flex items-center gap-2">
            <button
              onClick={() => setPage((old) => Math.max(old - 1, 1))}
              disabled={page === 1 || isFetching}
              className="p-2 bg-slate-800/80 hover:bg-indigo-600 disabled:opacity-50 disabled:hover:bg-slate-800/80 rounded-lg transition-colors flex items-center justify-center text-white"
            >
              <ChevronLeft size={18} />
            </button>
            <button
              onClick={() => setPage((old) => (data?.hasNextPage ? old + 1 : old))}
              disabled={!data.hasNextPage || isFetching}
              className="p-2 bg-slate-800/80 hover:bg-indigo-600 disabled:opacity-50 disabled:hover:bg-slate-800/80 rounded-lg transition-colors flex items-center justify-center text-white"
            >
              <ChevronRight size={18} />
            </button>
          </div>
        </div>
      )}
      
      {/* Analytics Modal */}
      {analyticsLink && (
        <AnalyticsModal 
          isOpen={!!analyticsLink}
          onClose={() => setAnalyticsLink(null)}
          linkId={analyticsLink.id}
          shortUrl={analyticsLink.url}
        />
      )}
    </div>
  );
};

export default RecentLinksList;