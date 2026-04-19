import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../../../context/AuthContext';
import { useFilters } from '../../../context/useFilters';
import { api } from '../../../services/api';
import Header from '../../../components/Header/Header';
import './EventFormPage.css';

function EventFormPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const { getToken } = useAuth();
  const { cities, regions } = useFilters();
  const isEditMode = !!eventId;

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState('');
  
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    regionId: '',
    cityId: '',
    address: '',
    startDateTime: '',
    endDateTime: '',
    maxParticipants: 10,
  });

  useEffect(() => {
    if (isEditMode) {
      const loadEvent = async () => {
        setLoading(true);
        try {
          const token = getToken();
          const event = await api.getEventById(eventId, token);
          
          setFormData({
            title: event.title || '',
            description: event.description || '',
            regionId: event.region?.id || '',
            cityId: event.city?.id || '',
            address: event.address || '',
            startDateTime: event.startDateTime ? event.startDateTime.slice(0, 16) : '',
            endDateTime: event.endDateTime ? event.endDateTime.slice(0, 16) : '',
            maxParticipants: (event.maxParticipants || 10),
          });
          
          if (event.imageUrl) {
            setImagePreview(event.imageUrl);
          }
        } catch (err) {
          console.error('Ошибка загрузки мероприятия:', err);
          setError('Не удалось загрузить данные мероприятия.');
        } finally {
          setLoading(false);
        }
      };
      loadEvent();
    }
  }, [eventId, isEditMode, getToken]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => setImagePreview(reader.result);
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');

    try {
      const token = getToken();
      
      if (!formData.maxParticipants) {
        setError('Укажите максимальное количество участников');
        setSaving(false);
        return;
      }

      const payload = {
        ...formData,
        regionId: parseInt(formData.regionId, 10),
        cityId: parseInt(formData.cityId, 10),
        maxParticipants: parseInt(formData.maxParticipants, 10),
        startDateTime: new Date(formData.startDateTime).toISOString(),
        endDateTime: formData.endDateTime ? new Date(formData.endDateTime).toISOString() : null,
      };

      let savedEvent;
      if (isEditMode) {
        savedEvent = await api.updateEvent(eventId, payload, token);
      } else {
        savedEvent = await api.createEvent(payload, token);
      }

      if (imageFile) {
        await api.uploadEventImage(savedEvent.id, imageFile, token);
      }

      navigate('/events');
    } catch (err) {
      console.error('Ошибка сохранения:', err);
      setError(err.message || 'Не удалось сохранить мероприятие.');
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    navigate('/events');
  };

  if (loading) {
    return (
      <>
        <Header />
        <div className="event-form-page">
          <div className="form-container">
            <p>Загрузка...</p>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <Header />
      <div className="event-form-page">
        <div className="form-container">
          <h2>{isEditMode ? 'Редактировать мероприятие' : 'Создать мероприятие'}</h2>
          
          {error && <div className="error-message">{error}</div>}

          <form onSubmit={handleSubmit} className="event-form">
            {/* Изображение */}
            <div className="form-section">
              <div className="image-upload">
                <div className="image-preview-container" onClick={() => document.getElementById('event-image').click()}>
                  {imagePreview ? (
                    <img src={imagePreview} alt="Preview" className="image-preview" />
                  ) : (
                    <div className="upload-placeholder">
                      <span>Загрузить изображение</span>
                    </div>
                  )}
                  <label className="upload-btn">Изменить</label>
                  <input
                    id="event-image"
                    type="file"
                    accept="image/*"
                    onChange={handleImageChange}
                    className="file-input"
                  />
                </div>
              </div>
            </div>

            {/* Основная информация */}
            <div className="form-section">
              <h3>Основная информация</h3>
              
              <div className="form-grid">
                <div className="form-group full-width">
                  <label>Название *</label>
                  <input
                    type="text"
                    name="title"
                    value={formData.title}
                    onChange={handleChange}
                    required
                    maxLength={200}
                    placeholder="Введите название мероприятия"
                  />
                </div>

                <div className="form-group full-width">
                  <label>Описание</label>
                  <textarea
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    rows="4"
                    placeholder="Опишите мероприятие"
                    maxLength={500}
                  />
                </div>
              </div>
            </div>

            {/* Местоположение */}
            <div className="form-section">
              <h3>Местоположение</h3>
              
              <div className="form-grid">
                <div className="form-group">
                  <label>Регион *</label>
                  <select
                    name="regionId"
                    value={formData.regionId}
                    onChange={handleChange}
                    required
                  >
                    <option value="">Выберите регион</option>
                    {regions.map(region => (
                      <option key={region.id} value={region.id}>{region.name}</option>
                    ))}
                  </select>
                </div>

                <div className="form-group">
                  <label>Город *</label>
                  <select
                    name="cityId"
                    value={formData.cityId}
                    onChange={handleChange}
                    required
                  >
                    <option value="">Выберите город</option>
                    {cities.map(city => (
                      <option key={city.id} value={city.id}>{city.name}</option>
                    ))}
                  </select>
                </div>

                <div className="form-group full-width">
                  <label>Адрес *</label>
                  <input
                    type="text"
                    name="address"
                    value={formData.address}
                    onChange={handleChange}
                    required
                    maxLength={200}
                    placeholder="Улица, дом"
                  />
                </div>
              </div>
            </div>

            {/* Дата и время */}
            <div className="form-section">
              <h3>Дата и время</h3>
              
              <div className="form-grid">
                <div className="form-group">
                  <label>Начало *</label>
                  <input
                    type="datetime-local"
                    name="startDateTime"
                    value={formData.startDateTime}
                    onChange={handleChange}
                    required
                  />
                </div>

                <div className="form-group">
                  <label>Окончание</label>
                  <input
                    type="datetime-local"
                    name="endDateTime"
                    value={formData.endDateTime}
                    onChange={handleChange}
                  />
                </div>
              </div>
            </div>

            {/* Участники */}
            <div className="form-section">
              <h3>Участники</h3>
                <div className="form-group">
                  <label>Максимальное количество участников *</label>
                  <input
                    type="number"
                    name="maxParticipants"
                    value={formData.maxParticipants}
                    onChange={handleChange}
                    min="1"
                    max="1000"
                    placeholder="10"
                    required
                  />
                </div>
            </div>

            <div className="form-actions">
              <button type="button" onClick={handleCancel} className="cancel-btn" disabled={saving}>
                Отмена
              </button>
              <button type="submit" className="submit-btn" disabled={saving}>
                {saving ? 'Сохранение...' : (isEditMode ? 'Сохранить' : 'Создать')}
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
}

export default EventFormPage;