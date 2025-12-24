// Упрощенная версия useFilters.js
import { useState, useEffect } from 'react';
import { api } from '../services/api';

export function useFilters() {
  const [filters, setFilters] = useState({
    cities: [],
    activities: [],
    genres: [],
  });

  useEffect(() => {
    const loadData = async () => {
      try {
        const activities = await api.getActivities();
        const cities = await api.getCities();
        const genres = await api.getGenres();

        // Извлекаем specialties из activities если они там есть
        const activityList = activities?.specialties || activities || [];
        
        setFilters({
          cities: cities || [],
          activities: activityList.map(item => ({
            id: item.id,
            name: item.localizedName || item.name
          })),
          genres: (genres || []).map(item => ({
            id: item.id,
            name: item.localizedName || item.name
          })),
        });
      } catch (error) {
        console.log('Ошибка:', error);
        setFilters(prev => ({ ...prev}));
      }
    };

    loadData();
  }, []);

  return filters;
}