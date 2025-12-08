// Segundo Aspecto
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOMContentLoaded');
    const svgButtonPlus = document.querySelector('.btnSvgAspectoSelectPlus');
    const svgButtonMinus = document.querySelector('.btnSvgAspectoSelectMinus')
    const segundoSelectAspecto = document.querySelector('.aspectoSelect2');
    console.log('svgButton:', svgButtonPlus);
    console.log('segundoSelectAspecto:', segundoSelectAspecto);

    if (svgButtonPlus && segundoSelectAspecto) {
        svgButtonPlus.addEventListener('click', () => {
        console.log('Botón pulsado');
        segundoSelectAspecto.style.display = 'block';
        svgButtonPlus.style.display = 'none';
        svgButtonMinus.style.display = 'block';
    });
    } else {
        console.error('NO encontrados:', svgButtonPlus, segundoSelectAspecto);
    }

    if (svgButtonMinus && segundoSelectAspecto) {
        svgButtonMinus.addEventListener('click', () => {
        console.log('Botón pulsado');
        segundoSelectAspecto.style.display = 'none';
        svgButtonMinus.style.display = 'none';
        svgButtonPlus.style.display = 'block';
    });
    } else {
        console.error('NO encontrados:', svgButtonMinus, segundoSelectAspecto);
    }
});

// Crear Carta
document.getElementById('crearCartaForm').onsubmit = async function(e) {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(this));
    const msgElemento = document.getElementById('cartaMsgCrear');

    if (!data.CardName || !data.CollectionId) {
        msgElemento.innerText = 'Nombre y colección son obligatorios';
        msgElemento.style.backgroundColor = '#b33939';
        msgElemento.style.borderColor = '#b33939';
        msgElemento.style.display = 'block';
        return;
    }

    data.CardNumber = parseInt(data.CardNumber, 10) || 0;
    data.copies = parseInt(data.copies, 10) || 0;
    data.CollectionId = parseInt(data.CollectionId, 10) || 0;

    try {
        const res = await fetch(API_CARTA, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data)
        });

        if (res.ok) {
            msgElemento.style.backgroundColor = '#249964';
            msgElemento.style.borderColor = '#249964';
            msgElemento.innerText = 'Carta creada correctamente';
        } else {
            msgElemento.style.backgroundColor = '#b33939';
            msgElemento.style.borderColor = '#b33939';
            const errorText = await res.text();
            msgElemento.innerText = 'Error creando carta: ' + errorText;
        }

        msgElemento.style.display = 'block';
    } catch (err) {
        msgElemento.style.backgroundColor = '#b33939';
        msgElemento.style.borderColor = '#b33939';
        msgElemento.innerText = 'Error en la conexion';
    }
}

// Actualizar Carta
document.getElementById('actualizarCartaForm').onsubmit = async function(e) {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(this));
    const msgElemento = document.getElementById('cartaMsgActualizar');

    if (!data.CardName || !data.Id) {
        msgElemento.innerText = 'Nombre y colección son obligatorios';
        msgElemento.style.backgroundColor = '#b33939';
        msgElemento.style.borderColor = '#b33939';
        msgElemento.style.display = 'block';
        return;
    }

    data.CardNumber = parseInt(data.CardNumber, 10) || 0;
    data.copies = parseInt(data.copies, 10) || 0;
    data.Id = parseInt(data.Id, 10) || 0;

    try {
        const res = await fetch(API_CARTA, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data)
        });

        if (res.ok) {
            msgElemento.style.backgroundColor = '#249964';
            msgElemento.style.borderColor = '#249964';
            msgElemento.innerText = 'Carta creada correctamente';
        } else {
            msgElemento.style.backgroundColor = '#b33939';
            msgElemento.style.borderColor = '#b33939';
            const errorText = await res.text();
            msgElemento.innerText = 'Error creando carta: ' + errorText;
        }

        msgElemento.style.display = 'block';
    } catch (err) {
        msgElemento.style.backgroundColor = '#b33939';
        msgElemento.style.borderColor = '#b33939';
        msgElemento.innerText = 'Error en la conexion';
    }
}

// Eliminar Carta
document.getElementById('eliminarCartaForm').onsubmit = async function(e) {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(this));
    const msgElemento = document.getElementById('cartaMsgEliminar');

    if (!data.CardName || !data.Id) {
        msgElemento.innerText = 'Nombre y colección son obligatorios';
        msgElemento.style.backgroundColor = '#b33939';
        msgElemento.style.borderColor = '#b33939';
        msgElemento.style.display = 'block';
        return;
    }

    data.CardNumber = parseInt(data.CardNumber, 10) || 0;
    data.copies = parseInt(data.copies, 10) || 0;
    data.Id = parseInt(data.Id, 10) || 0;

    try {
        const res = await fetch(API_CARTA, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data)
        });

        if (res.ok) {
            msgElemento.style.backgroundColor = '#249964';
            msgElemento.style.borderColor = '#249964';
            msgElemento.innerText = 'Carta creada correctamente';
        } else {
            msgElemento.style.backgroundColor = '#b33939';
            msgElemento.style.borderColor = '#b33939';
            const errorText = await res.text();
            msgElemento.innerText = 'Error creando carta: ' + errorText;
        }

        msgElemento.style.display = 'block';
    } catch (err) {
        msgElemento.style.backgroundColor = '#b33939';
        msgElemento.style.borderColor = '#b33939';
        msgElemento.innerText = 'Error en la conexion';
    }
}