const API_URL = '/api';

const handleUnauthorized = () => {
  localStorage.removeItem('musicianFinder_token');
  localStorage.removeItem('user_email');
  localStorage.removeItem('userRole');
  window.location.href = '/login';
};

// Функция-обёртка для fetch с проверкой статуса
const authFetch = async (url, options = {}) => {
  const response = await fetch(url, options);
  
  if (response.status === 401) {
    handleUnauthorized();
    throw new Error('Unauthorized');
  }
  
  return response;
};
const getAuthHeaders = (token) => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
});

const generateIdempotencyKey = () => crypto.randomUUID();

// Внутренний метод загрузки медиа
const uploadMedia= async (file, title, type, description, token) => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('title', title);
    formData.append('type', type);
    if (description) formData.append('description', description);
    const response = await authFetch(`${API_URL}/profiles/me/media`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Idempotency-Key': generateIdempotencyKey()
        },
        body: formData
    });
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    return response.json();
    }

export const api = {
    // Получить города
    async getCities() {
        const response = await fetch(`${API_URL}/metadata/cities`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    // Получить виды деятельности (специальности)
    async getActivities() {
        const response = await fetch(`${API_URL}/metadata/specialties`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    // Получить жанры
    async getGenres() {
        const response = await fetch(`${API_URL}/metadata/genres`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    // Получить список регионов
    async getRegions() {
        const response = await fetch(`${API_URL}/metadata/regions`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Запросить код на email
    async requestAuthCode(email) {
        const response = await fetch(`${API_URL}/auth/code`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email })
        });
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Войти по коду
    async loginWithCode(email, code) {
        const response = await fetch(`${API_URL}/auth/session`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, code })
        });
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'Invalid credentials' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Получить профиль текущего пользователя
    async getProfile(token) {
        const response = await authFetch(`${API_URL}/profiles/me`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Создать профиль
    async createProfile(profileData, token) {
        const response = await authFetch(`${API_URL}/profiles`, {
            method: 'POST',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify(profileData)
        });
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Частичное обновление профиля
    async updateProfile(profileData, token) {
        const response = await authFetch(`${API_URL}/profiles/me`, {
            method: 'PATCH',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify(profileData)
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
    },

    // Удалить профиль
    async deleteProfile(token) {
        const response = await authFetch(`${API_URL}/profiles/me`, {
            method: 'DELETE',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
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
        const response = await authFetch(`${API_URL}/profiles/me/avatar`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: formData
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Загрузить аудио в портфолио
    async uploadAudio(file, title, token, description = '') {
        return uploadMedia(file, title, 'Audio', description, token);
    },

    // Загрузить видео в портфолио
    async uploadVideo(file, title, token, description = '') {
        return uploadMedia(file, title, 'Video', description, token);
    },

    // Загрузить фото в портфолио
    async uploadPhoto(file, title, token, description = '') {
        return uploadMedia(file, title, 'Photo', description, token);
    },

    // Удалить медиа из портфолио
    async deleteMedia(mediaId, token) {
        const response = await authFetch(`${API_URL}/profiles/me/media/${mediaId}`, {
            method: 'DELETE',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    },

    // Временный метод получения медиа (через профиль)
    async getMedia(profileId, token) {
        const response = await authFetch(`${API_URL}/profiles/${profileId}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        const profile = await response.json();
        return {
            audio: profile.audio || [],
            video: profile.video || [],
            photos: profile.photos || []
        };
    },

    // Вспомогательная функция для URL аватара
    getAvatarUrl(avatarUrl) {
        if (!avatarUrl) return '/default-avatar.png';
        if (typeof avatarUrl === 'string' && (avatarUrl.startsWith('http') || avatarUrl.startsWith('data:'))) {
            return avatarUrl;
        }
        return '/default-avatar.png';
    },

    // Вспомогательная функция для URL аудио
    getAudioUrl(audioUrl) {
        if (!audioUrl) return null;
        if (typeof audioUrl === 'string' && (audioUrl.startsWith('http') || audioUrl.startsWith('data:'))) {
            return audioUrl;
        }
        return null;
    },

    // Поиск музыкантов
    async searchMusicians(searchParams = {}) {
        const params = new URLSearchParams();
        Object.entries(searchParams).forEach(([key, value]) => {
            if (value !== undefined && value !== null && value !== '') {
                if (Array.isArray(value)) {
                    value.forEach(v => params.append(key, v));
                } else {
                    params.append(key, value);
                }
            }
        });
        const query = params.toString();
        const url = `${API_URL}/profiles${query ? '?' + query : ''}`;
        const response = await fetch(url);
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Получить профиль по ID
    async getProfileById(userId) {
        const response = await authFetch(`${API_URL}/profiles/${userId}`);
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Отправить предложение о сотрудничестве
    async sendSuggestion(toProfileId, message, token) {
        const response = await authFetch(`${API_URL}/suggestions`, {
            method: 'POST',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify({ toProfileId, message })
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Получить входящие предложения
    async getReceivedSuggestions(token, page = 1, limit = 20) {
        const params = new URLSearchParams({ page, limit });
        const response = await authFetch(`${API_URL}/suggestions/received?${params}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить исходящие предложения
    async getSentSuggestions(token, page = 1, limit = 20) {
        const params = new URLSearchParams({ page, limit });
        const response = await authFetch(`${API_URL}/suggestions/sent?${params}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить избранные профили
    async getFavorites(token, page = 1, limit = 20) {
        const params = new URLSearchParams({ page, limit });
        const response = await authFetch(`${API_URL}/me/favorites?${params}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Добавить в избранное
    async addToFavorites(profileId, token) {
        const response = await authFetch(`${API_URL}/${profileId}/favorite`, {
            method: 'PUT',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }
        
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Удалить из избранного
    async removeFromFavorites(profileId, token) {
        const response = await authFetch(`${API_URL}/${profileId}/favorite`, {
            method: 'DELETE',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить уведомления
    async getNotifications(token, page = 1, limit = 20) {
        const params = new URLSearchParams({ page, limit });
        const response = await authFetch(`${API_URL}/notifications?${params}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Отметить уведомление как прочитанное
    async markNotificationAsRead(notificationId, token) {
        const response = await authFetch(`${API_URL}/notifications/${notificationId}/read`, {
            method: 'PATCH',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Отметить все уведомления как прочитанные
    async markAllNotificationsAsRead(token) {
        const response = await authFetch(`${API_URL}/notifications/read-all`, {
            method: 'POST',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить количество непрочитанных уведомлений
    async getUnreadNotificationsCount(token) {
        const response = await authFetch(`${API_URL}/notifications/unread-count`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить настройки уведомлений
    async getNotificationSettings(token) {
        const response = await authFetch(`${API_URL}/notifications/settings`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Обновить настройки уведомлений
    async updateNotificationSettings(settings, token) {
        const response = await authFetch(`${API_URL}/notifications/settings`, {
            method: 'PUT',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify(settings)
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Привязать аккаунт ВКонтакте
    async connectVk(code, codeVerifier, deviceId, token) {
        const response = await authFetch(`${API_URL}/profiles/me/connect-vk`, {
            method: 'POST',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify({ code, codeVerifier, deviceId })
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        return response.json();
    },

    // Получить ленту мероприятий
    async getEvents(filterParams = {}) {
        const params = new URLSearchParams();
        Object.entries(filterParams).forEach(([key, value]) => {
            if (value !== undefined && value !== null && value !== '') {
                if (Array.isArray(value)) {
                    value.forEach(v => params.append(key, v));
                } else {
                    params.append(key, value);
                }
            }
        });
        const query = params.toString();
        const url = `${API_URL}/events${query ? '?' + query : ''}`;
        const response = await authFetch(url);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить мероприятие по ID
    async getEventById(eventId, token) {
        const headers = token ? getAuthHeaders(token) : {};
        const response = await authFetch(`${API_URL}/events/${eventId}`, { headers });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Создать мероприятие
    async createEvent(eventData, token) {
        const response = await authFetch(`${API_URL}/events`, {
            method: 'POST',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify(eventData)
        });
        
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Обновить мероприятие
    async updateEvent(eventId, eventData, token) {
        const response = await authFetch(`${API_URL}/events/${eventId}`, {
            method: 'PATCH',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: JSON.stringify(eventData)
        });

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Отменить мероприятие
    async cancelEvent(eventId, token) {
        const response = await authFetch(`${API_URL}/events/${eventId}`, {
            method: 'DELETE',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Записаться на мероприятие
    async registerForEvent(eventId, token) {
        const response = await authFetch(`${API_URL}/events/${eventId}/registration`, {
            method: 'POST',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Отменить запись на мероприятие
    async unregisterFromEvent(eventId, token) {
        const response = await authFetch(`${API_URL}/events/${eventId}/registration`, {
            method: 'DELETE',
            headers: {
                ...getAuthHeaders(token),
                'Idempotency-Key': generateIdempotencyKey()
            }
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить мероприятия, созданные пользователем
    async getMyCreatedEvents(token, page = 1, limit = 20) {
        const params = new URLSearchParams({ page, limit });
        const response = await authFetch(`${API_URL}/events/created?${params}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Получить мероприятия, на которые записан пользователь
    async getMyRegisteredEvents(token, page = 1, limit = 20) {
        const params = new URLSearchParams({ page, limit });
        const response = await authFetch(`${API_URL}/events/registered?${params}`, {
            headers: getAuthHeaders(token)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    },

    // Загрузить изображение мероприятия
    async uploadEventImage(eventId, file, token) {
        const formData = new FormData();
        formData.append('image', file);
        const response = await authFetch(`${API_URL}/events/${eventId}/image`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Idempotency-Key': generateIdempotencyKey()
            },
            body: formData
        });

        if (response.status === 204 || response.status === 201) {
          return { success: true };
        }

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
    }
};
/*const API_URL = '/api';

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
    const response = await fetch(`${API_URL}/auth/request-code`, {
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
  async createProfile(profileData, token) { 
    const response = await fetch(`${API_URL}/Profiles`, {
      method: 'POST',
      headers: getAuthHeaders(token),
      body: JSON.stringify(profileData),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Обновить профиль
  async updateProfile(profileData, token) {
    const response = await fetch(`${API_URL}/Profiles`, {
      method: 'PUT',
      headers: getAuthHeaders(token),
      body: JSON.stringify(profileData),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Удалить профиль
  async deleteProfile(token) {
    const response = await fetch(`${API_URL}/Profiles`, {
      method: 'DELETE',
      headers: getAuthHeaders(token),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  async uploadAvatar(file, token) {
    const formData = new FormData();
    formData.append('avatar', file);

    const response = await fetch(`${API_URL}/Uploads/avatar`, {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData,
    });
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    return response.json(); 
  },

  async uploadAudio(file, title, token, description = '') {
    const formData = new FormData();
    formData.append('audio', file);
    formData.append('title', title);
    if (description) formData.append('description', description);

    const response = await fetch(`${API_URL}/Uploads/portfolio/audio`, {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData,
    });
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    return response.json(); 
  },

  async uploadVideo(file, title, token, description = '') {
    const formData = new FormData();
    formData.append('video', file);
    formData.append('title', title);
    if (description) formData.append('description', description);

    const response = await fetch(`${API_URL}/Uploads/portfolio/video`, {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData,
    });
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    return response.json();
  },

  async uploadPhoto(file, title, token, description = '') {
    const formData = new FormData();
    formData.append('photo', file);
    formData.append('title', title);
    if (description) formData.append('description', description);

    const response = await fetch(`${API_URL}/Uploads/portfolio/photo`, {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData,
    });
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    return response.json();
  },

  async getMedia(profileId, token) {
    const response = await fetch(`${API_URL}/Profiles/${profileId}/media`, {
      headers: { 
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
    return response.json(); 
  },
  
  getAvatarUrl(avatarUrl) {
    if (!avatarUrl) return '/default-avatar.png';
    if (typeof avatarUrl === 'string' && (avatarUrl.startsWith('http') || avatarUrl.startsWith('data:'))) {
      return avatarUrl;
    }
    return '/default-avatar.png';
  },

  getAudioUrl(audioUrl) {
    if (!audioUrl) return null;
    if (typeof audioUrl === 'string' && (audioUrl.startsWith('http') || audioUrl.startsWith('data:'))) {
      return audioUrl;
    }
    return null;
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
      body: JSON.stringify({ 
        toProfileId: profileId,
        message: message
      })
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
    const response = await fetch(`${API_URL}/Favorites/${favoriteUserId}/is-favorite`, {
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
  },

  async connectVk(code, codeVerifier, deviceId, token) {
    const response = await fetch(`${API_URL}/Profiles/connect-vk`, {
      method: 'POST',
      headers: getAuthHeaders(token),
      body: JSON.stringify({ code, codeVerifier, deviceId }),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.error || errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Получить уведомления пользователя
  async getNotifications(token, page = 1, limit = 20) {
    const params = new URLSearchParams({
      page,
      limit
    });

    const response = await fetch(`${API_URL}/Notifications?${params}`, {
      method: 'GET',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Отметить уведомление как прочитанное
  async markNotificationAsRead(notificationId, token) {
    const response = await fetch(`${API_URL}/Notifications/${notificationId}/read`, {
      method: 'PATCH',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Отметить все уведомления как прочитанные
  async markAllNotificationsAsRead(token) {
    const response = await fetch(`${API_URL}/Notifications/mark-all-read`, {
      method: 'POST',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Получить количество непрочитанных уведомлений
  async getUnreadNotificationsCount(token) {
    const response = await fetch(`${API_URL}/Notifications/unread-count`, {
      method: 'GET',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Получить настройки уведомлений
  async getNotificationSettings(token) {
    const response = await fetch(`${API_URL}/Profiles/notification-settings`, {
      method: 'GET',
      headers: getAuthHeaders(token)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  },

  // Получить ленту мероприятий
  async getEvents(filterParams = {}) {
    const queryString = new URLSearchParams(filterParams).toString();
    const response = await fetch(`${API_URL}/Events?${queryString}`);
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Получить мероприятие по ID
  async getEventById(eventId, token) {
    const headers = token ? getAuthHeaders(token) : {};
    const response = await fetch(`${API_URL}/Events/${eventId}`, { headers });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Создать мероприятие
  async createEvent(eventData, token) {
    const response = await fetch(`${API_URL}/Events`, {
      method: 'POST',
      headers: getAuthHeaders(token),
      body: JSON.stringify(eventData),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Обновить мероприятие
  async updateEvent(eventId, eventData, token) {
    const response = await fetch(`${API_URL}/Events/${eventId}`, {
      method: 'PUT',
      headers: getAuthHeaders(token),
      body: JSON.stringify(eventData),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Отменить мероприятие (только для создателя)
  async cancelEvent(eventId, token) {
    const response = await fetch(`${API_URL}/Events/${eventId}`, {
      method: 'DELETE',
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Записаться на мероприятие
  async registerForEvent(eventId, token) {
    const response = await fetch(`${API_URL}/Events/${eventId}/register`, {
      method: 'POST',
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Отменить запись на мероприятие
  async unregisterFromEvent(eventId, token) {
    const response = await fetch(`${API_URL}/Events/${eventId}/register`, {
      method: 'DELETE',
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Получить мероприятия, созданные пользователем
  async getMyCreatedEvents(token, page = 1, limit = 20) {
    const response = await fetch(`${API_URL}/Events/my/created?page=${page}&limit=${limit}`, {
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Получить мероприятия, на которые записан пользователь
  async getMyRegisteredEvents(token, page = 1, limit = 20) {
    const response = await fetch(`${API_URL}/Events/my/registered?page=${page}&limit=${limit}`, {
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Загрузить изображение мероприятия
  async uploadEventImage(eventId, file, token) {
    const formData = new FormData();
    formData.append('image', file);

    const response = await fetch(`${API_URL}/Events/${eventId}/image`, {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData,
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  },

  // Получить список регионов
  async getRegions() {
    const response = await fetch(`${API_URL}/Metadata/regions`);
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'HTTP error!' }));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  }
};*/