class PopupManager {
  constructor() {
    this.queue = [];
    this.isShowing = false;
    this.container = document.getElementById('popup-container');
  }

  show(message, type = 'info', duration = 3000) {
    this.queue.push({ message, type, duration });
    if (!this.isShowing) this._displayNext();
  }

  _displayNext() {
    if (!this.queue.length) {
      this.isShowing = false;
      return;
    }

    this.isShowing = true;
    const { message, type, duration } = this.queue.shift();

    const popup = document.createElement('div');
    popup.textContent = message;
    popup.setAttribute('role', 'alert');
    Object.assign(popup.style, {
      backgroundColor: this._getColor(type),
      color: 'white',
      padding: '15px 30px',
      borderRadius: '8px',
      fontWeight: 'bold',
      minWidth: '250px',
      textAlign: 'center',
      marginBottom: '10px',
      boxShadow: '0 2px 10px rgba(0,0,0,0.7)',
      opacity: '0',
      transition: 'opacity 0.4s ease'
    });

    this.container.appendChild(popup);
    requestAnimationFrame(() => popup.style.opacity = '1');

    setTimeout(() => {
      popup.style.opacity = '0';
      setTimeout(() => {
        this.container.removeChild(popup);
        this._displayNext();
      }, 400);
    }, duration);
  }

  _getColor(type) {
    return {
      success: '#4caf50',
      error: '#f44336',
      warning: '#ff9800',
      info: '#2196f3'
    }[type] || '#333';
  }
}

const popupManager = new PopupManager();
const showPopup = (type, message, duration = 3000) => popupManager.show(message, type, duration);

async function fetchWithPopup(url, options = {}, loadingMessage = null) {
  try {
    if (loadingMessage) showPopup('info', loadingMessage, 1000);
    const response = await fetch(url, options);
    let data = null;
    try { data = await response.json(); } catch {}
    if (response.ok) return data;
    const errorMsg = data?.message || 'Error inesperado';
    showPopup('error', errorMsg);
    throw new Error(errorMsg);
  } catch (error) {
    showPopup('error', error.message || 'Error de conexión');
    throw error;
  }
}

function decodeJWT(token) {
  try {
    return JSON.parse(atob(token.split('.')[1]));
  } catch (e) {
    console.error("Error decodificando JWT:", e);
    return null;
  }
}

async function login() {
  const username = document.getElementById('username').value.trim();
  const password = document.getElementById('password').value;
  const btn = document.getElementById('loginBtn');

  if (!username || !password) {
    showPopup('error', 'Por favor completa ambos campos');
    return;
  }

  btn.disabled = true;

  try {
    const data = await fetchWithPopup('http://localhost:5148/Users/Login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ UserName: username, Password: password })
    }, 'Iniciando sesión...');

    localStorage.setItem('token', data.token);
    const userData = decodeJWT(data.token);
    localStorage.setItem('userData', JSON.stringify(userData));

    showPopup('success', `¡Login exitoso, ${userData.username}!`);
    setTimeout(() => {
      window.location.href = userData["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] === 'ADMIN' ? '../Admin/Admin.html' : '../Citas/Citas.html';
    }, 1000);

  } catch (e) {
    btn.disabled = false;
  } finally {
    btn.disabled = false;
  }
}

document.getElementById('loginBtn')?.addEventListener('click', login);
function goToRegister() { window.location.href = 'register.html'; }
