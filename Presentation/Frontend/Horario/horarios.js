// fechas.js

const API_URL = "http://localhost:5148/EnabledDates";

// ===================== ALERTAS =====================
function mostrarAlerta(mensaje, tipo) {
    const alert = document.getElementById("alert");
    alert.textContent = mensaje;
    alert.className = `alert ${tipo}`;
    alert.style.display = "block";
    setTimeout(() => alert.style.display = "none", 3000);
}

// ===================== RENDERIZAR TABLA =====================
function renderTabla(fechas) {
    const tbody = document.getElementById("tablaFechas");
    tbody.innerHTML = "";

    if (!fechas || fechas.length === 0) {
        const fila = document.createElement("tr");
        fila.innerHTML = `<td colspan="4" style="text-align:center;">No hay fechas registradas</td>`;
        tbody.appendChild(fila);
        return;
    }

    fechas.forEach(f => {
        const fila = document.createElement("tr");
        fila.innerHTML = `
            <td>${f.id}</td>
            <td>${f.startDate}</td>
            <td>${f.endDate}</td>
            <td><button onclick="eliminarFecha(${f.id})">🗑️</button></td>
        `;
        tbody.appendChild(fila);
    });
}

// ===================== CARGAR FECHAS =====================
async function cargarFechas() {
    const tbody = document.getElementById("tablaFechas");
    tbody.innerHTML = `<tr><td colspan="4" style="text-align:center;">Cargando...</td></tr>`;

    try {
        const resp = await fetch(API_URL, {
            headers: { "Authorization": `Bearer ${localStorage.getItem("token")}` }
        });

        if (resp.status === 404) {
            renderTabla([]);
            return;
        }

        if (!resp.ok) {
            mostrarAlerta("No se pudieron obtener las fechas", "error");
            tbody.innerHTML = `<tr><td colspan="4" style="text-align:center;">Error al cargar</td></tr>`;
            return;
        }

        const fechas = await resp.json();
        renderTabla(fechas);
    } catch (e) {
        mostrarAlerta("Error de conexión con el servidor", "error");
        tbody.innerHTML = `<tr><td colspan="4" style="text-align:center;">Error de conexión</td></tr>`;
    }
}

// ===================== AGREGAR FECHA =====================
async function agregarFecha() {
    const fechaInicio = document.getElementById("fechaInicio").value;
    const fechaFin = document.getElementById("fechaFin").value;

    if (!fechaInicio || !fechaFin) {
        mostrarAlerta("Debes seleccionar ambas fechas", "error");
        return;
    }

    if (fechaFin < fechaInicio) {
        mostrarAlerta("La fecha de fin debe ser mayor o igual que la de inicio", "error");
        return;
    }

    const body = {
        startDate: fechaInicio,
        endDate: fechaFin
    };

    try {
        const resp = await fetch(API_URL, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${localStorage.getItem("token")}`
            },
            body: JSON.stringify(body)
        });

        if (!resp.ok) {
            const error = await resp.json();
            mostrarAlerta(error.message || "Error al agregar fecha", "error");
            return;
        }

        mostrarAlerta("Fecha agregada correctamente", "success");
        await cargarFechas();
    } catch (e) {
        mostrarAlerta("Error de conexión con el servidor", "error");
    }
}

// ===================== ELIMINAR =====================
async function eliminarFecha(id) {
    if (!confirm("¿Seguro que deseas eliminar esta fecha?")) return;

    try {
        const resp = await fetch(`${API_URL}/${id}`, {
            method: "DELETE",
            headers: { "Authorization": `Bearer ${localStorage.getItem("token")}` }
        });

        if (!resp.ok) {
            const error = await resp.json();
            mostrarAlerta(error.message || "Error al eliminar fecha", "error");
            return;
        }

        mostrarAlerta("Fecha eliminada correctamente", "success");
        await cargarFechas();
    } catch (e) {
        mostrarAlerta("Error de conexión con el servidor", "error");
    }
}

// ===================== INICIALIZAR =====================
document.addEventListener("DOMContentLoaded", () => {
    cargarFechas();
});
