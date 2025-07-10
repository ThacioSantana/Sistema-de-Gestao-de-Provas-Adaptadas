import { getUserType } from './auth.js';

const regrasAcesso = {
    'enviar-prova.html': 'coordenador',
    'avaliar-prova.html': 'naa'
};

export function ajustarSidebar() {
    const tipo = getUserType()?.toLowerCase();
    console.log("🔧 Ajustando sidebar para:", tipo);

    const menuItems = {
        menuDashboard: true,
        menuEnviarProva: tipo === 'coordenador',
        menuAvaliarProva: tipo === 'naa',
        menuHistorico: true
    };

    Object.entries(menuItems).forEach(([id, visivel]) => {
        const item = document.getElementById(id);
        if (item) {
            item.parentElement.style.display = visivel ? 'block' : 'none';
        }
    });
}

export function verificarAcessoPagina() {
    const tipo = getUserType()?.toLowerCase();
    const pagina = window.location.pathname.split('/').pop();

    const roleNecessario = regrasAcesso[pagina];

    if (roleNecessario && tipo !== roleNecessario) {
        window.location.href = 'dashboard.html';
    }
}

export function isModoAvaliacao() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.has('avaliar') && urlParams.get('avaliar') === 'true';
}

export function getChamadoIdFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
}