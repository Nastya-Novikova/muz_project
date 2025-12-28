import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext'; // Только для токена
import { useFilters } from '../../context/useFilters';
import { api } from '../../services/api';
import Header from '../../components/Header/Header';
import './EditProfilePage.css';

function EditProfilePage() {
  const { getToken, getUserEmail } = useAuth(); // Только токен
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
    description: ''
  });

  const [avatarFile, setAvatarFile] = useState(null);
  const [avatarPreview, setAvatarPreview] = useState('');
  const [audioFiles, setAudioFiles] = useState([]);
  const [photoFiles, setPhotoFiles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [isCreating, setIsCreating] = useState(true); // Создаём или редактируем?
  const [userEmail, setUserEmail] = useState('');

  useEffect(() => {
    const loadExistingProfile = async () => {
      try {
        const token = getToken();
        const email = getUserEmail();
        setUserEmail(email);
        // Пробуем загрузить существующий профиль
        const existingProfile = await api.getProfile(token);
        
        if (existingProfile) {
          setIsCreating(false); // Режим редактирования
          
          // Преобразуем данные с сервера в форму
          setFormData({
            fullName: existingProfile.fullName || '',
            age: existingProfile.age || '',
            activityType: existingProfile.specialties?.[0]?.id || '',
            city: existingProfile.city?.id || '',
            contact: email || '',
            phone: existingProfile.phone || '',
            telegram: existingProfile.telegram || '',
            genres: existingProfile.genres?.map(g => g.id) || [],
            experience: existingProfile.experience || '',
            description: existingProfile.description || ''
          });
          
          // Устанавливаем аватар если есть - аватар не содержится в возвращаемом объекте?
          /*if (existingProfile.avatarUrl) {
            setAvatarPreview(existingProfile.avatarUrl);
          }*/
        } else {
          setFormData(prev => ({
            ...prev,
            contact: email || ''
          }));
        }
      } catch (error) {
        // Профиля нет - остаёмся в режиме создания
        console.log('Профиль не найден, создаём новый');
        setIsCreating(true);

        const email = getUserEmail();
        setUserEmail(email || '');
        setFormData(prev => ({
          ...prev,
          contact: email || ''
        }));
      }
    };

    loadExistingProfile();
  }, [getToken, getUserEmail, navigate]);

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
      
      // Подготовка данных для отправки на сервер
      const profileData = {
        fullName: formData.fullName,
        age: formData.age ? parseInt(formData.age, 10) : null,
        description: formData.description,
        phone: formData.phone || null,
        telegram: formData.telegram || null,
        experience: formData.experience ? parseInt(formData.experience, 10) : null,
        cityId: formData.city ? parseInt(formData.city, 10) : null,
        specialtyIds: formData.activityType ? [parseInt(formData.activityType, 10)] : [],
        genreIds: formData.genres.map(id => parseInt(id, 10))
      };

      // Сохраняем профиль
      let profileResponse;
      if (isCreating) {
        profileResponse = await api.createProfile(profileData, token);
      } else {
        profileResponse = await api.updateProfile(profileData, token);
      }

      // Загружаем файлы если есть
      /*if (avatarFile) {
        await api.uploadAvatar(avatarFile, token);
      }*/
      
      if (audioFiles.length > 0) {
        for (const file of audioFiles) {
          await api.uploadAudio(file, file.name, token);
        }
      }
      
      if (photoFiles.length > 0) {
        for (const file of photoFiles) {
          await api.uploadPhoto(file, file.name, token);
        }
      }

      // Всё успешно - переходим на страницу профиля
      navigate('/profile');
      
    } catch (err) {
      setError(err.message || 'Не удалось сохранить профиль');
      console.error('Ошибка сохранения:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <>
        <Header />
        <div className="edit-profile-page">
          <div className="edit-profile-container">
            <p>Загрузка...</p>
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
          <h2>{isCreating ? 'Создать профиль' : 'Редактировать профиль'}</h2>
          
          {error && <div className="error-message">{error}</div>}
          
          <form onSubmit={handleSubmit} className="profile-form">
            {/* Аватар */}
            <div className="form-section">
              <div className="avatar-upload">
                <div className="avatar-preview">
                  <img src={avatarPreview || '/default-avatar.png'} alt="Аватар" />
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
                    maxLength={100}
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
                    value={userEmail}
                    disabled
                    className="disabled-input"
                  />
                </div>
                
                <div className="form-group">
                  <label>Телефон *</label>
                  <input
                    type="tel"
                    name="phone"
                    autoComplete='off'
                    value={formData.phone}
                    onChange={handleInputChange}
                    required
                    placeholder="+79991234567"
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
                    maxLength={50}
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
                  maxLength={90}
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
                disabled={loading}
                className="submit-btn"
              >
                {loading ? 'Сохранение...' : 'Сохранить'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
}

export default EditProfilePage;