// Gestor de popups con cola y accesibilidad
class PopupManager {
  constructor() {
    this.queue = [];
    this.isShowing = false;
    this.container = document.getElementById('popup-container');
  }

  show(message, type = 'info', duration = 3000) {
    this.queue.push({ message, type, duration });
    if (!this.isShowing) {
      this._displayNext();
    }
  }

  _displayNext() {
    if (this.queue.length === 0) {
      this.isShowing = false;
      return;
    }
    this.isShowing = true;
    const { message, type, duration } = this.queue.shift();

    const popup = document.createElement('div');
    popup.textContent = message;
    popup.setAttribute('role', 'alert');
    popup.style.backgroundColor = this._getColor(type);
    popup.style.color = 'white';
    popup.style.padding = '15px 30px';
    popup.style.borderRadius = '8px';
    popup.style.fontWeight = 'bold';
    popup.style.minWidth = '250px';
    popup.style.textAlign = 'center';
    popup.style.marginBottom = '10px';
    popup.style.boxShadow = '0 2px 10px rgba(0,0,0,0.7)';
    popup.style.opacity = '0';
    popup.style.transition = 'opacity 0.4s ease';

    this.container.appendChild(popup);

    // Animar entrada
    requestAnimationFrame(() => {
      popup.style.opacity = '1';
    });

    setTimeout(() => {
      // Animar salida
      popup.style.opacity = '0';
      setTimeout(() => {
        this.container.removeChild(popup);
        this._displayNext();
      }, 400);
    }, duration);
  }

  _getColor(type) {
    switch(type) {
      case 'success': return '#4caf50';
      case 'error': return '#f44336';
      case 'warning': return '#ff9800';
      case 'info': return '#2196f3';
      default: return '#333';
    }
  }
}

const popupManager = new PopupManager();

function showPopup(message, type = 'info', duration = 3000) {
  popupManager.show(message, type, duration);
}

// Wrapper para fetch que muestra popups segun respuesta HTTP
async function fetchWithPopup(url, options = {}) {
  try {
    const response = await fetch(url, options);

    let data = null;
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
      data = await response.json();
    } else {
      data = await response.text(); // Para debug, si no es JSON
      console.warn('Respuesta no JSON:', data);
    }

    if (response.ok) {
      switch (response.status) {
        case 200:
          showPopup('Acción realizada con éxito.', 'success');
          break;
        case 201:
          showPopup('Recurso creado con éxito.', 'success');
          break;
        case 204:
          showPopup('Eliminado correctamente.', 'success');
          break;
      }
      return data;
    } else {
      switch(response.status) {
        case 400:
          showPopup(data?.message || data || 'Error en la solicitud.', 'error');
          break;
        case 401:
          showPopup('Acceso denegado, por favor inicia sesión.', 'error');
          break;
        case 404:
          showPopup('No se encontraron resultados.', 'warning');
          break;
        default:
          showPopup('Error inesperado.', 'error');
      }
      throw new Error(data?.message || data || 'Error en la respuesta del servidor');
    }
  } catch (error) {
    showPopup('Error de conexión o inesperado.', 'error');
    throw error;
  }
}

// Función para crear cuenta usando fetchWithPopup
async function createAccount() {
  const email = document.getElementById("email").value.trim();
  const username = document.getElementById("username").value.trim();
  const password = document.getElementById("password").value;
  const btn = document.getElementById("createAccountBtn");

  if (!email || !username || !password) {
    showPopup("Por favor, completa todos los campos.", "error");
    return;
  }

  const userData = {
    email,
    UserName: username,
    Password: password,
  };

  btn.disabled = true;
  const originalText = btn.textContent;

  try {
    await fetchWithPopup('http://localhost:5148/Users/Register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(userData)
    });
    
    btn.textContent = "Creado con éxito";
    setTimeout(() => {
      btn.textContent = originalText;
      window.location.href = "../Login/login.html";
    }, 2000);
  } catch {
    btn.disabled = false;
  }
}

