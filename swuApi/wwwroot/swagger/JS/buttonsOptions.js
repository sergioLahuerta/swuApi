// GENERAL
// GENERAL
// GENERAL

document.getElementById('volverAtrasColecciones').addEventListener('click', () => {
    location.reload();
});

document.getElementById('volverAtrasCartas').addEventListener('click', () => {
    location.reload();
});

document.getElementById('volverAtrasSobres').addEventListener('click', () => {
    location.reload();
});

//SOBRESSS
//SOBRESSS
//SOBRESSS
function mostrarOpcionAbrirSobre() {
    const botonSobres = document.getElementById('sobres-buttonHome')
    botonSobres.addEventListener('click', () => {
        const seccionAbrirSobre = document.getElementById('abrirPacksContainer')
        seccionAbrirSobre.style.display = 'flex'
        seccionAbrirSobre.style.flexDirection = 'column'
        const body = document.getElementsByTagName('body')
        body[0].style.backgroundImage = "url('./src/backgroundImageAbrirSobres2.jpg')"
        
        botonSobres.style.display = 'none'
        
        const botonCartas = document.getElementById('cartas-buttonHome')
        botonCartas.style.display = 'none'

        const botonColecciones = document.getElementById('colecciones-buttonHome')
        botonColecciones.style.display = 'none'
    })
}
mostrarOpcionAbrirSobre();

// COLECCIONES
// COLECCIONES
// COLECCIONES
function mostrarOpcionesColeccion() {
    const botonColecciones = document.getElementById('colecciones-buttonHome')
    botonColecciones.addEventListener('click', () => {
        const seccionColecciones = document.getElementById('colecciones')
        seccionColecciones.style.display = 'flex'
        const body = document.getElementsByTagName('body')
        body[0].style.backgroundImage = "url('./src/backgroundColecciones.png')"
        
        botonColecciones.style.display = 'none'
        
        const botonCartas = document.getElementById('cartas-buttonHome')
        botonCartas.style.display = 'none'

        const botonSobres = document.getElementById('sobres-buttonHome')
        botonSobres.style.display = 'none';
    })
}
mostrarOpcionesColeccion();

//Funcion crear colecciones
function crearColeccionButton() {
    //Listas
    const listaColeccionesResultados = document.getElementById('listaColecciones')
    const cartasColeccion = document.getElementById('cartasColeccion')
    //Botones
    const crearColeccionBoton = document.getElementById('crearColeccionButton')
    const verTodasColeccionBoton = document.getElementById('todasColeccionesButton')
    const actualizarColeccionBoton = document.getElementById('actualizarColeccionButton')
    const eliminarColeccionBoton = document.getElementById('eliminarColeccionButton')
    //Secciones
    const crearColeccionSeccion = document.getElementById('crearColeccion')
    const actualizarColeccionSeccion = document.getElementById('actualizarColeccion')
    const eliminarColeccionSeccion = document.getElementById('eliminarColeccion')
    crearColeccionBoton.addEventListener('click', () =>{
        //Listas
        listaColeccionesResultados.style.display = 'none'
        cartasColeccion.style.display = 'none'
        cartasColeccion.innerHTML = ''
        //Botones
        crearColeccionBoton.style.display = 'none'
        verTodasColeccionBoton.style.display = 'block'
        actualizarColeccionBoton.style.display = 'block'
        eliminarColeccionBoton.style.display = 'block'
        //Secciones
        crearColeccionSeccion.style.display = 'flex'
        actualizarColeccionSeccion.style.display = 'none'
        eliminarColeccionSeccion.style.display = 'none'
    })
}
crearColeccionButton();


