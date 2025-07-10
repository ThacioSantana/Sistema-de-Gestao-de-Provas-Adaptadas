import { getToken, getUserType } from './auth.js';

export async function carregarDashboard() {
    if (!window.location.pathname.endsWith('dashboard.html')) return;

    try {
        const token = getToken();
        const userType = getUserType();

        // Mostrar/ocultar itens do menu conforme o tipo de usuário
        if (userType === 'coordenador') {
            document.getElementById('menuEnviarProva').style.display = 'block';
        } else {
            document.getElementById('menuEnviarProva').style.display = 'none';
        }

        let todos = [];

        // 🔄 Escolhe a API de acordo com o perfil
        if (userType === 'coordenador') {
            const res = await fetch('/api/Chamado/pendentes-prova', {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (!res.ok) throw new Error('Erro ao buscar chamados pendentes');
            todos = await res.json();

        } else {
            const res = await fetch('/api/Chamado/todos', {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (!res.ok) throw new Error('Erro ao buscar todos os chamados');
            todos = await res.json();
        }

        // 🔢 Contagem dos chamados
        const provasPendentes = todos.filter(c =>
            c.status === 'Criado' || c.status === 'Prova enviada'
        ).length;

        const provasAvaliadas = todos.filter(c =>
            c.status === 'Aprovado' || c.status === 'Reprovado' || c.status === 'Concluído'
        ).length;

        // 🖥️ Atualiza os números na tela
        document.getElementById('provasPendentes').textContent = provasPendentes;
        document.getElementById('provasAvaliadas').textContent = provasAvaliadas;

        // 🎯 Mostra/oculta cards conforme perfil
        if (userType === 'coordenador') {
            document.getElementById('cardProvasAvaliadas').style.display = 'none';
        } else {
            document.getElementById('cardProvasPendentes').style.display = 'none';
        }

    } catch (err) {
        console.error('Erro ao carregar dashboard:', err);
    }
}

// 🚀 Executa a função
carregarDashboard();