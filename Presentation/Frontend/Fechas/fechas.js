// fechas.js

const API_URL = "http://localhost:5148/EnabledDates";
const tablaFechas = document.getElementById("tablaFechas");
const alertBox = document.getElementById("alert");

// Mostrar alertas
function mostrarAlerta(mensaje, tipo = "success") {
    alertBox.textContent = mensaje;
    alertBox.className = `alert ${tipo}`;
    setTimeout(() => {
        alertBox.textContent = "";
        alertBox.className = "alert";
    }, 3000);
}

// Cargar las fechas desde la API
async function cargarFechas() {
    try {
        const resp = await fetch(API_URL);
        if (!resp.ok) throw new Error("Error al cargar fechas");

        const data = await resp.json();
        renderTabla(data);
    } catch (error) {
        mostrarAlerta("No se pudieron cargar las fechas", "error");
        console.error(error);
    }
}

// Renderizar la tabla
function renderTabla(fechas) {
    tablaFechas.innerHTML = "";
    fechas.forEach(fecha => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${fecha.id}</td>
            <td>${fecha.startDate}</td>
            <td>${fecha.endDate}</td>
            <td><button onclick="eliminarFecha(${fecha.id})">Eliminar</button></td>
        `;
        tablaFechas.appendChild(row);
    });
}

// Agregar nueva fecha
async function agregarFecha() {
    const fechaInicio = document.getElementById("fechaInicio").value;
    const fechaFin = document.getElementById("fechaFin").value;

    if (!fechaInicio || !fechaFin) {
        mostrarAlerta("Debes seleccionar ambas fechas", "error");
        return;
    }

    const nuevaFecha = {
        startDate: fechaInicio,
        endDate: fechaFin
    };

    try {
        const resp = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(nuevaFecha)
        });

        if (!resp.ok) throw new Error("Error al agregar la fecha");

        mostrarAlerta("Fecha agregada correctamente");
        cargarFechas(); // refrescar tabla
    } catch (error) {
        mostrarAlerta("Error al agregar fecha", "error");
        console.error(error);
    }
}

// Eliminar fecha
async function eliminarFecha(id) {
    if (!confirm("¿Seguro que deseas eliminar esta fecha?")) return;

    try {
        const resp = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
        if (!resp.ok) throw new Error("Error al eliminar fecha");

        mostrarAlerta("Fecha eliminada correctamente");
        cargarFechas(); // refrescar tabla
    } catch (error) {
        mostrarAlerta("Error al eliminar fecha", "error");
        console.error(error);
    }
}

// Inicializar
cargarFechas();
