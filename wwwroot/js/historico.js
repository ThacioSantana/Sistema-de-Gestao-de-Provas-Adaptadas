import { getToken, getUserType } from './auth.js';

export function carregarHistorico() {
    const userType = getUserType();
    if (userType !== 'coordenador') {
        const menuEnviarProva = document.getElementById('menuEnviarProva');
        if (menuEnviarProva) {
            menuEnviarProva.style.display = 'none';
        }
    }

    const container = document.getElementById('listaChamados');
    const inputBusca = document.getElementById('filtroBusca');
    const selectStatus = document.getElementById('filtroStatus');
    if (!container) return;

    let todosChamados = [];

    const renderChamados = (filtroTexto = '', filtroStatus = '') => {
        const chamadosFiltrados = todosChamados.filter(c => {
            const textoMatch = (
                c.nomeAluno?.toLowerCase().includes(filtroTexto.toLowerCase()) ||
                c.matriculaAluno?.toLowerCase().includes(filtroTexto.toLowerCase())
            );
            const statusMatch = filtroStatus === '' || c.status.trim().toLowerCase() === filtroStatus.trim().toLowerCase();
            return textoMatch && statusMatch;
        });

        container.innerHTML = '';

        if (chamadosFiltrados.length === 0) {
            container.innerHTML = '<p>Nenhum chamado encontrado.</p>';
            return;
        }

        chamadosFiltrados.forEach(chamado => {
            const item = document.createElement('div');
            item.className = 'chamado-item';

            const statusClass = {
                'finalizado': 'status-finalizado',
                'pendente': 'status-pendente',
                'aberto': 'status-aberto',
                'criado': 'status-aberto',
                'prova enviada': 'status-pendente',
                'aprovado': 'status-finalizado',
                'reprovado': 'status-reprovado'
            }[chamado.status.trim().toLowerCase()] || 'status-aberto';

            item.innerHTML = `
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <p><strong>Matrícula:</strong> ${chamado.matriculaAluno}</p>
                    <p><strong>Nome:</strong> ${chamado.nomeAluno}</p>
                    <p><strong>Data:</strong> ${new Date(chamado.dataAbertura).toLocaleDateString('pt-BR')}</p>
                </div>
                <div>
                    <button class="btn btn-submit btn-ver-detalhes" data-id="${chamado.id}" style="margin-top: 1rem; padding: 0.4rem 0.8rem; font-size: 0.9rem;">🔍 Ver Detalhes</button>
                </div>
            </div>
            <span class="status-indicador ${statusClass}">${chamado.status}</span>
        `;

            container.appendChild(item);
        });

        document.querySelectorAll('.btn-ver-detalhes').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.getAttribute('data-id');
                const chamado = todosChamados.find(c => c.id == id);
                const modal = document.getElementById('modalDetalhes');
                const conteudo = document.getElementById('conteudoModal');

                conteudo.innerHTML = `
                <p><strong>Nome do Aluno:</strong> ${chamado.nomeAluno}</p>
                <p><strong>Matrícula:</strong> ${chamado.matriculaAluno}</p>
                <p><strong>Status:</strong> ${chamado.status}</p>
                <p><strong>Data de Abertura:</strong> ${new Date(chamado.dataAbertura).toLocaleDateString('pt-BR')}</p>
                ${chamado.provaArquivo ? `<p><strong>Arquivo:</strong> <a href="/api/Chamado/download-prova/${chamado.id}" class="btn btn-download" style="color: #fff;" target="_blank">📎 Baixar Prova</a></p>` : ''}
            `;

                // Só mostra botão de avaliação se for NPA e status for "Prova enviada"
                if (chamado.status === "Prova enviada" && getUserType()?.toLowerCase() === "naa") {
                    conteudo.innerHTML += `
                    <button class="btn btn-submit" id="btnAvaliarChamado" style="margin-top: 1rem;">✏️ Avaliar Chamado</button>
                `;

                    setTimeout(() => {
                        document.getElementById('btnAvaliarChamado')?.addEventListener('click', () => {
                            window.location.href = `avaliar-prova.html?avaliar=true&id=${chamado.id}`;
                        });
                    }, 0);
                }

                modal.style.display = 'flex';
            });
        });
    };

    fetch('/api/Chamado/todos', {
        headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
        }
    })
        .then(res => {
            if (!res.ok) throw new Error('Erro ao buscar chamados');
            return res.json();
        })
        .then(data => {
            todosChamados = data;
            renderChamados();
        })
        .catch(err => {
            container.innerHTML = '<p>Erro ao carregar histórico.</p>';
            console.error(err);
        });

    inputBusca?.addEventListener('input', () => {
        renderChamados(inputBusca.value, selectStatus.value);
    });

    selectStatus?.addEventListener('change', () => {
        renderChamados(inputBusca.value, selectStatus.value);
    });

    document.getElementById('fecharModal')?.addEventListener('click', () => {
        document.getElementById('modalDetalhes').style.display = 'none';
    });

    window.addEventListener('click', (event) => {
        const modal = document.getElementById('modalDetalhes');
        if (event.target === modal) modal.style.display = 'none';
    });
}