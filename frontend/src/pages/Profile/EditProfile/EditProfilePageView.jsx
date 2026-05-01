import React from 'react';
import Header from '../../../components/Header/Header';
import MultiSelectDropdown from '../../../components/MultiSelectDropDown/MultiSelectDropDown';
import './EditProfilePage.css';

function EditProfilePageView({
  isCreating,
  error,
  saving,
  formData,
  userRole,
  userEmail,
  avatarPreview,
  lookingForChecked,
  desiredGenres,
  desiredSpecialties,
  activities,
  genres,
  cities,
  existingAudios,
  existingPhotos,
  existingVideos,
  audioFiles,
  photoFiles,
  videoFiles,
  audioTitles,
  onInputChange,
  onAvatarChange,
  onLookingForChange,
  onGenreToggle,
  onDesiredGenreChange,
  onDesiredSpecialtyChange,
  onAudioUpload,
  onPhotoUpload,
  onVideoUpload,
  onRemovePhotoFile,
  onRemoveAudioFile,
  onRemoveVideoFile,
  onRemoveExistingAudio,
  onRemoveExistingPhoto,
  onRemoveExistingVideo, 
  onNotifyByEmailChange,
  onSubmit,
  onCancel
}) {
  return (
    <>
      {!isCreating && <Header />}
      <div className="edit-profile-page">
        <div className="edit-profile-container">
          <h2>{isCreating ? 'Создать профиль' : 'Редактировать профиль'}</h2>
          
          {error && <div className="error-message">{error}</div>}
          
          <form onSubmit={onSubmit} className="profile-form">
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
                      onChange={onAvatarChange}
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
                    onChange={onInputChange}
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
                    onChange={onInputChange}
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
                    onChange={onInputChange}
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
                    onChange={onInputChange}
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
                    onChange={onInputChange}
                    placeholder="@username"
                    minLength={0}  // разрешаем пустое
                    maxLength={32}
                    pattern="(^$|@?[a-zA-Z0-9_]{5,32})"
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
                  onChange={(ids) => onInputChange({ target: { name: 'specialtyIds', value: ids } })}
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
                      onClick={() => onGenreToggle(genre.id)}
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
                  onChange={onInputChange}
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
                    onChange={(e) => onLookingForChange(e.target.checked)}
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
                      onChange={onDesiredGenreChange}
                      placeholder="Выберите жанры..."
                    />
                  </div>
                  
                  {userRole === 'Band' && (
                    <div className="form-group mb">
                      <label>Направления деятельности, которые ищем</label>
                      <MultiSelectDropdown
                        options={activities}
                        selectedIds={desiredSpecialties}
                        onChange={onDesiredSpecialtyChange}
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
                  onChange={onInputChange}
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
                      onChange={onPhotoUpload} 
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
                              onClick={() => onRemoveExistingPhoto(photo.id)}
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
                              onClick={() => onRemovePhotoFile(index)} 
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
                      onChange={onAudioUpload}
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
                              onClick={() => onRemoveExistingAudio(audio.id)}
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
                              onClick={() => onRemoveAudioFile(index)}
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
                      onChange={onVideoUpload} 
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
                              onClick={() => onRemoveExistingVideo(video.id)}
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
                              onClick={() => onRemoveVideoFile(index)}
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

            {/* Настройка уведомлений */}
            <div className="form-section">
              <h2>Настройка уведомлений</h2>
              <div className="form-group">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    checked={formData.notifyByEmail}
                    onChange={(e) => onNotifyByEmailChange(e.target.checked)}
                    className="checkbox-box"
                  />
                  <span className="checkbox-span">
                    Получать уведомления о новых предложениях и мероприятиях на email
                  </span>
                </label>
                <p className="notification-hint">
                  Уведомления будут приходить на адрес: {userEmail}
                </p>
              </div>
            </div>

            <div className="form-actions">
              {!isCreating && (
                <button
                  type="button"
                  onClick={onCancel}
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

export default EditProfilePageView;