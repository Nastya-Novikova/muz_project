import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
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
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/profile/edit" element={<EditProfilePage />} />
          <Route path="/suggestions" element={<SuggestionsPage />} />
          <Route path="/notifications" element={<NotificationsPage />} />
          <Route path="/profile/:userId" element={<ProfilePage />} />
          <Route path="/events" element={<EventsPage />} />
          <Route path="/events/create" element={<EventFormPage />} />
          <Route path="/events/:eventId/edit" element={<EventFormPage />} />
          <Route path="/events/:eventId" element={<EventDetailPage />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
}

export default App;