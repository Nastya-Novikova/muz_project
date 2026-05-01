import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute';
import HomePage from './pages/Home/HomePage';
import LoginOTP from './pages/Login/LoginPage';
import ProfilePage from './pages/Profile/ProfilePage/ProfilePage';
import EditProfilePage from './pages/Profile/EditProfile/EditProfilePage';
import SuggestionsPage from './pages/Suggestions/SuggestionsPage';
import NotificationsPage from './pages/Notifications/NotificationsPage';
import EventsPage from './pages/Events/EventsPage/EventsPage';
import EventFormPage from './pages/Events/EventFormPage/EventFormPage';
import EventDetailPage from './pages/Events/EventDetailPage/EventDetailPage';

function App() {
  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginOTP />} />
          <Route path="/" element={<HomePage />} />
          <Route path="/profile" element={
            <ProtectedRoute><ProfilePage /></ProtectedRoute>
          } />
          <Route path="/profile/edit" element={
            <ProtectedRoute><EditProfilePage /></ProtectedRoute>
          } />
          <Route path="/suggestions" element={
            <ProtectedRoute><SuggestionsPage /></ProtectedRoute>
          } />
          <Route path="/notifications" element={
            <ProtectedRoute><NotificationsPage /></ProtectedRoute>
          } />
          <Route path="/profile/:userId" element={
            <ProtectedRoute><ProfilePage /></ProtectedRoute>
          } />
          <Route path="/events" element={
            <ProtectedRoute><EventsPage /></ProtectedRoute>
          } />
          <Route path="/events/create" element={
            <ProtectedRoute><EventFormPage /></ProtectedRoute>
          } />
          <Route path="/events/:eventId/edit" element={
            <ProtectedRoute><EventFormPage /></ProtectedRoute>
          } />
          <Route path="/events/:eventId" element={
            <ProtectedRoute><EventDetailPage /></ProtectedRoute>
          } />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
}

export default App;