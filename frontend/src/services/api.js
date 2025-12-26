const API_URL = 'http://localhost:7000/api';

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
  }
};