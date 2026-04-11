import React, { useEffect } from 'react';

const VkAuthButton = ({ onSuccess, onError }) => {
  // Генерация случайной строки для code_verifier
  const generateCodeVerifier = () => {
    const chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-';
    let result = '';
    for (let i = 0; i < 64; i++) {
      result += chars[Math.floor(Math.random() * chars.length)];
    }
    return result;
  };

  // Генерация случайной строки для state
  const generateState = () => {
    const chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    let result = '';
    for (let i = 0; i < 32; i++) {
      result += chars[Math.floor(Math.random() * chars.length)];
    }
    return result;
  };

  // Вычисление code_challenge из code_verifier (SHA256 + base64url)
  const generateCodeChallenge = async (codeVerifier) => {
    const encoder = new TextEncoder();
    const data = encoder.encode(codeVerifier);
    const digest = await crypto.subtle.digest('SHA-256', data);
    return btoa(String.fromCharCode(...new Uint8Array(digest)))
      .replace(/\+/g, '-')
      .replace(/\//g, '_')
      .replace(/=+$/, '');
  };

  useEffect(() => {
    const script = document.createElement('script');
    script.src = 'https://unpkg.com/@vkid/sdk@<3.0.0/dist-sdk/umd/index.js';
    script.async = true;
    script.onload = () => initializeVKID();
    script.onerror = () => onError?.(new Error('Failed to load VK ID SDK'));
    document.body.appendChild(script);
  }, []);

  const initializeVKID = async () => {
    if (!window.VKIDSDK) return;

    const VKID = window.VKIDSDK;
    
    // Генерируем PKCE параметры
    const codeVerifier = generateCodeVerifier();
    const codeChallenge = await generateCodeChallenge(codeVerifier);
    const state = generateState();
    
    // Сохраняем для отправки на бэкенд
    localStorage.setItem('vk_code_verifier', codeVerifier);
    localStorage.setItem('vk_state', state);

    VKID.Config.init({
      app: 54526928,
      redirectUrl: 'https://musicianfinder.cloudpub.ru/profile',
      responseMode: VKID.ConfigResponseMode.Callback,
      source: VKID.ConfigSource.LOWCODE,
      scope: '',
      codeChallenge: codeChallenge,
      codeChallengeMethod: 'S256',
      state: state,
    });

    const floatingOneTap = new VKID.FloatingOneTap();

    floatingOneTap.render({
      appName: 'MusicianFinder',
      fastAuthEnabled: false,
      showAlternativeLogin: true,
    })
    .on(VKID.WidgetEvents.ERROR, (error) => {
      console.error('VK ID Error:', error);
      onError?.(error);
    })
    .on(VKID.FloatingOneTapInternalEvents.LOGIN_SUCCESS, (payload) => {
      const { code, device_id, state: returnedState } = payload;
      
      // Проверяем state
      const savedState = localStorage.getItem('vk_state');
      if (savedState !== returnedState) {
        console.error('State mismatch');
        onError?.(new Error('Security validation failed'));
        return;
      }
      
      const savedCodeVerifier = localStorage.getItem('vk_code_verifier');
      
      // Очищаем
      localStorage.removeItem('vk_code_verifier');
      localStorage.removeItem('vk_state');
      
      floatingOneTap.close();
      onSuccess?.(code, savedCodeVerifier, device_id);
    });
  };

  return null;
};

export default VkAuthButton;