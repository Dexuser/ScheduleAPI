// horarios.js

const API_URL = "http://localhost:5148/Schedules";

// ===================== ALERTAS =====================
function mostrarAlerta(mensaje, tipo = "info") {
    const alert = document.getElementById("alert");
    alert.textContent = mensaje;
    alert.className = `alert ${tipo}`;
    alert.style.display = "block";
    setTimeout(() => alert.style.display = "none", 3000);
}

// ===================== FETCH CON JWT =====================
async function fetchJWT(url, options = {}) {
    const token = localStorage.getItem("token");
    if (!token) {
        mostrarAlerta("Token no encontrado. Inicia sesión.", "error");
        throw new Error("Token no encontrado");
    }

    if (!options.headers) options.headers = {};
    options.headers["Authorization"] = `Bearer ${token}`;
    options.headers["Content-Type"] = "application/json";

    try {
        const response = await fetch(url, options);
        const data = await response.json().catch(() => null);

        if (!response.ok) {
            const msg = data?.message || "Error inesperado";
            mostrarAlerta(msg, "error");
            throw new Error(msg);
        }
        return data;
    } catch (e) {
        mostrarAlerta("Error de conexión con el servidor", "error");
        throw e;
    }
}

// ===================== VARIABLES =====================
let horarios = [];
let editandoId = null;

// ===================== RENDERIZAR TABLA =====================
function renderTabla() {
    const tbody = document.getElementById("tablaHorarios");
    tbody.innerHTML = "";

    if (!horarios || horarios.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;">No hay horarios registrados</td></tr>`;
        return;
    }

    horarios.forEach(h => {
        const fila = document.createElement("tr");
        fila.innerHTML = `
            <td>${h.id}</td>
            <td>${h.startTime}</td>
            <td>${h.endTime}</td>
            <td>${h.description || ""}</td>
            <td><button onclick="eliminarHorario(${h.id})">🗑️</button></td>
            <td><button onclick="editarHorario(${h.id})">✏️</button></td>
        `;
        tbody.appendChild(fila);
    });
}

// ===================== CARGAR HORARIOS =====================
async function cargarHorarios() {
    const tbody = document.getElementById("tablaHorarios");
    tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;">Cargando...</td></tr>`;

    try {
        horarios = await fetchJWT(API_URL);
        renderTabla();
    } catch {
        tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;">Error al cargar</td></tr>`;
    }
}

// ===================== LIMPIAR FORMULARIO =====================
function limpiarFormulario() {
    document.getElementById("horaInicio").value = "";
    document.getElementById("horaFin").value = "";
    document.getElementById("descripcion").value = "";
    editandoId = null;
    document.getElementById("cancelarBtn").style.display = "none";
}

// ===================== AGREGAR O EDITAR HORARIO =====================
async function agregarHorario() {
    const horaInicio = document.getElementById("horaInicio").value;
    const horaFin = document.getElementById("horaFin").value;
    const descripcion = document.getElementById("descripcion").value.trim();

    if (!horaInicio || !horaFin) {
        mostrarAlerta("Debes seleccionar ambas horas", "error");
        return;
    }

    if (horaFin < horaInicio) {
        mostrarAlerta("La hora de fin debe ser mayor o igual a la de inicio", "error");
        return;
    }

    const body = { startTime: horaInicio, endTime: horaFin, description: descripcion };

    try {
        if (editandoId) {
            // EDITAR
            await fetchJWT(`${API_URL}/${editandoId}`, { method: "PUT", body: JSON.stringify(body) });
            mostrarAlerta("Horario actualizado correctamente", "success");
        } else {
            // AGREGAR
            await fetchJWT(API_URL, { method: "POST", body: JSON.stringify(body) });
            mostrarAlerta("Horario agregado correctamente", "success");
        }
        limpiarFormulario();
        await cargarHorarios();
    } catch {}
}

// ===================== ELIMINAR HORARIO =====================
async function eliminarHorario(id) {
    if (!confirm("¿Seguro que deseas eliminar este horario?")) return;

    try {
        await fetchJWT(`${API_URL}/${id}`, { method: "DELETE" });
        mostrarAlerta("Horario eliminado correctamente", "success");
        await cargarHorarios();
    } catch {}
}

// ===================== EDITAR HORARIO =====================
function editarHorario(id) {
    const horario = horarios.find(h => h.id === id);
    if (!horario) return;

    document.getElementById("horaInicio").value = horario.startTime;
    document.getElementById("horaFin").value = horario.endTime;
    document.getElementById("descripcion").value = horario.description || "";
    editandoId = id;
    document.getElementById("cancelarBtn").style.display = "inline-block";
}

// ===================== CANCELAR EDICIÓN =====================
function cancelarEdicion() {
    limpiarFormulario();
}

// ===================== INICIALIZAR =====================
document.addEventListener("DOMContentLoaded", () => {
    cargarHorarios();
});
