const API_URL = 'http://localhost:7000/api';

const getAuthHeaders = (token) => ({
  'Content-Type': 'application/json',
  'Authorization': `Bearer ${token}`
});

export const api = {
  // Получить города
  async getCities() {
    const response = await fetch(`${API_URL}/Metadata/cities`);
    return await response.json();
  },

  // Получить виды деятельности
  async getActivities() {
    const response = await fetch(`${API_URL}/Metadata/activities`);
    return await response.json();
  },

  // Получить жанры
  async getGenres() {
    const response = await fetch(`${API_URL}/Metadata/genres`);
    return await response.json();
  },

  // Получить код
  async requestAuthCode(email) {
    const response = await fetch(`${API_URL}/Auth/request-code`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email }),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Войти по коду
  async loginWithCode(email, code) {
    const response = await fetch(`${API_URL}/Auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, code }),
    });

    if (!response.ok) {
      const errorData = await response.json().catch();
      throw new Error();
    }

    return response.json();
  },

  // Получить профиль
  async getProfile(token) { // Принимаем токен
    const response = await fetch(`${API_URL}/Profiles`, {
      method: 'GET',
      headers: getAuthHeaders(token), // Используем токен
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Создать профиль
  async createProfile(profileData, token) { // Принимаем токен
    const response = await fetch(`${API_URL}/Profiles`, {
      method: 'POST',
      headers: getAuthHeaders(token), // Используем токен
      body: JSON.stringify(profileData),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Обновить профиль
  async updateProfile(profileData, token) { // Принимаем токен
    const response = await fetch(`${API_URL}/Profiles`, {
      method: 'PUT',
      headers: getAuthHeaders(token), // Используем токен
      body: JSON.stringify(profileData),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Загрузить аватар
  async uploadAvatar(file, token) { // Принимаем токен
    const formData = new FormData();
    formData.append('avatar', file);

    const response = await fetch(`${API_URL}/Uploads/avatar`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}` // <-- Только авторизация, без Content-Type
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Загрузить аудио
  async uploadAudio(file, title, token, description = '') { // Принимаем токен
    const formData = new FormData();
    formData.append('audio', file);
    formData.append('title', title);
    if (description) formData.append('description', description);

    const response = await fetch(`${API_URL}/Uploads/portfolio/audio`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}` // <-- Только авторизация, без Content-Type
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Загрузить фото
  async uploadPhoto(file, title, token, description = '') { // Принимаем токен
    const formData = new FormData();
    formData.append('photo', file);
    formData.append('title', title);
    if (description) formData.append('description', description);

    const response = await fetch(`${API_URL}/Uploads/portfolio/photo`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}` // <-- Только авторизация, без Content-Type
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  //Поиск музыкантов
  async searchMusicians(searchParams) {
    const response = await fetch(`${API_URL}/Profiles/search`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(searchParams),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  //Поиск пользователя по ID
  async getProfileById(userId) {
    const response = await fetch(`${API_URL}/Profiles/${userId}`, {
      method: 'GET'
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  }
};