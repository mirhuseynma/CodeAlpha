import React from 'react';
import { BrowserRouter, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import Navbar from './components/Navbar';
import Login from './pages/Login';
import Register from './pages/Register';
import Events from './pages/Events';
import MyRegistrations from './pages/MyRegistrations';

import EventDetails from './pages/EventDetails';
import AdminLayout from './pages/admin/AdminLayout';
import AdminDashboard from './pages/admin/AdminDashboard';
import AdminEvents from './pages/admin/AdminEvents';
import AdminUsers from './pages/admin/AdminUsers';
import EventRegistrations from './pages/admin/EventRegistrations';

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/admin" element={<AdminLayout />}>
          <Route index element={<AdminDashboard />} />
          <Route path="events" element={<AdminEvents />} />
          <Route path="events/:id/registrations" element={<EventRegistrations />} />
          <Route path="users" element={<AdminUsers />} />
        </Route>

        <Route path="/" element={<div className="container"><Navbar /><Outlet /></div>}>
          <Route index element={<Navigate to="/events" />} />
          <Route path="login" element={<Login />} />
          <Route path="register" element={<Register />} />
          <Route path="events" element={<Events />} />
          <Route path="events/:id" element={<EventDetails />} />
          <Route path="my-registrations" element={<MyRegistrations />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
