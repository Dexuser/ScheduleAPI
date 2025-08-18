const API_URL = "http://localhost:5148/Appointments";
function cancelarCita(id) {
    if (confirm('¿Está seguro de que desea cancelar esta cita?')) {
        citasData = citasData.filter(cita => cita.id !== id);
        actualizarTablaCitas();

        const disponibilidad = disponibilidadesData.find(d => d.id === id);
        if (disponibilidad && disponibilidad.citasAgendadas > 0) {
            disponibilidad.citasAgendadas--;
        }
        actualizarTablaDisponibilidades();
    }
}

function agendarCita(id) {
    const disponibilidad = disponibilidadesData.find(d => d.id === id);
    if (disponibilidad && disponibilidad.citasAgendadas < disponibilidad.estaciones) {
        const nuevaCita = {
            id: Date.now(),
            fecha: disponibilidad.fecha,
            estaciones: disponibilidad.estaciones,
            duracion: disponibilidad.duracion,
            horario: disponibilidad.horario,
            agendada: true
        };
        citasData.push(nuevaCita);
        disponibilidad.citasAgendadas++;
        actualizarTablaCitas();
        actualizarTablaDisponibilidades();
        alert('Cita agendada exitosamente');
    } else {
        alert('No hay cupos disponibles para esta fecha');
    }
}

function mostrarModalNuevaCita() {
    document.getElementById('modalNuevaCita').style.display = 'block';
}

function cerrarModal() {
    document.getElementById('modalNuevaCita').style.display = 'none';
    document.getElementById('formNuevaCita').reset();
}

function formatearHorario(fecha, horaInicio, horaFin) {
    const diasSemana = ['domingo', 'lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado'];
    const fechaObj = new Date(fecha + 'T00:00:00');
    const diaSemana = diasSemana[fechaObj.getDay()];
    return `${diaSemana} ${horaInicio}:00 - ${horaFin}:00`;
}

function actualizarTablaCitas() {
    const tbody = document.getElementById('citasTableBody');
    tbody.innerHTML = '';
    citasData.forEach(cita => {
        const row = tbody.insertRow();
        row.innerHTML = `
            <td>${cita.id}</td>
            <td>${cita.fecha}</td>
            <td>${cita.estaciones}</td>
            <td>${cita.duracion}</td>
            <td><div class="horario-text">${cita.horario}</div></td>
            <td><button class="btn btn-cancelar" onclick="cancelarCita(${cita.id})">CANCELAR</button></td>
        `;
    });
}

function actualizarTablaDisponibilidades() {
    const tbody = document.getElementById('agendarTableBody');
    tbody.innerHTML = '';
    disponibilidadesData.forEach(disp => {
        const puedeAgendar = disp.citasAgendadas < disp.estaciones;
        const row = tbody.insertRow();
        row.innerHTML = `
            <td>${disp.id}</td>
            <td>${disp.fecha}</td>
            <td>${disp.estaciones}</td>
            <td>${disp.citasAgendadas}</td>
            <td>${disp.duracion}</td>
            <td><div class="horario-text">${disp.horario}</div></td>
            <td><button class="btn btn-agendar" onclick="agendarCita(${disp.id})" ${!puedeAgendar ? 'disabled' : ''}>Agendar</button></td>
        `;
    });
}

document.getElementById('formNuevaCita').addEventListener('submit', function(e) {
    e.preventDefault();
    const fecha = document.getElementById('fecha').value;
    const estaciones = parseInt(document.getElementById('estaciones').value);
    const duracion = parseInt(document.getElementById('duracion').value);
    const horaInicio = document.getElementById('horaInicio').value;
    const horaFin = document.getElementById('horaFin').value;

    if (horaInicio >= horaFin) {
        alert('La hora de inicio debe ser menor que la hora de fin');
        return;
    }

    const fechaFormateada = new Date(fecha).toLocaleDateString('es-ES');
    const horarioFormateado = formatearHorario(fecha, horaInicio, horaFin);

    const nuevaDisponibilidad = {
        id: disponibilidadesData.length + 1,
        fecha: fechaFormateada,
        estaciones: estaciones,
        citasAgendadas: 0,
        duracion: duracion,
        horario: horarioFormateado
    };

    disponibilidadesData.push(nuevaDisponibilidad);
    actualizarTablaDisponibilidades();
    cerrarModal();
    alert('Nueva disponibilidad creada exitosamente');
});

window.onclick = function(event) {
    const modal = document.getElementById('modalNuevaCita');
    if (event.target == modal) {
        cerrarModal();
    }
};

// Inicializar tablas
actualizarTablaCitas();
actualizarTablaDisponibilidades();
