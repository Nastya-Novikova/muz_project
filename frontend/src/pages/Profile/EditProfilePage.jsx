import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useFilters } from '../../context/useFilters';
import { api } from '../../services/api';
import Header from '../../components/Header/Header';
import MultiSelectDropdown from '../../components/MultiSelectDropDown/MultiSelectDropDown';
import './EditProfilePage.css';

function EditProfilePage() {
  const { getToken, getUserEmail, getUserRole } = useAuth();
  const navigate = useNavigate();
  const { activities, genres, cities } = useFilters();
  
  const [formData, setFormData] = useState({
    fullName: '',
    age: '',
    city: '',
    phone: '',
    telegram: '',
    experience: '',
    description: '',
    genreIds: [],
    specialtyIds: [],
    collaborationGoalIds: [],
    lookingFor: 'NotLooking',
    desiredGenreIds: [],
    desiredSpecialtyIds: []
  });

  const userRole = getUserRole() || 'Individual';
  
  const [lookingForChecked, setLookingForChecked] = useState(false);
  const [desiredGenres, setDesiredGenres] = useState([]);
  const [desiredSpecialties, setDesiredSpecialties] = useState([]);

  const [avatarFile, setAvatarFile] = useState(null);
  const [avatarPreview, setAvatarPreview] = useState('');
  
  const [audioFiles, setAudioFiles] = useState([]);
  const [audioTitles, setAudioTitles] = useState({});
  const [photoFiles, setPhotoFiles] = useState([]);
  const [videoFiles, setVideoFiles] = useState([]);

  const [existingAudios, setExistingAudios] = useState([]); 
  const [existingPhotos, setExistingPhotos] = useState([]);
  const [existingVideos, setExistingVideos] = useState([]);
  const [audiosToDelete, setAudiosToDelete] = useState([]); 

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [isCreating, setIsCreating] = useState(false);
  const [userEmail, setUserEmail] = useState('');

  useEffect(() => {
    const fetchProfile = async () => {
      setLoading(true);
      try {
        const token = getToken();
        const email = getUserEmail();
        setUserEmail(email);

        const profile = await api.getProfile(token);
        console.log('Получен профиль:', profile);
        
        if (profile) {
          setIsCreating(false);
          
          const media = await api.getMedia(profile.id, token).catch(() => ({}));
          if (media?.audio) {
            setExistingAudios(media.audio);
          }
          if (media?.photos) {
            setExistingPhotos(media.photos);
          }

          if (media?.video) {
            setExistingVideos(media.video);
          }
          
          if (profile.avatarUrl) {
            setAvatarPreview(api.getAvatarUrl(profile.avatarUrl));
          }
          
          setFormData({
            fullName: profile.fullName || '',
            age: profile.age || '',
            city: profile.city?.id?.toString() || profile.cityName || '',
            phone: profile.phone || '',
            telegram: profile.telegram || '',
            experience: profile.experience?.toString() || '',
            description: profile.description || '',
            genreIds: profile.genres?.map(g => g.id) || [],
            specialtyIds: profile.specialties?.map(s => s.id) || [],
            collaborationGoalIds: profile.collaborationGoals?.map(g => g.id) || [],
            lookingFor: profile.lookingFor || 'NotLooking',
            desiredGenreIds: profile.desiredGenres?.map(g => g.id) || [],
            desiredSpecialtyIds: profile.desiredSpecialties?.map(s => s.id) || []
          });

          setLookingForChecked(profile.lookingFor !== 'NotLooking');
          setDesiredGenres(profile.desiredGenres?.map(g => g.id) || []);
          setDesiredSpecialties(profile.desiredSpecialties?.map(s => s.id) || []);
        }
      } catch (error) {
        console.log('Профиль не найден, создаем новый');
        setIsCreating(true);
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [getToken, getUserEmail]);

  const handleLookingForChange = (checked) => {
    setLookingForChecked(checked);
  
    if (checked) {
      if (userRole === 'Individual') {
        setFormData(prev => ({ ...prev, lookingFor: 'LookingForBand' }));
      } else {
        setFormData(prev => ({ ...prev, lookingFor: 'LookingForMusician' }));
      }
    } else {
      setFormData(prev => ({ ...prev, lookingFor: 'NotLooking' }));
      setDesiredGenres([]);
      setDesiredSpecialties([]);
    }
  };
  
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleGenreToggle = (genreId) => {
    const newGenres = formData.genreIds.includes(genreId)
      ? formData.genreIds.filter(g => g !== genreId)
      : [...formData.genreIds, genreId];
    setFormData(prev => ({ ...prev, genreIds: newGenres }));
  };

  const handleDesiredGenreChange = (ids) => {
    setDesiredGenres(ids);
    setFormData(prev => ({ ...prev, desiredGenreIds: ids }));
  };

  const handleDesiredSpecialtyChange = (ids) => {
    setDesiredSpecialties(ids);
    setFormData(prev => ({ ...prev, desiredSpecialtyIds: ids }));
  };

  const handleAvatarChange = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      alert('Пожалуйста, выберите изображение');
      return;
    }

    if (file.size > 5 * 1024 * 1024) { 
      alert('Изображение слишком большое. Максимальный размер: 5MB');
      return;
    }

    setAvatarFile(file);
    const reader = new FileReader();
    reader.onloadend = () => {
      setAvatarPreview(reader.result);
    };
    reader.readAsDataURL(file);
  };

  const uploadAvatarToServer = async (token) => {
    if (!avatarFile) return false;
    try {
      const response = await api.uploadAvatar(avatarFile, token);
      if (response.avatarUrl) {
        setAvatarPreview(response.avatarUrl);
      }
      return true;
    } catch (error) {
      console.error('Ошибка загрузки аватара:', error);
      throw new Error(`Не удалось загрузить аватар: ${error.message}`);
    }
  };

  const handleAudioUpload = (e) => {
    const files = Array.from(e.target.files);
    
    const totalAfterAdd = existingAudios.length + audioFiles.length + files.length;
    if (totalAfterAdd > 5) {
      alert(`Можно загрузить не более 5 аудиозаписей. Уже есть: ${existingAudios.length + audioFiles.length}`);
      return;
    }
    
    const validFiles = files.filter(file => {
      const validTypes = ['audio/mpeg', 'audio/wav', 'audio/x-wav', 'audio/mp3'];
      const isValidType = validTypes.includes(file.type);
      const isValidSize = file.size <= 30 * 1024 * 1024;
      
      if (!isValidType) alert(`${file.name}: Допустимы только MP3, WAV файлы`);
      if (!isValidSize) alert(`${file.name}: Файл слишком большой. Максимум: 30MB`);
      
      return isValidType && isValidSize;
    });

    const newTitles = { ...audioTitles };
    validFiles.forEach(file => {
      const title = file.name.replace(/\.[^/.]+$/, "");
      newTitles[file.name] = title;
    });
    setAudioTitles(newTitles);
    setAudioFiles(prev => [...prev, ...validFiles]);
  };

  const handlePhotoUpload = (e) => {
    const files = Array.from(e.target.files);
  
    if (photoFiles.length + files.length > 5) {
      alert(`Можно загрузить не более 5 фотографий. Уже выбрано: ${photoFiles.length}`);
      return;
    }
    
    const validFiles = files.filter(file => {
      const isValidType = file.type.startsWith('image/');
      const isValidSize = file.size <= 5 * 1024 * 1024;
      
      if (!isValidType) alert(`${file.name}: Допустимы только изображения`);
      if (!isValidSize) alert(`${file.name}: Файл слишком большой. Максимум: 5MB`);
      
      return isValidType && isValidSize;
    });
    setPhotoFiles(prev => [...prev, ...validFiles]);
  };

  const handleVideoUpload = (e) => {
    const files = Array.from(e.target.files);
    
    if (videoFiles.length + files.length > 3) {
      alert(`Можно загрузить не более 3 видео. Уже выбрано: ${videoFiles.length}`);
      return;
    }
    
    const validFiles = files.filter(file => {
      const isValidType = file.type.startsWith('video/');
      const isValidSize = file.size <= 30 * 1024 * 1024;
      
      if (!isValidType) alert(`${file.name}: Допустимы только видеофайлы`);
      if (!isValidSize) alert(`${file.name}: Файл слишком большой. Максимум: 30MB`);
      
      return isValidType && isValidSize;
    });
    setVideoFiles(prev => [...prev, ...validFiles]);
  };

  const removePhotoFile = (index) => setPhotoFiles(prev => prev.filter((_, i) => i !== index));
  const removeAudioFile = (index) => setAudioFiles(prev => prev.filter((_, i) => i !== index));
  const removeVideoFile = (index) => setVideoFiles(prev => prev.filter((_, i) => i !== index));
  
  const removeExistingAudio = (audioId) => {
    setExistingAudios(prev => prev.filter(audio => audio.id !== audioId));
    setAudiosToDelete(prev => [...prev, audioId]);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    
    try {
      const token = getToken();
      const profileData = {
        profileType: userRole === 'Band' ? 'Band' : 'Individual',
        fullName: formData.fullName,
        age: formData.age ? parseInt(formData.age, 10) : null,
        description: formData.description,
        phone: formData.phone || null,
        telegram: formData.telegram || null,
        cityId: formData.city ? parseInt(formData.city, 10) : null,
        experience: formData.experience ? parseInt(formData.experience, 10) : 0,
        lookingFor: formData.lookingFor,
        genreIds: formData.genreIds.map(id => parseInt(id, 10)),
        specialtyIds: formData.specialtyIds.map(id => parseInt(id, 10)),
        collaborationGoalIds: formData.collaborationGoalIds.map(id => parseInt(id, 10)),
        desiredGenreIds: desiredGenres.map(id => parseInt(id, 10)),
        desiredSpecialtyIds: desiredSpecialties.map(id => parseInt(id, 10))
      };

      if (isCreating) {
        await api.createProfile(profileData, token);
      } else {
        await api.updateProfile(profileData, token);
      }

      if (avatarFile) {
        await uploadAvatarToServer(token);
      }
      
      for (const file of audioFiles) {
        const title = audioTitles[file.name] || file.name.replace(/\.[^/.]+$/, "");
        await api.uploadAudio(file, title, token, '');
      }

      for (const file of photoFiles) {
        const title = file.name.replace(/\.[^/.]+$/, "");
        await api.uploadPhoto(file, title, token, '');
      }

      for (const file of videoFiles) {
        const title = file.name.replace(/\.[^/.]+$/, "");
        await api.uploadVideo(file, title, token, '');
      }

      navigate('/profile');
      
    } catch (err) {
      setError(err.message || 'Не удалось сохранить профиль');
      console.error('Ошибка сохранения:', err);
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <>
        <Header />
        <div className="edit-profile-page">
          <div className="edit-profile-container">
            <p>Загрузка профиля...</p>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      {!isCreating && <Header />}
      <div className="edit-profile-page">
        <div className="edit-profile-container">
          <h2>{isCreating ? 'Создать профиль' : 'Редактировать профиль'}</h2>
          
          {error && <div className="error-message">{error}</div>}
          
          <form onSubmit={handleSubmit} className="profile-form">
            {/* Аватар */}
            <div className="form-section">
              <div className="avatar-upload">
                <div className="avatar-preview">
                  <img 
                    src={avatarPreview || '/default-avatar.png'} 
                    alt="Аватар" 
                    onError={(e) => {
                      e.target.src = '/default-avatar.png';
                    }}
                  />
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
                  <label>{userRole === 'Band' ? 'Название коллектива *' : 'ФИО *'}</label>
                  <input
                    type="text"
                    name="fullName"
                    autoComplete='off'
                    value={formData.fullName}
                    onChange={handleInputChange}
                    required
                    placeholder={userRole === 'Band' ? "Введите название коллектива" : "Введите ФИО"}
                    maxLength={100}
                  />
                </div>
                
                <div className="form-group">
                  <label>{userRole === 'Band' ? 'Год основания *' : 'Возраст *'}</label>
                  <input
                    type="number"
                    name="age"
                    value={formData.age}
                    onChange={handleInputChange}
                    required
                    min={userRole === 'Band' ? "1900" : "10"}
                    max={userRole === 'Band' ? "2026" : "100"}
                    placeholder={userRole === 'Band' ? "2010" : "25"}
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
                <label>{userRole === 'Band' ? "Состав коллектива" : "Вид деятельности"} *</label>
                <MultiSelectDropdown
                  label=""
                  options={activities}
                  selectedIds={formData.specialtyIds}
                  onChange={(ids) => setFormData(prev => ({ ...prev, specialtyIds: ids }))}
                  placeholder="Выберите виды деятельности..."
                  allText="Все виды"
                />
              </div>
              
              <div className="form-group mb">
                <label>Жанры</label>
                <div className="genre-tags">
                  {genres.map(genre => (
                    <button
                      key={genre.id}
                      type="button"
                      className={`genre-tag ${formData.genreIds.includes(genre.id) ? 'selected' : ''}`}
                      onClick={() => handleGenreToggle(genre.id)}
                    >
                      {genre.name}
                    </button>
                  ))}
                </div>
              </div>
              
              <div className="form-group">
                <label>Стаж (лет) *</label>
                <input
                  type="number"
                  name="experience"
                  value={formData.experience}
                  onChange={handleInputChange}
                  min="1"
                  placeholder="5"
                  required
                />
              </div>
            </div>

            {/* Поиск */}
            <div className="form-section">
              <h2>Поиск</h2>
              
              <div className="form-group mb">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    checked={lookingForChecked}
                    onChange={(e) => handleLookingForChange(e.target.checked)}
                    className="checkbox-box"
                  />
                  <span className="checkbox-span">
                    {userRole === 'Individual' 
                      ? 'Ищу коллектив' 
                      : 'Ищем музыкантов'}
                  </span>
                </label>
              </div>
              
              {lookingForChecked && (
                <>
                  <div className="form-group mb">
                    <label>
                      {userRole === 'Individual' 
                        ? 'Жанры коллектива' 
                        : 'Жанры, которые ищем'}
                    </label>
                    <MultiSelectDropdown
                      options={genres}
                      selectedIds={desiredGenres}
                      onChange={handleDesiredGenreChange}
                      placeholder="Выберите жанры..."
                    />
                  </div>
                  
                  {userRole === 'Band' && (
                    <div className="form-group mb">
                      <label>Направления деятельности, которые ищем</label>
                      <MultiSelectDropdown
                        options={activities}
                        selectedIds={desiredSpecialties}
                        onChange={handleDesiredSpecialtyChange}
                        placeholder="Выберите направления..."
                      />
                    </div>
                  )}
                </>
              )}
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
                  placeholder={userRole === 'Band' 
                    ? "Расскажите о коллективе, его стиле, достижениях..." 
                    : "Расскажите о себе, своих музыкальных предпочтениях, опыте..."}
                  maxLength={500}
                />
              </div>
            </div>

            {/* Портфолио */}
            <div className="form-section">
              <h2>Портфолио</h2>
              
              {/* Фото */}
              <div className="form-group mb">
                <label>Фотографии</label>
                <div className="file-upload-area">
                  <label className="upload-area">
                    <span>Загрузить фото (JPEG, PNG)</span>
                    <input
                      type="file"
                      accept="image/*"
                      multiple
                      onChange={handlePhotoUpload} 
                      className="file-input"
                    />
                  </label>
                  {existingPhotos.length > 0 && (
                    <div className="uploaded-files">
                      <div className="photos-preview-grid">
                        {existingPhotos.map((photo) => (
                          <div key={photo.id} className="photo-preview-item">
                            <img 
                              src={photo.fileUrl} 
                              alt={photo.title || 'Фото'} 
                              className="photo-preview"
                              onError={(e) => { e.target.src = '/default-image.png'; }}
                            />
                            <button
                              type="button"
                              onClick={() => removeExistingPhoto(photo.id)}
                              className="remove-file-btn"
                              title="Удалить фото"
                            >
                              ×
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                  
                  {/* Новые фото */}
                  {photoFiles.length > 0 && ( 
                    <div className="uploaded-files">
                      <div className="photos-preview-grid">
                        {photoFiles.map((file, index) => (
                          <div key={index} className="photo-preview-item">
                            <img src={URL.createObjectURL(file)} alt="preview" className="photo-preview" />
                            <button
                              type="button"
                              onClick={() => removePhotoFile(index)} 
                              className="remove-file-btn"
                            >
                              ×
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>

              {/* Аудио */}
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
                  
                  {(existingAudios.length > 0 || audioFiles.length > 0) && (
                    <div className="uploaded-files">
                      <div className="audio-list">
                        {existingAudios.map((audio) => (
                          <div key={audio.id} className="file-item existing">
                            <span>{audio.title || 'Аудиозапись'}</span>
                            <button
                              type="button"
                              onClick={() => removeExistingAudio(audio.id)}
                              className="remove-audio-btn"
                              title="Удалить аудио"
                            >
                              ×
                            </button>
                          </div>
                        ))}
                        
                        {audioFiles.map((file, index) => (
                          <div key={`new-${index}`} className="file-item new">
                            <span>{file.name}</span>
                            <button
                              type="button"
                              onClick={() => removeAudioFile(index)}
                              className="remove-audio-btn"
                            >
                              ×
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>

              {/* Видео */}
              <div className="form-group mb">
                <label>Видеозаписи</label>
                <div className="file-upload-area">
                  <label className="upload-area">
                    <span>Загрузить видео (MP4, AVI)</span>
                    <input
                      type="file"
                      accept="video/*"
                      multiple
                      onChange={handleVideoUpload} 
                      className="file-input"
                    />
                  </label>
                  {existingVideos.length > 0 && (
                    <div className="uploaded-files">
                      <div className="videos-preview-grid">
                        {existingVideos.map((video) => (
                          <div key={video.id} className="video-preview-item">
                            <video 
                              src={video.fileUrl} 
                              controls 
                              className="video-preview"
                              style={{ width: '100%', maxHeight: '150px' }}
                            />
                            <button
                              type="button"
                              onClick={() => removeExistingVideo(video.id)}
                              className="remove-file-btn"
                              title="Удалить видео"
                            >
                              ×
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                  
                  {/* Новые видео */}
                  {videoFiles.length > 0 && ( 
                    <div className="uploaded-files">
                      <div className="videos-preview-grid">
                        {videoFiles.map((file, index) => (
                          <div key={index} className="video-preview-item">
                            <video src={URL.createObjectURL(file)} controls className="video-preview" />
                            <button
                              type="button"
                              onClick={() => removeVideoFile(index)}
                              className="remove-file-btn"
                            >
                              ×
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>
            </div>

            <div className="form-actions">
              {!isCreating && (
                <button
                  type="button"
                  onClick={() => navigate('/profile')}
                  className="cancel-btn"
                >
                  Отмена
                </button>
              )}
              <button
                type="submit"
                disabled={saving}
                className="submit-btn"
              >
                {saving ? 'Сохранение...' : 'Сохранить'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  );
}

export default EditProfilePage;