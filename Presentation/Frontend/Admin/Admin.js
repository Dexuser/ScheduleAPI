// URLs de redirección
const urls = {
    'agregar-horario': '../Horario/horarios.html',
    'habilitar-fechas': '../Fechas/fechas.html',
    'agregar-turnos': '../Turnos/Turnos.html'
};

// Variables globales
let isLocked = false;
let statusTimeout;

// Función principal de navegación
function navegarA(accion) {
    if (isLocked) {
        showStatus('Panel bloqueado. Desbloquea para continuar.', 'error');
        return;
    }

    const card = event.currentTarget;
    const originalContent = card.innerHTML;
    
    card.classList.add('loading');
    card.innerHTML = `
        <div class="loading"></div>
        <p style="margin-top: 10px;">Cargando...</p>
    `;

    setTimeout(() => {
        if (urls[accion]) {
            // 🔹 Redirección real
            window.location.href = urls[accion];
        } else {
            showStatus('URL no configurada para esta acción', 'error');
            card.innerHTML = originalContent;
            card.classList.remove('loading');
        }
    }, 1500);
}

// Resto de funciones sin cambios...
function editMode() { /* ... */ }
function refreshData() { /* ... */ }
function toggleLock() { /* ... */ }
function showMore() { /* ... */ }
function handleMoreOption(optionNumber, optionName) { /* ... */ }
function showStatus(message, type = 'success') { /* ... */ }
function addHoverEffects() { /* ... */ }
function handleKeyboardShortcuts(event) { /* ... */ }
function validateUrl(url) { /* ... */ }

document.addEventListener('DOMContentLoaded', function() {
    showStatus('Panel de administración cargado correctamente ✓');
    addHoverEffects();
    document.addEventListener('keydown', handleKeyboardShortcuts);
    console.log('Panel de administración inicializado');
    console.log('URLs configuradas:', urls);
});

window.addEventListener('error', function(event) {
    showStatus('Ha ocurrido un error inesperado', 'error');
    console.error('Error:', event.error);
});

window.AdminPanel = {
    navegarA,
    editMode,
    refreshData,
    toggleLock,
    showMore,
    showStatus
};
