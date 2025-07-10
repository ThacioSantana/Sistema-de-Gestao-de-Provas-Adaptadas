import { getToken } from './auth.js';

document.addEventListener('DOMContentLoaded', async () => {
    const token = getToken();

    const form = document.getElementById('provaForm');

    const selectGroup = document.createElement('div');
    selectGroup.classList.add('form-group');

    const label = document.createElement('label');
    label.htmlFor = 'selectChamado';
    label.textContent = 'Selecione um chamado pendente:';

    const selectChamado = document.createElement('select');
    selectChamado.id = 'selectChamado';
    selectChamado.required = true;
    selectChamado.classList.add('form-control'); // caso queira uma classe adicional

    selectGroup.appendChild(label);
    selectGroup.appendChild(selectChamado);

// Insere no topo do formulário
    form.insertBefore(selectGroup, form.firstChild);

    const alunoInput = document.getElementById('aluno');
    const matriculaInput = document.getElementById('matricula');
    const observacoesInput = document.getElementById('observacoes');

    let chamados = [];

    try {
        const res = await fetch('/api/Chamado/pendentes-prova', {
            headers: { Authorization: `Bearer ${token}` }
        });
        if (!res.ok) throw new Error('Erro ao buscar chamados pendentes');
        chamados = await res.json();

        chamados.forEach(c => {
            const opt = document.createElement('option');
            opt.value = c.chamadoId;
            opt.textContent = `${c.alunoNome} - ${c.cursoNome}`;
            selectChamado.appendChild(opt);
        });

        // Evento: ao selecionar um chamado, preenche os campos
        selectChamado.addEventListener('change', () => {
            const chamadoSelecionado = chamados.find(c => c.chamadoId == selectChamado.value);
            if (chamadoSelecionado) {
                alunoInput.value = chamadoSelecionado.alunoNome;
                matriculaInput.value = chamadoSelecionado.alunoMatricula;
                observacoesInput.value = chamadoSelecionado.descricao;
            }
        });

    } catch (err) {
        console.error('Erro ao carregar chamados:', err);
    }

    // Enviar prova
    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const chamadoId = selectChamado.value;
        const arquivo = document.getElementById('arquivoProva').files[0];
        if (!arquivo) return alert('Selecione um arquivo');

        const formData = new FormData();
        formData.append('arquivo', arquivo);

        try {
            const res = await fetch(`/api/Chamado/enviar-prova/${chamadoId}`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}` },
                body: formData
            });

            if (!res.ok) throw new Error(await res.text());
            alert('Prova enviada com sucesso!');
            window.location.reload();

        } catch (err) {
            alert('Erro ao enviar prova: ' + err.message);
        }
    });
});
