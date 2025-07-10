import { handleLoginForm, logout, isAuthenticated, getUserName } from './auth.js';
import { verificarAcessoPagina, ajustarSidebar } from './roles.js';
import { carregarHistorico } from './historico.js';
import { carregarDashboard } from './dashboard.js';

document.addEventListener('DOMContentLoaded', () => {
    console.log("✅ DOM carregado");

    // Proteção de rotas básica (autenticação)
    const path = window.location.pathname;
    if (!isAuthenticated() && !path.endsWith('index.html') && path !== '/') {
        console.warn("⛔ Usuário não autenticado, redirecionando para login");
        window.location.href = 'index.html';
        return;
    }

    // Proteção de rotas por perfil
    verificarAcessoPagina();

    // Se estiver na index.html, configura o login
    if (path.endsWith('index.html') || path === '/') {
        console.log("📥 Configurando formulário de login");
        handleLoginForm();
        return;
    }

    // Exibe dados do usuário
    const emailElement = document.getElementById('userEmail');
    const welcomeElement = document.getElementById('welcomeMessage');

    if (emailElement) {
        emailElement.textContent = localStorage.getItem('userEmail');
    }

    if (welcomeElement) {
        welcomeElement.textContent = `Bem-vindo, ${getUserName()}`;
    }

    // Carrega conteúdo específico de cada página
    if (path.endsWith('historico.html')) {
        carregarHistorico();
    } else if (path.endsWith('dashboard.html')) {
        carregarDashboard();
    }

    // Ajusta menus visíveis
    ajustarSidebar();

    // Configura botão de logout
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', logout);
    }
});