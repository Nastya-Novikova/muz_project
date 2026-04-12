// src/components/SuggestionModal/SuggestionModal.jsx
import React, { useState } from 'react';
import './SuggestionModal.css';

function SuggestionModal({ isOpen, onClose, onSend, userName }) {
  const [message, setMessage] = useState('');
  const [sending, setSending] = useState(false);

  const handleSend = async () => {
    if (!message.trim()) {
      alert('Введите текст сообщения');
      return;
    }

    setSending(true);
    try {
      await onSend(message);
      setMessage('');
      onClose();
    } catch (error) {
      alert('Не удалось отправить предложение');
    } finally {
      setSending(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="suggestion-modal-overlay" onClick={onClose}>
      <div className="suggestion-modal-content" onClick={(e) => e.stopPropagation()}>
        <h2>Предложение сотрудничества</h2>
        <p>Напишите сообщение для <strong>{userName}</strong></p>
        
        <textarea
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Введите ваше предложение..."
          rows={5}
          maxLength={200}
          className="suggestion-textarea"
          autoFocus
        />
        
        <div className="suggestion-modal-actions">
          <button onClick={onClose} className="suggestion-btn-cancel" disabled={sending}>
            Отмена
          </button>
          <button 
            onClick={handleSend} 
            className="suggestion-btn-send" 
            disabled={sending || !message.trim()}
          >
            {sending ? 'Отправка...' : 'Отправить'}
          </button>
        </div>
      </div>
    </div>
  );
}

export default SuggestionModal;