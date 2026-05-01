import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Header from '../../../components/Header/Header';
import { useAuth } from '../../../context/AuthContext';
import { useFilters } from '../../../context/useFilters';
import { api } from '../../../services/api';
import EditProfilePageView from './EditProfilePageView';

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
    desiredSpecialtyIds: [],
    notifyByEmail: true
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
  const [photosToDelete, setPhotosToDelete] = useState([]);
  const [videosToDelete, setVideosToDelete] = useState([]); 

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
            desiredSpecialtyIds: profile.desiredSpecialties?.map(s => s.id) || [],
            notifyByEmail: profile.notifyByEmail ?? true
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

  const removeExistingPhoto = (photoId) => {
    setExistingPhotos(prev => prev.filter(photo => photo.id !== photoId));
    setPhotosToDelete(prev => [...prev, photoId]);
  };

  const removeExistingVideo = (videoId) => {
    setExistingVideos(prev => prev.filter(video => video.id !== videoId));
    setVideosToDelete(prev => [...prev, videoId]);
  };

  const handleNotifyByEmailChange = (checked) => {
    setFormData(prev => ({
      ...prev,
      notifyByEmail: checked
    }));
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
        desiredSpecialtyIds: desiredSpecialties.map(id => parseInt(id, 10)),
        notifyByEmail: formData.notifyByEmail
      };

      if (isCreating) {
        await api.createProfile(profileData, token);
      } else {
        await api.updateProfile(profileData, token);
      }

      if (!isCreating) {

        for (const audioId of audiosToDelete) {
          try {
              await api.deleteMedia(audioId, token);
              console.log(`Аудио ${audioId} удалено`);
          } catch (err) {
              console.error(`Ошибка удаления аудио ${audioId}:`, err);
          }
        }
        
        for (const photoId of photosToDelete) {
          try {
              await api.deleteMedia(photoId, token);
              console.log(`Фото ${photoId} удалено`);
          } catch (err) {
              console.error(`Ошибка удаления фото ${photoId}:`, err);
          }
        }

        for (const videoId of videosToDelete) {
          try {
              await api.deleteMedia(videoId, token);
              console.log(`Видео ${videoId} удалено`);
          } catch (err) {
              console.error(`Ошибка удаления видео ${videoId}:`, err);
          }
        }
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
    <EditProfilePageView
      isCreating={isCreating}
      error={error}
      saving={saving}
      formData={formData}
      userRole={userRole}
      userEmail={userEmail}
      avatarPreview={avatarPreview}
      lookingForChecked={lookingForChecked}
      desiredGenres={desiredGenres}
      desiredSpecialties={desiredSpecialties}
      activities={activities}
      genres={genres}
      cities={cities}
      existingAudios={existingAudios}
      existingPhotos={existingPhotos}
      existingVideos={existingVideos}
      audioFiles={audioFiles}
      photoFiles={photoFiles}
      videoFiles={videoFiles}
      audioTitles={audioTitles}
      onInputChange={handleInputChange}
      onAvatarChange={handleAvatarChange}
      onLookingForChange={handleLookingForChange}
      onGenreToggle={handleGenreToggle}
      onDesiredGenreChange={handleDesiredGenreChange}
      onDesiredSpecialtyChange={handleDesiredSpecialtyChange}
      onAudioUpload={handleAudioUpload}
      onPhotoUpload={handlePhotoUpload}
      onVideoUpload={handleVideoUpload}
      onRemovePhotoFile={removePhotoFile}
      onRemoveAudioFile={removeAudioFile}
      onRemoveVideoFile={removeVideoFile}
      onRemoveExistingAudio={removeExistingAudio}
      onRemoveExistingPhoto={removeExistingPhoto}
      onRemoveExistingVideo={removeExistingVideo} 
      onSubmit={handleSubmit}
      onNotifyByEmailChange={handleNotifyByEmailChange}
      onCancel={() => navigate('/profile')}
    />
  );
}

export default EditProfilePage;