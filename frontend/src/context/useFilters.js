import { useState, useEffect } from 'react';
import { api } from '../services/api';

export function useFilters() {
  const [filters, setFilters] = useState({
    cities: [],
    activities: [],
    genres: [],
    loading: true
  });

  useEffect(() => {
    loadFilters();
  }, []);

  const loadFilters = async () => {
    try {
      const [cities, activities, genres] = await Promise.all([
        api.getCities(),
        api.getActivities(),
        api.getGenres()
      ]);

      setFilters({
        cities,
        activities,
        genres,
        loading: false
      });
    } catch (error) {
      console.log('Ошибка');
      setFilters({
        cities: [],
        activities: [],
        genres: [],
        loading: false
      });
    }
  };

  return filters;
}