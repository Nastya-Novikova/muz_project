import React, { useState, useEffect } from 'react';
import './LoginPage.css';

function LoginOTP({ onLogin }) {
  const [email, setEmail] = useState('');
  const [code, setCode] = useState('');
  const [isEmailSent, setIsEmailSent] = useState(false);
  const [isCodeResendAvailable, setIsCodeResendAvailable] = useState(false);
  const [timer, setTimer] = useState(60);

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
    onLogin(email);
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

          {/* Поле кода */}
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