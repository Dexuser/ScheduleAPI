// Turnos.js

const API_TURNOS = "http://localhost:5148/Shifts";
const API_HORARIOS = "http://localhost:5148/Schedules";
const tablaTurnos = document.getElementById("turnosTableBody");
const selectHorario = document.getElementById("horario");

// Obtener token desde localStorage
function getToken() {
    return localStorage.getItem("token");
}

// Mostrar alertas simples
function mostrarAlerta(mensaje, tipo = "success") {
    alert(mensaje); // ⚠️ Aquí puedes usar tu popupManager si quieres mantener consistencia
}

// Cargar horarios dinámicamente en el select
async function cargarHorarios() {
    try {
        const token = getToken();
        const resp = await fetch(API_HORARIOS, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!resp.ok) throw new Error("Error al cargar horarios");
        const horarios = await resp.json();

        selectHorario.innerHTML = "";
        horarios.forEach(h => {
            const option = document.createElement("option");
            option.value = h.id;
            option.textContent = `${h.description} (${h.startTime} - ${h.endTime})`;
            selectHorario.appendChild(option);
        });
    } catch (error) {
        mostrarAlerta("No se pudieron cargar los horarios", "error");
        console.error(error);
    }
}

// Cargar turnos en la tabla
async function cargarTurnos() {
    try {
        const token = getToken();
        const resp = await fetch(API_TURNOS, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!resp.ok) throw new Error("Error al cargar turnos");
        const turnos = await resp.json();
        renderTabla(turnos);
    } catch (error) {
        mostrarAlerta("No se pudieron cargar los turnos", "error");
        console.error(error);
    }
}

// Renderizar tabla
function renderTabla(turnos) {
    tablaTurnos.innerHTML = "";
    turnos.forEach(turno => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${turno.id}</td>
            <td>${turno.date}</td>
            <td>${turno.servicesSlots}</td>
            <td>${turno.meetingDurationOnMinutes}</td>
            <td>${turno.schedule.description} (${turno.schedule.startTime} - ${turno.schedule.endTime})</td>
            <td><button onclick="eliminarTurno(${turno.id})">Eliminar</button></td>
        `;
        tablaTurnos.appendChild(row);
    });
}

// Agregar turno
async function agregarTurno() {
    const fecha = document.getElementById("fecha").value;
    const duracion = document.getElementById("duracion").value;
    const estaciones = document.getElementById("estaciones").value;
    const horarioId = selectHorario.value;

    if (!fecha || !duracion || !estaciones || !horarioId) {
        mostrarAlerta("Todos los campos son obligatorios", "error");
        return;
    }

    const nuevoTurno = {
        date: fecha,
        meetingDurationOnMinutes: parseInt(duracion),
        servicesSlots: parseInt(estaciones),
        scheduleId: parseInt(horarioId)
    };

    try {
        const token = getToken();
        const resp = await fetch(API_TURNOS, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(nuevoTurno)
        });

        if (!resp.ok) throw new Error("Error al agregar turno");

        mostrarAlerta("Turno agregado correctamente");
        cargarTurnos();
    } catch (error) {
        mostrarAlerta("Error al agregar turno", "error");
        console.error(error);
    }
}

// Eliminar turno
async function eliminarTurno(id) {
    if (!confirm("¿Seguro que deseas eliminar este turno?")) return;

    try {
        const token = getToken();
        const resp = await fetch(`${API_TURNOS}/${id}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!resp.ok) throw new Error("Error al eliminar turno");

        mostrarAlerta("Turno eliminado correctamente");
        cargarTurnos();
    } catch (error) {
        mostrarAlerta("Error al eliminar turno", "error");
        console.error(error);
    }
}

// Inicializar
cargarHorarios();
cargarTurnos();