//Funcion ver todas las colecciones
function verTodasColeccionButton() {
    //Listas
    const listaColeccionesResultados = document.getElementById('listaColecciones')
    const cartasColeccion = document.getElementById('cartasColeccion')
    //Botones
    const verTodasColeccionBoton = document.getElementById('todasColeccionesButton')
    const crearColeccionBoton = document.getElementById('crearColeccionButton')
    const actualizarColeccionBoton = document.getElementById('actualizarColeccionButton')
    const eliminarColeccionBoton = document.getElementById('eliminarColeccionButton')
    //Secciones
    const crearColeccionSeccion = document.getElementById('crearColeccion')
    const actualizarColeccionSeccion = document.getElementById('actualizarColeccion')
    const eliminarColeccionSeccion = document.getElementById('eliminarColeccion')
    
    verTodasColeccionBoton.addEventListener('click', () => {
        mostrarColecciones();
        //Listas
        listaColeccionesResultados.style.display = 'block'
        cartasColeccion.style.display = 'block'
        cartasColeccion.innerHTML = ''
        //Botones
        verTodasColeccionBoton.style.display = 'none'
        crearColeccionBoton.style.display = 'block'
        actualizarColeccionBoton.style.display = 'block'
        eliminarColeccionBoton.style.display = 'block'
        //Secciones
        crearColeccionSeccion.style.display = 'none'
        actualizarColeccionSeccion.style.display = 'none'
        eliminarColeccionSeccion.style.display = 'none'
    })
}
verTodasColeccionButton();


//Funcion actualizar colecciones
function actualizarColeccionButton() {
    //Listas
    const listaColeccionesResultados = document.getElementById('listaColecciones')
    const cartasColeccion = document.getElementById('cartasColeccion')
    //Botones
    const actualizarColeccionBoton = document.getElementById('actualizarColeccionButton')
    const verTodasColeccionBoton = document.getElementById('todasColeccionesButton')
    const crearColeccionBoton = document.getElementById('crearColeccionButton')
    const eliminarColeccionBoton = document.getElementById('eliminarColeccionButton')
    //Secciones
    const actualizarColeccionSeccion = document.getElementById('actualizarColeccion')
    const crearColeccionSeccion = document.getElementById('crearColeccion')
    const eliminarColeccionSeccion = document.getElementById('eliminarColeccion')
    actualizarColeccionBoton.addEventListener('click', () =>{
        //Listas
        listaColeccionesResultados.style.display = 'none'
        cartasColeccion.style.display = 'none'
        cartasColeccion.innerHTML = ''
        //Botones
        actualizarColeccionBoton.style.display = 'none'
        verTodasColeccionBoton.style.display = 'block'
        crearColeccionBoton.style.display = 'block'
        eliminarColeccionBoton.style.display = 'block'
        //Secciones
        actualizarColeccionSeccion.style.display = 'flex'
        crearColeccionSeccion.style.display = 'none'
        eliminarColeccionSeccion.style.display = 'none'
    })
}
actualizarColeccionButton();


//Funcion eliminar colecciones
function eliminarColeccionButton() {
    //Listas
    const listaColeccionesResultados = document.getElementById('listaColecciones')
    const cartasColeccion = document.getElementById('cartasColeccion')
    //Botones
    const eliminarColeccionBoton = document.getElementById('eliminarColeccionButton')
    const verTodasColeccionBoton = document.getElementById('todasColeccionesButton')
    const crearColeccionBoton = document.getElementById('crearColeccionButton')
    const actualizarColeccionBoton = document.getElementById('actualizarColeccionButton')
    //Secciones
    const eliminarColeccionSeccion = document.getElementById('eliminarColeccion')
    const crearColeccionSeccion = document.getElementById('crearColeccion')
    const actualizarColeccionSeccion = document.getElementById('actualizarColeccion')
    eliminarColeccionBoton.addEventListener('click', () =>{
        //Listas
        listaColeccionesResultados.style.display = 'none'
        cartasColeccion.style.display = 'none'
        cartasColeccion.innerHTML = ''
        //Botones
        eliminarColeccionBoton.style.display = 'none'
        verTodasColeccionBoton.style.display = 'block'
        crearColeccionBoton.style.display = 'block'
        actualizarColeccionBoton.style.display = 'block'
        //Secciones
        eliminarColeccionSeccion.style.display = 'flex'
        crearColeccionSeccion.style.display = 'none'
        actualizarColeccionSeccion.style.display = 'none'
    })
}
eliminarColeccionButton();

