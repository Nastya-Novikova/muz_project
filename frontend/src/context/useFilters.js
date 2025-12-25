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

        const activityList = Array.isArray(activities) ? activities : (activities?.specialties || []);
        const genreList = Array.isArray(genres) ? genres : (genres?.genres || []);
        const cityList = Array.isArray(cities) ? cities : (cities?.cities || []);

        setFilters({
          cities: cityList.map(item => ({
            id: item.id,
            name: item.localizedName || item.name
          })),
          activities: activityList.map(item => ({
            id: item.id,
            name: item.localizedName || item.name
          })),
          genres: genreList.map(item => ({
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