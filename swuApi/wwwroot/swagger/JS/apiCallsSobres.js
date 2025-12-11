function mostrarCartasAbiertas(cartas, collectionData) { 
    const contenedor = document.getElementById("resultadoPack");
    
    if (!cartas || cartas.length === 0) {
        contenedor.innerHTML = "<p>No se generaron cartas.</p>";
        return;
    }

    const ordenRareza = {
        "Common": 1,
        "Uncommon": 2,
        "Rare": 3,
        "Legendary": 4,
        "Showcase": 5
    };

    cartas.sort((a, b) => ordenRareza[a.rarity] - ordenRareza[b.rarity]);
    
    const totalCollectionCards = collectionData.numCards || '?';

    // Genera el HTML
    contenedor.innerHTML = `
        <h3 style="margin-left: 100px;">Cartas obtenidas:</h3>
        <div class="cards-grid">
            ${cartas.map(c => {
                let styles = '';
                const defaultBackground = 'linear-gradient(180deg, #3b3fb6 80%, #1e2170 100%)';
                
                let background = defaultBackground; // Start with the default background
                let filter = '';

                if (c.model === 'Foil') {
                    filter = 'brightness(1.2)';
                }

                if (c.model === 'Hyperspace') {
                    background = 'linear-gradient(180deg, #222228 80%, #222228 100%)';
                }
                
                if (c.model === 'Hyperspace Foil') {
                    // Hyperspace Foil sobrescribe el fondo y añade brillo
                    background = 'linear-gradient(180deg, #222228 80%, #222228 100%)';
                    filter = 'brightness(1.2)';
                }

                if (c.model === 'Showcase') {
                    // Showcase sobrescribe el fondo y añade brillo
                    background = 'linear-gradient(180deg, #a5a5bcff 80%, #a5a5bcff 100%)';
                    filter = 'brightness(1.2)';
                }
                
                // Construye el string de estilos finales
                styles += `background: ${background};`;
                if (filter) styles += `filter: ${filter};`;

                // Devuelve el HTML de la carta para el mapeo
                return `
                    <div class="carta-item" style="${styles}"> 
                        <div style='background-color: #222228; border-radius: 15px; width: -webkit-fill-available; padding: 10px;'>
                            <p style='background-color: #f0f0f0; font-weight: 800; color: #000000; margin-bottom: 10px;'>${c.cardName}</p>
                            <p style='border-radius: 15px;'>${c.subtitle ? `<span>${c.subtitle}</span>` : ''}</p>
                        </div>
                        <div style='background-color: #222228; border-radius: 15px; width: -webkit-fill-available; padding: 10px; margin-top: 10px;'>
                            <p style='background-color: #f0f0f0; color: #000000;'><span>Copias:</span> ${c.copies || 1}</p>
                            <p>${c.model}</p>
                            <p>${c.cardNumber || '?'} / ${totalCollectionCards}</p> 
                        </div>
                    </div>
                `;
            }).join("")}
        </div>
    `;
}

// --- El EVENT LISTENER (Se mantiene igual) ---
const API_PACK_OPEN = "http://localhost:8309/open/";

document.getElementById("abrirPackBtn").addEventListener("click", async () => {
    const packId = document.getElementById("packIdInput").value;
    const resultadoDiv = document.getElementById("resultadoPack");

    if (!packId) {
        resultadoDiv.innerHTML = "<p style='color:#b33939'>Debes introducir un ID de pack</p>";
        return;
    }

    try {
        const resPack = await fetch(API_PACK_OPEN + packId, {
            method: "POST"
        });

        if (!resPack.ok) {
            const msg = await resPack.text();
            resultadoDiv.innerHTML = `<p style='color:#b33939'>Error al abrir el pack: ${msg}</p>`;
            return;
        }

        const cartas = await resPack.json();
        
        if (cartas.length === 0) {
            mostrarCartasAbiertas([]);
            return;
        }

        // 2. OBTENER EL ID DE LA COLECCIÓN
        const collectionId = cartas[0].collectionId; // Asumimos que todas las cartas son de la misma colección
        
        // 3. OBTENER LOS DATOS DE LA COLECCIÓN (para NumCards)
        const resCollection = await fetch(`${API_COLECCION}/${collectionId}`);
        let collectionData = { numCards: '?' }; // Valor por defecto si falla la llamada
        
        if (resCollection.ok) {
            collectionData = await resCollection.json();
        }

        // 4. LLAMAR A LA FUNCIÓN DE RENDERIZADO, PASANDO LA COLECCIÓN
        mostrarCartasAbiertas(cartas, collectionData);

    } catch (err) {
        resultadoDiv.innerHTML = `<p style='color:#b33939'>Error de conexión al servidor: ${err.message}</p>`;
    }
});