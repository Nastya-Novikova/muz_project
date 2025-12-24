import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Header from '../../components/Header/Header';
import './EditProfilePage.css';

function EditProfilePage() {
  const { user, updateProfile } = useAuth();
  const navigate = useNavigate();
  
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
      other: ''
    }
  });

  const [avatarFile, setAvatarFile] = useState(null);
  const [avatarPreview, setAvatarPreview] = useState(user?.avatar || '');
  const [audioFiles, setAudioFiles] = useState([]);
  const [photoFiles, setPhotoFiles] = useState([]);

  useEffect(() => {
    if (user) {
      setFormData({
        fullName: user.fullName || '',
        age: user.age || '',
        activityType: user.activityType || '',
        city: user.city || '',
        contact: user.contact || user.email || '',
        phone: user.phone || '',
        telegram: user.telegram || '',
        genres: user.genres || [],
        experience: user.experience || '',
        description: user.description || '',
        portfolio: user.portfolio || { audio: [], photos: [], other: '' }
      });
      setAvatarPreview(user.avatar);
    }
  }, [user]);

  const activityOptions = [
    'Вокал', 'Гитара', 'Бас-гитара', 'Ударные', 
    'Клавишные', 'Скрипка', 'Виолончель', 'Флейта',
    'Саксофон', 'Труба', 'Композитор', 'Аранжировщик'
  ];

  const genreOptions = [
    'Рок', 'Джаз', 'Поп', 'Хип-хоп', 'Блюз', 'Классика',
    'Метал', 'Кантри', 'Электроника', 'R&B'
  ];

  const cityOptions = [
    'Москва',
    'Санкт-Петербург',
    'Новосибирск',
    'Екатеринбург',
    'Казань',
    'Нижний Новгород',
  ];

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleGenreToggle = (genre) => {
    setFormData(prev => ({
      ...prev,
      genres: prev.genres.includes(genre)
        ? prev.genres.filter(g => g !== genre)
        : [...prev.genres, genre]
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

  const handleSubmit = (e) => {
    e.preventDefault();
    
    const updatedUser = {
      ...user,
      ...formData,
      avatar: avatarPreview,
      profileCompleted: true
    };

    const allUsers = JSON.parse(localStorage.getItem('musicianFinder_users') || '{}');
    allUsers[user.email] = updatedUser;
    localStorage.setItem('musicianFinder_users', JSON.stringify(allUsers));

    updateProfile(updatedUser);
    navigate('/profile');
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
                    {cityOptions.map(city => (
                      <option key={city} value={city}>{city}</option>
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
                  {activityOptions.map(activity => (
                    <option key={activity} value={activity}>{activity}</option>
                  ))}
                </select>
              </div>
              
              <div className="form-group mb">
                <label>Жанры</label>
                <div className="genre-tags">
                  {genreOptions.map(genre => (
                    <button
                      key={genre}
                      type="button"
                      className={`genre-tag ${formData.genres.includes(genre) ? 'selected' : ''}`}
                      onClick={() => handleGenreToggle(genre)}
                    >
                      {genre}
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
              
              <div className="form-group">
                <label>Дополнительная информация</label>
                <textarea
                  name="portfolio.other"
                  value={formData.portfolio.other}
                  onChange={(e) => setFormData(prev => ({
                    ...prev,
                    portfolio: { ...prev.portfolio, other: e.target.value }
                  }))}
                  rows="3"
                  placeholder="Ссылки на соцсети, дополнительные проекты..."
                />
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