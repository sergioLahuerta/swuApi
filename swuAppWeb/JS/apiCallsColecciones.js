// Cambia estas rutas a la URL real de tu backend
const API_COLECCION = "http://localhost:8309/api/Collection";
const API_CARTA = "http://localhost:8309/api/Card";

async function getColeccionById(id) {
    const response = await fetch(`${API_COLECCION}/${id}`);
    if (response.status === 404) return null;
    const item = await response.json();
    return item;
}

// Crear Colección
document.getElementById('crearColeccionForm').onsubmit = async function (e) {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(this));
    data.numCards = parseInt(data.numCards, 10);

    const msgElementoCrear = document.getElementById('coleccionMsgCrear');

    try {
        const res = await fetch(API_COLECCION, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (res.ok) {
            msgElementoCrear.style.backgroundColor = '#249964';
            msgElementoCrear.style.borderColor = '#249964';
            msgElementoCrear.innerText = 'Colección creada correctamente';
        } else {
            msgElementoCrear.style.backgroundColor = '#b33939';
            msgElementoCrear.style.borderColor = '#b33939';
            const errorText = await res.text();
            msgElementoCrear.innerText = `Error creando colección: ${errorText}`;
        }
    } catch (error) {
        msgElementoCrear.style.backgroundColor = '#b33939';
        msgElementoCrear.style.borderColor = '#b33939';
        msgElementoCrear.innerText = 'Error en la conexión';
    }

    msgElementoCrear.style.display = 'block';
    this.reset();
};


// Generar color de fondo de cada card
function colorFondoCard(color, percent) {
    let R = parseInt(color.substring(1,3),16);
    let G = parseInt(color.substring(3,5),16);
    let B = parseInt(color.substring(5,7),16);

    R = Math.round(R * (100 + percent) / 100);
    G = Math.round(G * (100 + percent) / 100);
    B = Math.round(B * (100 + percent) / 100);

    R = (R < 255)?R:255;
    G = (G < 255)?G:255;
    B = (B < 255)?B:255;

    let RR = (R.toString(16).length==1)?"0"+R.toString(16):R.toString(16);
    let GG = (G.toString(16).length==1)?"0"+G.toString(16):G.toString(16);
    let BB = (B.toString(16).length==1)?"0"+B.toString(16):B.toString(16);

    return "#"+RR+GG+BB;
}


// Obtener colección con cartas por Id
async function mostrarColeccionConCartas(id) {
    // Obtener colección
    const resColeccion = await fetch(`${API_COLECCION}/${id}`);
    if (!resColeccion.ok) {
        alert('Error cargando colección');
        return;
    }
    const collection = await resColeccion.json();
    const color = collection.color;

    // Obtener cartas de esa colección
    const resCartas = await fetch(`${API_CARTA}?filterField=CollectionId&filterValue=${id}`);
    const cards = resCartas.ok ? await resCartas.json() : [];

    const cartasContainer = document.getElementById('cartasColeccion');
    cartasContainer.innerHTML = `<h3 style="width: fit-content; background: linear-gradient(120deg,#cfffdf,#274eb7,#b7c7fc 75%); background-size: 400% 400%; animation: holo 8s ease infinite; color: ${color}; font-weight: 900; padding: 18px 44px; border-radius: 13px; box-shadow: 0 4px 18px rgba(60,80,130,.22); font-size: 2.1rem; border: 2px solid #b0e4ff;">Cartas de la colección:</h3>`;

    if (cards.length > 0) {
        const divContenedor = document.createElement('div');
        divContenedor.className = 'cartas-contenedor';
        divContenedor.style.cssText = `width: fit-content; display: flex; gap: 10px; padding: 15px; border-radius: 15px;`;

        cards.forEach(c => {
            const div = document.createElement('div');
            div.className = 'carta-item';
            const colorDark = colorFondoCard(color, -55);
            div.style.background = `linear-gradient(180deg, ${color} 80%, ${colorDark} 100%)`;

            // Ajustes de model
            if(c.model === 'Foil') {
                div.style.filter = 'brightness(1.2)'
            }

            if(c.model === 'Hyperspace') {
                div.style.background = `linear-gradient(180deg, #222228 80%, #222228 100%)`;
            }
            
            if(c.model === 'Hyperspace Foil') {
                div.style.background = `linear-gradient(180deg, #222228 80%, #222228 100%)`;
                div.style.filter = 'brightness(1.2)'
            }

            if(c.model === 'Showcase') {
                div.style.background = `linear-gradient(180deg, #a5a5bcff 80%, #a5a5bcff 100%)`;
                div.style.filter = 'brightness(1.2)'
            }

            div.innerHTML = `
            <div style='background-color: #222228; border-radius: 15px; width: -webkit-fill-available; padding: 10px;'>
                <p style='background-color: #f0f0f0; font-weight: 800; color: ${collection.color}; margin-bottom: 10px;'>${c.cardName}</p>
                <p style='border-radius: 15px;'>${c.subtitle ? `<span>${c.subtitle}</span>` : ''}</p>
            </div>
            <div style='background-color: #222228; border-radius: 15px; width: -webkit-fill-available; padding: 10px;'>
                <p style='background-color: #f0f0f0; color: #000000;'><span>Copias:</span> ${c.copies || 1}</p>
                <p>${c.model}</p>
                <p>${c.cardNumber}/${collection.numCards}</p>
            </div>
            `;

            divContenedor.appendChild(div);
        });

        cartasContainer.appendChild(divContenedor);
    } else {
        cartasContainer.innerHTML += '<p>No hay cartas en esta colección.</p>';
    }
}


