import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../../services/api';
import { useAuth } from '../../context/AuthContext';
import './LoginPage.css';

function LoginOTP() {
  const [email, setEmail] = useState('');
  const [code, setCode] = useState('');
  const [isEmailSent, setIsEmailSent] = useState(false);
  const [isCodeResendAvailable, setIsCodeResendAvailable] = useState(false);
  const [timer, setTimer] = useState(60);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { login } = useAuth();

  useEffect(() => {
    let interval = null;
    if (isEmailSent && !isCodeResendAvailable) {
      interval = setInterval(() => {
        setTimer(prev => {
          if (prev <= 1) {
            clearInterval(interval);
            setIsCodeResendAvailable(true);
            return 60;
          }
          return prev - 1;
        });
      }, 1000);
    }
    return () => clearInterval(interval);
  }, [isEmailSent, isCodeResendAvailable]);

  const handleSendCode = async () => {
    if (!email || !email.includes('@')) {
      setError('Введите корректный email');
      return;
    }

    setError('');
    try {
      await api.requestAuthCode(email);
      setIsEmailSent(true);
      setIsCodeResendAvailable(false);
      setTimer(60);
      setCode('');
    } catch (err) {
      setError('Не удалось отправить код');
    }
  };

  const handleVerifyCode = async () => {
    if (code.length !== 6) {
      setError('Введите 6-значный код');
      return;
    }
    
    setError('');
    try {
      const response = await api.loginWithCode(email, code);
      if (response.success && response.token) {
        // Логинимся - передаем user и token
        login(response.user, response.token);
        
        // Проверяем, создан ли профиль
        if (!response.user.profileCreated) {
          navigate('/profile/edit'); 
        } else {
          navigate('/');
        }
      } else {
         setError('Неверный код или ошибка сервера');
      }
    } catch (err) {
      setError('Не удалось войти');
    }  
  };

  const handleResetEmail = () => {
    setEmail('');
    setCode('');
    setIsEmailSent(false);
    setIsCodeResendAvailable(false);
    setTimer(60);
    setError('');
  };

  const handleLogoClick = () => {
    navigate('/');
  };

  return (
    <div className="login-page-wrapper">
      <div className="login-container">
        <h1 onClick={handleLogoClick} style={{ cursor: 'pointer' }}>
          Войти в MusicianFinder
        </h1>
        <p>Введите ваш email - мы отправим одноразовый код для входа</p>

        {error && <div className="error-message">{error}</div>}

        <div className="email-input-group">
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            disabled={isEmailSent}
            autoComplete='off'
            placeholder="Email"
            aria-label="Email для входа"
            className={isEmailSent ? 'email-input-stretched' : ''}
          />
          {!isEmailSent && (
            <button
              onClick={handleSendCode}
              disabled={!email || !email.includes('@')}
            >
              Отправить код
            </button>
          )}
        </div>

        {isEmailSent && (
          <div className='code-input-group'>
            <input
              type="text"
              maxLength={6}
              value={code}
              onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))}
              placeholder="Введите код"
              className="code-input"
              aria-label="Одноразовый код" />
          </div>
        )}  

        {isEmailSent && (
          <div className="login-links">
            <button onClick={handleResetEmail}>Изменить почту</button>
            <button
              onClick={handleSendCode}
              disabled={!isCodeResendAvailable}
            >
              {isCodeResendAvailable ? 'Отправить код ещё раз' : `Отправить через ${timer}с`}
            </button>
          </div>
        )}

        {isEmailSent && (
          <div className="enter-group">
            <button
              onClick={handleVerifyCode}
              disabled={code.length !== 6}
            >
              Войти
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default LoginOTP;