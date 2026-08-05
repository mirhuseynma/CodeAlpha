import React, { useState } from 'react';
import { X, MapPin, Monitor, Globe, Clock, ChevronLeft, ChevronRight } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import api from '../../services/api';

interface AnalyticsModalProps {
  isOpen: boolean;
  onClose: () => void;
  linkId: string;
  shortUrl: string;
}

interface UrlVisitDto {
  ipAddress: string | null;
  country: string | null;
  userAgent: string | null;
  referer: string | null;
  visitedAt: string;
}

interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

const AnalyticsModal: React.FC<AnalyticsModalProps> = ({ isOpen, onClose, linkId, shortUrl }) => {
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const { data, isLoading, isError, isFetching } = useQuery({
    queryKey: ['analytics', linkId, page],
    queryFn: async () => {
      const response = await api.get<PagedResult<UrlVisitDto>>(`/links/${linkId}/analytics?pageNumber=${page}&pageSize=${pageSize}`);
      return response.data;
    },
    enabled: isOpen,
  });

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* Backdrop */}
      <div 
        className="absolute inset-0 bg-black/60 backdrop-blur-sm"
        onClick={onClose}
      />
      
      {/* Modal */}
      <div className="relative bg-slate-900 border border-slate-800 w-full max-w-4xl rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[85vh]">
        
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-slate-800">
          <div>
            <h2 className="text-xl font-semibold text-white">Link Analytics</h2>
            <p className="text-sm text-indigo-400 mt-1">{shortUrl}</p>
          </div>
          <button 
            onClick={onClose}
            className="p-2 text-slate-400 hover:text-white hover:bg-slate-800 rounded-lg transition-colors"
          >
            <X size={20} />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 overflow-y-auto flex-1">
          {isLoading ? (
            <div className="flex justify-center items-center py-20">
              <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500"></div>
            </div>
          ) : isError ? (
            <p className="text-red-400 text-center py-20">Failed to load analytics.</p>
          ) : data?.items.length === 0 ? (
            <div className="text-center py-20">
              <Globe size={48} className="mx-auto text-slate-600 mb-4" />
              <p className="text-slate-400">No visits recorded yet for this link.</p>
            </div>
          ) : (
            <div className="space-y-4">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-slate-300 font-medium">Recent Visits</h3>
                <span className="text-xs font-semibold px-2.5 py-1 bg-indigo-500/20 text-indigo-300 rounded-full border border-indigo-500/30">
                  Total: {data?.totalCount}
                </span>
              </div>
              
              <div className="border border-slate-800 rounded-xl overflow-hidden">
                <table className="w-full text-left text-sm text-slate-400">
                  <thead className="text-xs text-slate-500 uppercase bg-slate-900/50 border-b border-slate-800">
                    <tr>
                      <th className="px-4 py-3">Time</th>
                      <th className="px-4 py-3">Location (IP)</th>
                      <th className="px-4 py-3">Device / Browser</th>
                      <th className="px-4 py-3">Referer</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-800">
                    {data?.items.map((visit, idx) => (
                      <tr key={idx} className="hover:bg-slate-800/50 transition-colors">
                        <td className="px-4 py-3 whitespace-nowrap">
                          <div className="flex items-center gap-2">
                            <Clock size={14} className="text-slate-500" />
                            <span>{new Date(visit.visitedAt).toLocaleString()}</span>
                          </div>
                        </td>
                        <td className="px-4 py-3">
                          <div className="flex items-center gap-2">
                            <MapPin size={14} className="text-rose-400" />
                            <span>{visit.country || 'Unknown'}</span>
                            <span className="text-xs text-slate-600 ml-1">({visit.ipAddress || 'Unknown IP'})</span>
                          </div>
                        </td>
                        <td className="px-4 py-3">
                          <div className="flex items-center gap-2">
                            <Monitor size={14} className="text-indigo-400" />
                            <span className="truncate max-w-[200px]" title={visit.userAgent}>{visit.userAgent || 'Unknown'}</span>
                          </div>
                        </td>
                        <td className="px-4 py-3 truncate max-w-[150px]" title={visit.referer}>
                          {visit.referer || '-'}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Pagination */}
              {data && data.totalPages > 1 && (
                <div className="flex items-center justify-between pt-4">
                  <span className="text-sm text-slate-400">
                    Page {data.pageNumber} of {data.totalPages}
                  </span>
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => setPage((old) => Math.max(old - 1, 1))}
                      disabled={page === 1 || isFetching}
                      className="p-1.5 bg-slate-800 hover:bg-indigo-600 disabled:opacity-50 disabled:hover:bg-slate-800 rounded-lg transition-colors text-white"
                    >
                      <ChevronLeft size={16} />
                    </button>
                    <button
                      onClick={() => setPage((old) => (data.hasNextPage ? old + 1 : old))}
                      disabled={!data.hasNextPage || isFetching}
                      className="p-1.5 bg-slate-800 hover:bg-indigo-600 disabled:opacity-50 disabled:hover:bg-slate-800 rounded-lg transition-colors text-white"
                    >
                      <ChevronRight size={16} />
                    </button>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AnalyticsModal;