//Obtener todas las colecciones
async function mostrarColecciones() {
    const resultadosColecciones = document.getElementById('listaColecciones');
    const msgElementos = document.getElementById('coleccionMsgAll');
    msgElementos.style.display = 'none';

    try {
        const res = await fetch(API_COLECCION);
        if (!res.ok) throw new Error('Error al obtener colecciones');
        const collection = await res.json();
        resultadosColecciones.innerHTML = '';

        if (collection.length === 0) {
            msgElementos.style.backgroundColor = '#b33939';
            msgElementos.style.borderColor = '#b33939';
            msgElementos.innerText = 'No hay colecciones disponibles';
            msgElementos.style.display = 'block';
            return;
        }

        collection.forEach(c => {
            const btnCard = document.createElement('button');
            btnCard.type = 'button';
            btnCard.className = 'coleccion-item';
            btnCard.style.backgroundColor = c.color
            btnCard.innerHTML = `<h3 style= 'font-weight: 700;'>${c.collectionName}</h3>
            <h3 style= 'font-weight: 700;'>${c.color}</h3>
            <p style='color: white;'>Número de cartas: ${c.numCards}</p>`;
            btnCard.addEventListener('click', () => {
                mostrarColeccionConCartas(c.id);
            });
            resultadosColecciones.style.display = 'flex';
            resultadosColecciones.appendChild(btnCard);
        });

    } catch (err) {
        resultadosColecciones.innerHTML = '';
        msgElementos.style.backgroundColor = '#b33939';
        msgElementos.style.borderColor = '#b33939';
        msgElementos.innerText = 'Error obteniendo colecciones';
        msgElementos.style.display = 'block';
        console.error('Error obteniendo colecciones:', err);
    }
}
// LLamada a la función en buttonsOption.js

// Actualizar Colección
document.getElementById('actualizarColeccionForm').onsubmit = async function (e) {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(this));
    const msgElementoActualizar = document.getElementById('coleccionMsgActualizar');

    try {
        const res = await fetch(`${API_COLECCION}/cambiar-id`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                CollectionName: data.collectionName,
                color: data.color,
                CardNumber: parseInt(data.numCards, 10)
            })
        });

        if (res.ok) {
            msgElementoActualizar.style.backgroundColor = '#249964';
            msgElementoActualizar.style.borderColor = '#249964';
            msgElementoActualizar.innerText = 'Colección actualizada correctamente';
        } else {
            msgElementoActualizar.style.backgroundColor = '#b33939';
            msgElementoActualizar.style.borderColor = '#b33939';
            msgElementoActualizar.innerText = 'Error actualizando colección: ' + await res.text();
        }
        msgElementoActualizar.style.display = 'block';
    } catch (err) {
        msgElementoActualizar.style.backgroundColor = '#b33939';
        msgElementoActualizar.style.borderColor = '#b33939';
        msgElementoActualizar.innerText = 'Error de conexión';
        msgElementoActualizar.style.display = 'block';
    }

    this.reset();
};

// Eliminar Colección
document.getElementById('borrarColeccionForm').onsubmit = async function (e) {
    e.preventDefault();
    const id = new FormData(this).get('id');
    const msgElementoBorrar = document.getElementById('coleccionMsgBorrar');

    // Comprobar si la colección existe antes de borrar
    const collection = await getColeccionById(id);
    if (!collection) {
        msgElementoBorrar.innerText = `La colección con el Id: ${id} no existe`;
        msgElementoBorrar.style.backgroundColor = '#b33939';
        msgElementoBorrar.style.borderColor = '#b33939';
        this.reset();
        return;
    }

    try {
        const res = await fetch(`${API_COLECCION}/${id}`, { method: 'DELETE' });
        if (res.ok) {
            msgElementoBorrar.style.backgroundColor = '#249964';
            msgElementoBorrar.style.borderColor = '#249964';
            msgElementoBorrar.innerText = `Eliminada correctamente la colección con id: ${id}`;
        } else {
            msgElementoBorrar.style.backgroundColor = '#b33939';
            msgElementoBorrar.style.borderColor = '#b33939';
            const errorText = await res.text();
            msgElementoBorrar.innerText = `Error eliminando colección: ${errorText}`;
        }
        msgElementoBorrar.style.display = 'block';
    } catch (err) {
        msgElementoBorrar.style.backgroundColor = '#b33939';
        msgElementoBorrar.style.borderColor = '#b33939';
        msgElementoBorrar.innerText = 'Error en la conexión';
        msgElementoBorrar.style.display = 'block';
    }
    msgElementoBorrar.style.display = 'block';
    this.reset();
};