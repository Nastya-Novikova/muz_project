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
  }
};