// FIN COLECCIONES
// FIN COLECCIONES
// FIN COLECCIONES


// CARTAS
// CARTAS
// CARTAS
function visibleSeccionartas() {
    const botonColecciones = document.getElementById('cartas-buttonHome')
    botonColecciones.addEventListener('click', () => {
        const seccionCartas = document.getElementById('cartas')
        seccionCartas.style.display = 'flex'
        seccionCartas.style.flexDirection = 'row'
        const body = document.getElementsByTagName('body')
        body[0].style.backgroundImage = "url('./src/backgroundCartas.jpg')"
        
        const botonCartas = document.getElementById('cartas-buttonHome')
        botonCartas.style.display = 'none'
        
        const botonColecciones = document.getElementById('colecciones-buttonHome')
        botonColecciones.style.display = 'none'

        const botonSobres = document.getElementById('sobres-buttonHome')
        botonSobres.style.display = 'none';
    })
}
visibleSeccionartas();

// Funcion crear cartas
function crearCartasButton() {
    const crearCartasSeccion = document.getElementById('crearCarta')
    const actualizarCartasSeccion = document.getElementById('actualizarCarta')
    const eliminarCartasSeccion = document.getElementById('eliminarCarta')

    const botonCrearCarta = document.getElementById('crearCartaButton')
    const botonActualizarCarta = document.getElementById('actualizarCartaButton')
    const botonEliminarrCarta = document.getElementById('eliminarCartaButton')
    
    botonCrearCarta.addEventListener('click', () => {
        //Botones
        botonCrearCarta.style.display = 'none'
        botonActualizarCarta.style.display = 'block'
        botonEliminarrCarta.style.display = 'block'

        //Secciones
        crearCartasSeccion.style.display = 'flex'
        actualizarCartasSeccion.style.display = 'none'
        eliminarCartasSeccion.style.display = 'none'
    })
}
crearCartasButton()


// Funcion actulizar cartas
function actualizarCartasButton() {
    const actualizarCartasSeccion = document.getElementById('actualizarCarta')
    const crearCartasSeccion = document.getElementById('crearCarta')
    const eliminarCartasSeccion = document.getElementById('eliminarCarta')

    const botonActualizarCarta = document.getElementById('actualizarCartaButton')
    const botonCrearCarta = document.getElementById('crearCartaButton')
    const botonEliminarrCarta = document.getElementById('eliminarCartaButton')
    
    botonActualizarCarta.addEventListener('click', () => {
        //Botones
        botonActualizarCarta.style.display = 'none'
        botonCrearCarta.style.display = 'block'
        botonEliminarrCarta.style.display = 'block'

        //Secciones
        actualizarCartasSeccion.style.display = 'flex'
        crearCartasSeccion.style.display = 'none'
        eliminarCartasSeccion.style.display = 'none'
    })
}
actualizarCartasButton()

// Funcion eliminar cartas
function eliminarCartasButton() {
    const eliminarCartasSeccion = document.getElementById('eliminarCarta')
    const crearCartasSeccion = document.getElementById('crearCarta')
    const actualizarCartasSeccion = document.getElementById('actualizarCarta')

    const botonEliminarrCarta = document.getElementById('eliminarCartaButton')
    const botonCrearCarta = document.getElementById('crearCartaButton')
    const botonActualizarCarta = document.getElementById('actualizarCartaButton')
    
    botonEliminarrCarta.addEventListener('click', () => {
        //Botones
        botonEliminarrCarta.style.display = 'none'
        botonCrearCarta.style.display = 'block'
        botonActualizarCarta.style.display = 'block'

        //Secciones
        eliminarCartasSeccion.style.display = 'flex'
        crearCartasSeccion.style.display = 'none'
        actualizarCartasSeccion.style.display = 'none'
    })
}
eliminarCartasButton()