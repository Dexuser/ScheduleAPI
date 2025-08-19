// ======================
// Utilidades
// ======================

class PopupManager {
    constructor() {
        this.queue = [];
        this.isShowing = false;
        this.container = document.getElementById("popup-container");
    }

    show(message, type = "info", duration = 3000) {
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

        const popup = document.createElement("div");
        popup.textContent = message;
        popup.setAttribute("role", "alert");
        Object.assign(popup.style, {
            backgroundColor: this._getColor(type),
            color: "white",
            padding: "12px 20px",
            borderRadius: "8px",
            fontWeight: "bold",
            minWidth: "200px",
            textAlign: "center",
            marginBottom: "8px",
            boxShadow: "0 2px 10px rgba(0,0,0,0.6)",
            opacity: "0",
            transition: "opacity 0.4s ease"
        });

        this.container.appendChild(popup);
        requestAnimationFrame(() => (popup.style.opacity = "1"));

        setTimeout(() => {
            popup.style.opacity = "0";
            setTimeout(() => {
                this.container.removeChild(popup);
                this._displayNext();
            }, 400);
        }, duration);
    }

    _getColor(type) {
        return {
            success: "#4caf50",
            error: "#f44336",
            warning: "#ff9800",
            info: "#2196f3"
        }[type] || "#333";
    }
}

const popupManager = new PopupManager();
const showPopup = (type, message, duration = 3000) =>
    popupManager.show(message, type, duration);

async function fetchWithPopup(url, options = {}, loadingMessage = null) {
    try {
        if (loadingMessage) showPopup("info", loadingMessage, 1000);
        const response = await fetch(url, options);
        let data = null;
        try {
            data = await response.json();
        } catch { }
        if (response.ok) return data;
        const errorMsg = data?.message || "Error inesperado";
        showPopup("error", errorMsg);
        throw new Error(errorMsg);
    } catch (error) {
        showPopup("error", error.message || "Error de conexión");
        throw error;
    }
}

// ======================
// Funciones de Citas
// ======================

// Cargar citas del usuario
async function loadMyAppointments() {
    const table = document.getElementById("citasTableBody");
    if (!table) return;

    table.innerHTML = "<tr><td colspan='4'>Cargando...</td></tr>";

    try {
        const data = await fetchWithPopup("http://localhost:5148/Shifts/WithThisUser", {
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
        });

        table.innerHTML = "";

        if (!data || data.length === 0) {
            table.innerHTML =
                "<tr><td colspan='4'>No tienes citas agendadas</td></tr>";
            return;
        }

        data.forEach(turno => {
            turno.slots
                .filter(slot => slot.isTaken)
                .forEach(slot => {
                    const row = document.createElement("tr");
                    row.innerHTML = `
            <td>${turno.date}</td>
            <td>${slot.startTime}</td>
            <td>${slot.endTime}</td>
            <td>
              <button class="btn btn-cancelar" onclick="cancelAppointment(${slot.id})">Cancelar</button>
            </td>
          `;
                    table.appendChild(row);
                });
        });
    } catch (err) {
        console.error("Error cargando citas:", err);
        table.innerHTML = "<tr><td colspan='4'>Error cargando citas</td></tr>";
    }
}

// Cargar turnos disponibles
async function loadAvailableShifts() {
    const table = document.getElementById("agendarTableBody");
    if (!table) return;

    table.innerHTML = "<tr><td colspan='6'>Cargando...</td></tr>";

    try {
        const data = await fetchWithPopup("http://localhost:5148/Shifts/WithOutThisUser", {
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
        });

        table.innerHTML = "";

        if (!data || data.length === 0) {
            table.innerHTML =
                "<tr><td colspan='6'>No hay turnos disponibles</td></tr>";
            return;
        }

        data.forEach(turno => {
            turno.slots
                .filter(slot => !slot.isTaken)
                .forEach(slot => {
                    const row = document.createElement("tr");
                    row.innerHTML = `
            <td>${turno.id}</td>
            <td>${turno.date}</td>
            <td>${turno.schedule.description}</td>
            <td>${turno.meetingDurationOnMinutes} min</td>
            <td>${slot.startTime} - ${slot.endTime}</td>
            <td>
              <button class="btn btn-agendar" onclick="bookShift(${slot.id})">Agendar</button>
            </td>
          `;
                    table.appendChild(row);
                });
        });
    } catch (err) {
        console.error("Error cargando turnos:", err);
        table.innerHTML = "<tr><td colspan='6'>Error cargando turnos</td></tr>";
    }
}

// Agendar cita
async function bookShift(slotId) {
    const userData = JSON.parse(localStorage.getItem("userData"));
    if (!userData) {
        showPopup("error", "Debes iniciar sesión primero");
        return;
    }

    try {
        await fetchWithPopup(
            "http://localhost:5148/Appointments",
            {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ userid: userData.sub, slotid: slotId })
            },
            "Agendando cita..."
        );

        showPopup("success", "Cita agendada con éxito");
        loadMyAppointments();
        loadAvailableShifts();
    } catch (err) {
        console.error("Error agendando cita:", err);
    }
}

// Cancelar cita
async function cancelAppointment(slotId) {
    try {
        await fetchWithPopup(
            "http://localhost:5148/Appointments",
            {
                method: "PATCH",
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ state: "CANCELED", slotid: slotId })
            },
            "Cancelando cita..."
        );

        showPopup("success", "Cita cancelada con éxito");
        loadMyAppointments();
        loadAvailableShifts();
    } catch (err) {
        console.error("Error cancelando cita:", err);
    }
}

// ======================
// Inicialización
// ======================
document.addEventListener("DOMContentLoaded", () => {
    loadMyAppointments();
    loadAvailableShifts();
});
