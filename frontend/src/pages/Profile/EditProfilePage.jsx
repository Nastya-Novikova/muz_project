import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useFilters } from '../../context/useFilters';
import { api } from '../../services/api';
import Header from '../../components/Header/Header';
import './EditProfilePage.css';

function EditProfilePage() {
  const { user, updateProfile: updateAuthContext, getToken } = useAuth();
  const navigate = useNavigate();
  const { activities, genres, cities } = useFilters();
  
  const [formData, setFormData] = useState({
    fullName: '',
    age: '',
    activityType: '',
    city: '',
    contact: '',
    phone: '',
    telegram: '',
    genres: [],
    experience: '',
    description: '',
    portfolio: {
      audio: [],
      photos: [],
    }
  });

  const [avatarFile, setAvatarFile] = useState(null);
  const [avatarPreview, setAvatarPreview] = useState(user?.avatar || '');
  const [audioFiles, setAudioFiles] = useState([]);
  const [photoFiles, setPhotoFiles] = useState([]);
  const [loading, setLoading] = useState(false); // Состояние загрузки
  const [error, setError] = useState('');

  useEffect(() => {
    if (user && activities.length > 0 && genres.length > 0 && cities.length > 0) { // Убедитесь, что списки загружены
      // Найти id по label
      const activityId = activities.find(a => a.label === user.activityType)?.id || '';
      const parsedActivityId = activityId !== undefined ? Number(activityId) : null;
      const cityId = cities.find(c => c.label === user.city)?.id || '';
      const parsedCityId = cityId !== undefined ? Number(cityId) : null;
      const genreIds = Array.isArray(user.genres) ? user.genres
        .map(genreLabel => genres.find(g => g.label === genreLabel)?.id)
        .filter(id => id !== undefined) : [];// Убираем null, если были несовпадения

      setFormData({
        fullName: user.fullName || '',
        age: user.age || '',
        activityType: parsedActivityId, // Сохраняем id
        city: parsedCityId,           // Сохраняем id
        contact: user.contact || user.email || '',
        phone: user.phone || '',
        telegram: user.telegram || '',
        genres: genreIds,       // Сохраняем массив id
        experience: user.experience || '',
        description: user.description || '',
        portfolio: user.portfolio || { audio: [], photos: []}
      });
      setAvatarPreview(user.avatar);
    }
  }, [user, activities, genres, cities]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleGenreToggle = (genreId) => {
    setFormData(prev => ({
      ...prev,
      genres: prev.genres.includes(genreId)
        ? prev.genres.filter(g => g !== genreId)
        : [...prev.genres, genreId]
    }));
  };

  const handleAvatarChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setAvatarFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setAvatarPreview(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleAudioUpload = (e) => {
    const files = Array.from(e.target.files);
    setAudioFiles(prev => [...prev, ...files]);
  };

  const handlePhotoUpload = (e) => {
    const files = Array.from(e.target.files);
    setPhotoFiles(prev => [...prev, ...files]);
  };

  const removeAudio = (index) => {
    setAudioFiles(prev => prev.filter((_, i) => i !== index));
  };

  const removePhoto = (index) => {
    setPhotoFiles(prev => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    
    try {
      const token = getToken();

      const cityId= formData.city !== null && formData.city !== '' ? Number(formData.city) : null;
      // specialtyIds - массив, даже если один элемент
      const specialtyIds = formData.activityType !== null && formData.activityType !== '' ? [Number(formData.activityType)] : [];
      // 1. Подготовка данных для профиля (без файлов)
      const profileData = {
        fullName: formData.fullName,
        age: parseInt(formData.age, 10) || null,
        description: formData.description,
        phone: formData.phone,
        telegram: formData.telegram || null,
        experience: parseInt(formData.experience, 10) || null,
        cityId: cityId, // Уже id
        specialtyIds: specialtyIds, // Массив id
        genreIds: formData.genres, // Уже массив id
      };

      // 2. Вызов ручки создания/обновления профиля (всегда первым)
      let profileResponse;
      if (!user.profileCreated) { // profileCreated === false
        profileResponse = await api.createProfile(profileData, token);
      } else { // profileCreated === true
        profileResponse = await api.updateProfile(profileData, token);
      }

      // 3. Загрузка аватарки (если выбрана)
      if (avatarFile) {
        await api.uploadAvatar(avatarFile, token);
      }

      // 4. Загрузка аудио (если есть)
      if (audioFiles.length > 0) {
        for (const file of audioFiles) {
          await api.uploadAudio(file, file.name, token); // Можно улучшить: передавать реальное имя или title из интерфейса
        }
      }

      // 5. Загрузка фото (если есть) - не работает
      /*if (photoFiles.length > 0) {
      for (const file of photoFiles) {
        await api.uploadPhoto(file, file.name, token);
      }
    }*/
      // получение обновленного профиля
      //const updatedProfile = await api.getProfile(token);

      // 6. Обновление состояния в AuthContext
      // Нужно получить обновлённый профиль с бэкенда, чтобы получить новые данные (например, id профиля, обновлённые списки и т.д.)
      // const updatedProfileFromServer = await api.getProfile(); // Если бэкенд возвращает обновлённый профиль после создания/обновления
      // updateAuthContext(updatedProfileFromServer, user.token); // Передаём обновлённый профиль и токен

      // Пока обновляем локально, так как бэкенд возвращает обновлённый объект после create/update
      //updateAuthContext(updatedProfile, user.token); 

      // 7. Переход
      navigate('/profile');
    } catch (err) {
      setError(err.message || 'Не удалось сохранить профиль');
    } finally {
      setLoading(false);
    }
  };

  if (!user) {
    return (
      <>
        <Header />
        <div className="edit-profile-page">
          <div className="edit-profile-container">
            <p>Пожалуйста, войдите в систему</p>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <Header />
      <div className="edit-profile-page">
        <div className="edit-profile-container">
          <h2>Редактировать профиль</h2>
          
          <form onSubmit={handleSubmit} className="profile-form">
            {/* Аватар */}
            <div className="form-section">
              <div className="avatar-upload">
                <div className="avatar-preview">
                    <img src={avatarPreview} alt="Аватар" />
                  <label className="upload-btn">
                    <input
                      type="file"
                      accept="image/*"
                      onChange={handleAvatarChange}
                      className="file-input"
                    />
                    Изменить
                  </label>
                </div>
              </div>
            </div>

            {/* Личные данные */}
            <div className="form-section">
              <h2>Личные данные</h2>
              <div className="form-grid">
                <div className="form-group">
                  <label>ФИО или название коллектива *</label>
                  <input
                    type="text"
                    name="fullName"
                    autoComplete='off'
                    value={formData.fullName}
                    onChange={handleInputChange}
                    required
                    placeholder="Введите ФИО"
                  />
                </div>
                
                <div className="form-group">
                  <label>Возраст *</label>
                  <input
                    type="number"
                    name="age"
                    value={formData.age}
                    onChange={handleInputChange}
                    required
                    min="10"
                    max="100"
                    placeholder="25"
                  />
                </div>

                <div className="form-group">
                  <label>Город *</label>
                  <select
                    name="city"
                    value={formData.city}
                    onChange={handleInputChange}
                    required
                    className="city-select"
                  >
                    <option value="">Выберите город</option>
                    {cities.map(city => (
                      <option key={city.id} value={city.id}>
                        {city.name}
                      </option>
                    ))}
                  </select>
                </div>
                
                <div className="form-group">
                  <label>Почта</label>
                  <input
                    type="email"
                    value={user.email}
                    disabled
                    className="disabled-input"
                  />
                </div>
                
                <div className="form-group">
                  <label>Телефон</label>
                  <input
                    type="tel"
                    name="phone"
                    autoComplete='off'
                    value={formData.phone}
                    onChange={handleInputChange}
                    placeholder="+7 (999) 123-45-67"
                  />
                </div>
                
                <div className="form-group">
                  <label>Telegram</label>
                  <input
                    type="text"
                    name="telegram"
                    value={formData.telegram}
                    onChange={handleInputChange}
                    placeholder="@username"
                  />
                </div>
              </div>
            </div>

            {/* Деятельность */}
            <div className="form-section">
              <h2>Деятельность</h2>
              
              <div className="form-group mb">
                <label>Вид деятельности *</label>
                <select
                  name="activityType"
                  value={formData.activityType}
                  onChange={handleInputChange}
                  required
                >
                  <option value="">Выберите вид деятельности</option>
                  {activities.map(activity => (
                    <option key={activity.id} value={activity.id}>
                      {activity.name}
                    </option>
                  ))}
                </select>
              </div>
              
              <div className="form-group mb">
                <label>Жанры</label>
                <div className="genre-tags">
                  {genres.map(genre => (
                    <button
                      key={genre.id}
                      type="button"
                      className={`genre-tag ${formData.genres.includes(genre.id) ? 'selected' : ''}`}
                      onClick={() => handleGenreToggle(genre.id)}
                    >
                      {genre.name}
                    </button>
                  ))}
                </div>
              </div>
              
              <div className="form-group">
                <label>Стаж (лет)</label>
                <input
                  type="number"
                  name="experience"
                  value={formData.experience}
                  onChange={handleInputChange}
                  min="0"
                  placeholder="5"
                />
              </div>
            </div>

            {/* О себе */}
            <div className="form-section">
              <h2>О себе</h2>
              <div className="form-group">
                <label>Описание</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  rows="4"
                  placeholder="Расскажите о себе, своих музыкальных предпочтениях, опыте..."
                />
              </div>
            </div>

            {/* Портфолио */}
            <div className="form-section">
              <h2>Портфолио</h2>
              
              <div className="form-group mb">
                <label>Аудиозаписи</label>
                <div className="file-upload-area">
                  <label className="upload-area">
                    <span>Загрузить аудио (MP3, WAV)</span>
                    <input
                      type="file"
                      accept="audio/*"
                      multiple
                      onChange={handleAudioUpload}
                      className="file-input"
                    />
                  </label>
                  {audioFiles.length > 0 && (
                    <div className="uploaded-files">
                      {audioFiles.map((file, index) => (
                        <div key={index} className="file-item">
                          <span>{file.name}</span>
                          <button
                            type="button"
                            onClick={() => removeAudio(index)}
                            className="remove-btn"
                          >
                            ✕
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
              
              <div className="form-group mb">
                <label>Фото (сертификаты, награды)</label>
                <div className="file-upload-area">
                  <label className="upload-area">
                    <span>Загрузить фото (JPG, PNG)</span>
                    <input
                      type="file"
                      accept="image/*"
                      multiple
                      onChange={handlePhotoUpload}
                      className="file-input"
                    />
                  </label>
                  {photoFiles.length > 0 && (
                    <div className="uploaded-files">
                      {photoFiles.map((file, index) => (
                        <div key={index} className="file-item">
                          <span>{file.name}</span>
                          <button
                            type="button"
                            onClick={() => removePhoto(index)}
                            className="remove-btn"
                          >
                            ✕
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            </div>

            <div className="form-actions">
              <button
                type="button"
                onClick={() => navigate('/profile')}
                className="cancel-btn"
              >
                Отмена
              </button>
              <button
                type="submit"
                className="submit-btn"
              >
                Сохранить
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
}

export default EditProfilePage;