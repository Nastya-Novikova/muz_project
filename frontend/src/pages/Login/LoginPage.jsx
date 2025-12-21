import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './LoginPage.css';

function LoginOTP() {
  const [email, setEmail] = useState('');
  const [code, setCode] = useState('');
  const [isEmailSent, setIsEmailSent] = useState(false);
  const [isCodeResendAvailable, setIsCodeResendAvailable] = useState(false);
  const [timer, setTimer] = useState(60);
  const navigate = useNavigate();
  const { login, setIsNewUser } = useAuth();

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

  const handleSendCode = () => {
    if (!email || !email.includes('@')) return;
    console.log('Отправляем код на:', email);
    setIsEmailSent(true);
    setIsCodeResendAvailable(false);
  };

  const handleVerifyCode = () => {
    if (code.length !== 6) return;
    console.log('Проверяем код:', code);
    
    const existingUsers = JSON.parse(localStorage.getItem('musicianFinder_users') || '{}');
    const existingUser = existingUsers[email];
    
    if (existingUser) {
      login(existingUser);
      navigate('/');
    } else {
      const newUser = {
        id: Date.now().toString(),
        email: email,
        name: '',
        avatar: `https://ui-avatars.com/api/?name=${encodeURIComponent(email)}&background=random`,
        isAuthenticated: true,
        profileCompleted: false,
        fullName: '',
        age: '',
        activityType: '',
        contact: email,
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
      };
      
      existingUsers[email] = newUser;
      localStorage.setItem('musicianFinder_users', JSON.stringify(existingUsers));
      
      login(newUser);
      setIsNewUser(true);
      navigate('/profile/edit');
    }
  };

  const handleResetEmail = () => {
    setEmail('');
    setCode('');
    setIsEmailSent(false);
    setIsCodeResendAvailable(false);
    setTimer(60);
  };

  return (
    <div className="login-page-wrapper">
      <div className="login-container">
        <h1>Войти в MusicianFinder</h1>
        <p>Введите ваш email - мы отправим одноразовый код для входа</p>

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