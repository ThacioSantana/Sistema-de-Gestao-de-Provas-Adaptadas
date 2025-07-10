import { login as loginApi } from './api.js';

export function logout() {
    localStorage.clear();
    window.location.href = '/index.html';
}

export function isAuthenticated() {
    return !!localStorage.getItem('token');
}

export function getToken() {
    return localStorage.getItem('token');
}

export function getUserType() {
    return localStorage.getItem('userType');
}

export function getUserName() {
    return localStorage.getItem('userName');
}

export function saveLoginData(data) {
    localStorage.setItem('isAuthenticated', 'true');
    localStorage.setItem('token', data.token);
    localStorage.setItem('userType', data.perfil.toLowerCase());
    localStorage.setItem('userEmail', data.matricula);
    localStorage.setItem('userName', data.nome);
}

export function handleLoginForm() {
    const loginForm = document.getElementById('loginForm');
    if (!loginForm) return;

    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const loginBtn = e.target.querySelector('button[type="submit"]');
        const originalText = loginBtn.textContent;

        try {
            loginBtn.disabled = true;
            loginBtn.textContent = 'Autenticando...';

            const matricula = document.getElementById('matricula').value;
            const senha = document.getElementById('senha').value;

            const data = await loginApi(matricula, senha);

            if (!data || !data.token || !data.perfil) {
                throw new Error('Resposta inválida do servidor');
            }

            saveLoginData(data);
            window.location.href = '/html/dashboard.html';

        } catch (err) {
            console.error('Erro no login:', err);
            alert(err.message || 'Erro no login. Verifique suas credenciais e tente novamente.');
        } finally {
            loginBtn.disabled = false;
            loginBtn.textContent = originalText;
        }
    });
}