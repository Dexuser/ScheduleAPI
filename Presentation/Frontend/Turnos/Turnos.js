const API_URL = "http://localhost:5148/Shifts";

function getToken() {
    return localStorage.getItem("jwt");
}

//Cargar todos los turnos tanto admin y user 
async function cargarTurnos() {
    try {
        const response = await fetch(API_URL, {
            headers: { "Authorization": `Bearer ${getToken()}` }
        });

        if (!response.ok) {
            alert("Error al cargar turnos");
            return;
        }

        const shifts = await response.json();
        renderizarTurnos(shifts);
    } catch (err) {
        console.error(err);
        alert("Error al conectar con el servidor");
    }
}

//Cargar solo los turnos del usuario logueado
async function cargarTurnosUsuario() {
    try {
        const response = await fetch(`${API_URL}/OfUser`, {
            headers: { "Authorization": `Bearer ${getToken()}` }
        });

        if (!response.ok) {
            alert("Error al cargar tus turnos");
            return;
        }

        const shifts = await response.json();
        renderizarTurnos(shifts);
    } catch (err) {
        console.error(err);
        alert("Error al conectar con el servidor");
    }
}

//Renderizar turnos en la tabla
function renderizarTurnos(shifts) {
    const tableBody = document.getElementById('turnosTableBody');
    tableBody.innerHTML = "";

    shifts.forEach(shift => {
        const newRow = tableBody.insertRow();
        newRow.innerHTML = `
            <td>${shift.id}</td>
            <td>${shift.date || 'N/A'}</td>
            <td>${shift.stations || 'N/A'}</td>
            <td>${shift.duration || 'N/A'}</td>
            <td>${shift.schedule || 'N/A'}</td>
            <td>
                <div class="action-buttons">
                    <button class="action-btn delete-btn" onclick="eliminarTurno(${shift.id})" title="Eliminar">✖</button>
                </div>
            </td>
        `;
    });
}

//Crear turno (ADMIN)
async function agregarTurno() {
    const fecha = document.getElementById('fecha').value;
    const duracion = document.getElementById('duracion').value;
    const estaciones = document.getElementById('estaciones').value;
    const horarioSelect = document.getElementById('horario');
    const horarioTexto = horarioSelect.options[horarioSelect.selectedIndex].text;

    if (!fecha || !duracion || !estaciones) {
        alert("Todos los campos son obligatorios");
        return;
    }

    const shiftData = {
        date: fecha,
        duration: parseInt(duracion),
        stations: parseInt(estaciones),
        schedule: horarioTexto
    };

    try {
        const response = await fetch(API_URL, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${getToken()}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(shiftData)
        });

        if (!response.ok) {
            const error = await response.json();
            alert(error.message || "Error creando turno");
            return;
        }

        alert("Turno creado con éxito");
        cargarTurnos();
        limpiarFormulario();
    } catch (err) {
        console.error(err);
        alert("Error al conectar con el servidor");
    }
}

//Eliminar turno (ADMIN)
async function eliminarTurno(id) {
    if (!confirm("¿Seguro que quieres eliminar este turno?")) return;

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: "DELETE",
            headers: { "Authorization": `Bearer ${getToken()}` }
        });

        if (!response.ok) {
            const error = await response.json();
            alert(error.message || "Error eliminando turno");
            return;
        }

        alert("Turno eliminado con éxito");
        cargarTurnos();
    } catch (err) {
        console.error(err);
        alert("Error al conectar con el servidor");
    }
}

//Limpiar formulario
function limpiarFormulario() {
    document.getElementById('fecha').value = "";
    document.getElementById('duracion').value = "";
    document.getElementById('estaciones').value = "";
    document.getElementById('horario').selectedIndex = 0;
}

// Cargar turnos al iniciar
document.addEventListener("DOMContentLoaded", cargarTurnos);
