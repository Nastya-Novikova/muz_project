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
  async uploadAvatar(file, token) {
    const formData = new FormData();
    formData.append('avatar', file);

    const response = await fetch(`${API_URL}/Uploads/avatar`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  //Конвертировать аватар в необходимый формат
  convertAvatarBytesToUrl(avatarBytes) {
    if (!avatarBytes || !avatarBytes.length) return null;
    
    try {
      if (typeof avatarBytes === 'string') {
        if (avatarBytes.startsWith('data:image')) {
          return avatarBytes;
        }
        return `data:image/jpeg;base64,${avatarBytes}`;
      }
      return `data:image/jpeg;base64,${base64String}`;
    } catch (error) {
      console.error('Error converting avatar bytes:', error);
      return null;
    }
  },

  //Загрузить аудио
  async uploadAudio(file, title, token, description = '') {
    const formData = new FormData();
    formData.append('audio', file);
    formData.append('title', title);
    if (description) formData.append('description', description);

    const response = await fetch(`${API_URL}/Uploads/portfolio/audio`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  async getMedia(profileId, token) {
    const response = await fetch(`${API_URL}/Profiles/${profileId}/media`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  convertAudioBytesToUrl(audioBytes) {
    if (!audioBytes || !audioBytes.length) return null;
    
    try {
      if (typeof audioBytes === 'string') {
        if (audioBytes.startsWith('data:audio')) {
          return audioBytes;
        }
        return `data:audio/mpeg;base64,${audioBytes}`;
      }
      
      return `data:audio/mpeg;base64,${base64String}`;
    } catch (error) {
      console.error('Error converting audio bytes:', error);
      return null;
    }
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
  },
  
  //Направить предложение о сотрудничестве
  async sendSuggestion(profileId, message=" ", token) {
    const response = await fetch(`${API_URL}/Collaborations/${profileId}`, {
      method: 'POST',
      headers: getAuthHeaders(token),
      body: JSON.stringify({ message })
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

// Получить предложения мне
  async getReceivedSuggestions(token, page = 1, limit = 20, sortBy = "createdAt", sortDesc = true) {
    const params = new URLSearchParams({
      page,
      limit,
      sortBy,
      sortDesc
    });

    const response = await fetch(`${API_URL}/Collaborations/received?${params}`, {
        method: 'GET',
        headers: getAuthHeaders(token)
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
        throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
      }

    return response.json();
  },

  // Получить мои предложения (отправленные)
  async getSentSuggestions(token, page = 1, limit = 20, sortBy = "createdAt", sortDesc = true) {
    const params = new URLSearchParams({
      page,
      limit,
      sortBy,
      sortDesc
    });

    const response = await fetch(`${API_URL}/Collaborations/sent?${params}`, {
      method: 'GET',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

// Получить избранные профили
async getFavorites(token, page = 1, limit = 20) {
  const params = new URLSearchParams({
    page,
    limit
  });

  const response = await fetch(`${API_URL}/Favorites?${params}`, {
    method: 'GET',
    headers: getAuthHeaders(token)
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
    throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
  }

  return response.json();
},

  // Добавить в избранное
  async addToFavorites(favoriteUserId, token) {
    const response = await fetch(`${API_URL}/Favorites/${favoriteUserId}`, {
      method: 'POST',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Удалить из избранного
  async removeFromFavorites(favoriteUserId, token) {
    const response = await fetch(`${API_URL}/Favorites/${favoriteUserId}`, {
      method: 'DELETE',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Проверить, добавлен ли пользователь в избранное
  async checkIsFavorite(favoriteUserId, token) {
    const response = await fetch(`${API_URL}/Favorites/${favoriteUserId}`, {
      method: 'GET',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Проверить наличие предложения пользователю
  async checkCollaboration(collaboratedProfileId, token) {
    const response = await fetch(`${API_URL}/Collaborations/${collaboratedProfileId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return response.json();
  }
};