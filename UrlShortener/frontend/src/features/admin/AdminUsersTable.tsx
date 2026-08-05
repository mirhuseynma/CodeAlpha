import { useState } from 'react';
import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { ChevronLeft, ChevronRight, Trash2 } from 'lucide-react';
import api from '../../services/api';

interface AdminUserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  linksCount: number;
  createdAt: string;
}

interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

const AdminUsersTable = () => {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const queryClient = useQueryClient();

  const { data, isLoading, isFetching } = useQuery({
    queryKey: ['adminUsers', page],
    queryFn: async () => {
      const response = await api.get<PagedResult<AdminUserDto>>(`/admin/users?pageNumber=${page}&pageSize=${pageSize}`);
      return response.data;
    },
    placeholderData: keepPreviousData,
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/admin/users/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminUsers'] });
    }
  });

  if (isLoading) {
    return <div className="animate-pulse h-64 bg-slate-800/50 rounded-xl w-full"></div>;
  }

  return (
    <div className="space-y-4">
      <div className="bg-slate-900/50 border border-slate-800 rounded-xl overflow-hidden">
        <table className="w-full text-left text-sm text-slate-400">
          <thead className="text-xs text-slate-500 uppercase bg-slate-900/80 border-b border-slate-800">
            <tr>
              <th className="px-6 py-4">Name</th>
              <th className="px-6 py-4">Email</th>
              <th className="px-6 py-4">Joined At</th>
              <th className="px-6 py-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-800">
            {data?.items.map((user) => (
              <tr key={user.id} className="hover:bg-slate-800/50 transition-colors">
                <td className="px-6 py-4 font-medium text-white">{user.firstName} {user.lastName}</td>
                <td className="px-6 py-4">{user.email}</td>
                <td className="px-6 py-4">{new Date(user.createdAt).toLocaleDateString()}</td>
                <td className="px-6 py-4 text-right">
                  <button
                    onClick={() => {
                      if(window.confirm('Are you sure you want to permanently delete this user and all their links?')) {
                        deleteMutation.mutate(user.id);
                      }
                    }}
                    disabled={deleteMutation.isPending}
                    className="p-1.5 text-slate-400 hover:text-white bg-slate-800 hover:bg-red-600 rounded transition-colors"
                    title="Delete User"
                  >
                    <Trash2 size={14} />
                  </button>
                </td>
              </tr>
            ))}
            {data?.items.length === 0 && (
              <tr>
                <td colSpan={4} className="px-6 py-8 text-center text-slate-500">No users found.</td>
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

export default AdminUsersTable;
