import { getToken, getUserType } from './auth.js';
import { isModoAvaliacao, getChamadoIdFromUrl } from './roles.js';
import {
    getChamadoPorId,
    avaliarChamado,
    criarChamado,
    getCursos,
    enviarAnexos,
    getDownloadProvaUrl,
    getAlunoPorMatricula
} from './api.js';

document.addEventListener('DOMContentLoaded', async () => {
    const userType = getUserType();
    if (userType?.toLowerCase() !== 'naa') {
        window.location.href = 'dashboard.html';
        return;
    }

    await carregarCursos();

    document.getElementById('alunoMatricula').addEventListener('blur', async () => {
        const matricula = document.getElementById('alunoMatricula').value.trim();
        if (!matricula) return;

        try {
            const aluno = await getAlunoPorMatricula(getToken(), matricula);

            document.getElementById('alunoNome').value = aluno.alunoNome;
            document.getElementById('alunoEmail').value = aluno.alunoEmail;

            const cursoSelect = document.getElementById('curso');
            cursoSelect.innerHTML = `<option value="${aluno.cursoId}" selected>${aluno.cursoNome}</option>`;
            cursoSelect.disabled = true;

            document.getElementById('coordenadorNome').value = aluno.coordenadorNome;
            document.getElementById('coordenadorEmail').value = aluno.coordenadorEmail;

        } catch (error) {
            console.error('Erro ao buscar aluno:', error);
            alert('Aluno não encontrado ou erro na API.');
        }
    });

    const form = document.getElementById('formChamado');
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const chamadoData = montarDadosDoChamado(form);

        try {
            const chamadoCriado = await criarChamado(getToken(), chamadoData);
            const arquivos = form.anexos.files;
            if (arquivos.length > 0) {
                await enviarAnexos(getToken(), chamadoCriado.id, arquivos);
            }

            exibirMensagemSucesso();
            form.reset();
        } catch (error) {
            console.error('❌ Erro ao criar chamado:', error);
            alert('Erro ao criar chamado: ' + error.message);
        }
    });

    if (isModoAvaliacao()) {
        await initAvaliacao();
    }
});

function montarDadosDoChamado(form) {
    return {
        AlunoMatricula: form.alunoMatricula.value,
        AlunoNome: form.alunoNome.value,
        AlunoEmail: form.alunoEmail.value,
        CursoId: parseInt(form.curso.value),
        TipoDeficiencia: form.tipoDeficiencia.value,
        Descricao: form.descricao.value,
        CoordenadorNome: form.coordenadorNome?.value,
        CoordenadorEmail: form.coordenadorEmail?.value
    };
}

function exibirMensagemSucesso() {
    const mensagem = document.getElementById('mensagemSucesso');
    mensagem.style.display = 'block';
    setTimeout(() => {
        mensagem.style.display = 'none';
    }, 5000);
}

async function carregarCursos() {
    try {
        const cursos = await getCursos(getToken());
        const select = document.getElementById('curso');

        cursos.forEach(curso => {
            const option = document.createElement('option');
            option.value = curso.id;
            option.textContent = curso.nome;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar cursos:', error);
    }
}

async function initAvaliacao() {
    const chamadoId = getChamadoIdFromUrl();
    const token = getToken();
    console.log('🔍 Modo Avaliação Ativo');
    console.log('🔍 ID do chamado:', chamadoId);
    console.log('🔍 Token:', token);

    try {
        const chamado = await getChamadoPorId(token, chamadoId);
        console.log('✅ Chamado carregado com sucesso:', chamado);

        preencherFormularioChamado(chamado);
        configurarDownload(chamadoId, chamado);
        configurarBotoesAvaliacao(chamadoId);
    } catch (error) {
        console.error('❌ Erro ao carregar os dados do chamado:', error);
        alert('Erro ao carregar os dados do chamado.');
    }
}

function preencherFormularioChamado(chamado) {
    const campos = {
        alunoMatricula: chamado.matriculaAluno,
        alunoNome: chamado.nomeAluno,
        alunoEmail: chamado.emailAluno,
        coordenadorNome: chamado.nomeCoordenador,
        coordenadorEmail: chamado.emailCoordenador,
        descricao: chamado.descricao,
    };

    Object.entries(campos).forEach(([id, value]) => {
        const el = document.getElementById(id);
        if (el) {
            el.value = value;
            el.setAttribute('readonly', 'true');
        }
    });

    document.getElementById('curso').innerHTML = `<option selected>${chamado.cursoNome}</option>`;
    document.getElementById('curso').disabled = true;
}

function configurarDownload(chamadoId, chamado) {
    const btn = document.getElementById('btnDownload');
    if (!btn) return;
    if (chamado.provaArquivo) {
        btn.style.display = 'inline-block';
        btn.href = getDownloadProvaUrl(chamadoId);
    } else {
        btn.style.display = 'none';
    }
}

function configurarBotoesAvaliacao(chamadoId) {
    const acoes = document.querySelector('.form-actions');
    if (!acoes) return;

    acoes.innerHTML = `
        <button class="btn-submit" id="btnAprovar">✅ Aprovar</button>
        <button class="btn-submit" id="btnReprovar" style="background-color: #d9534f;">❌ Reprovar</button>
        <button class="btn-submit" id="btnConcluir" style="background-color: #5cb85c;">✅ Encerrar</button>
    `;

    document.getElementById('btnAprovar').addEventListener('click', () => validarChamado('Aprovado', chamadoId));
    document.getElementById('btnReprovar').addEventListener('click', () => validarChamado('Reprovado', chamadoId));
    document.getElementById('btnConcluir').addEventListener('click', () => validarChamado('Concluído', chamadoId));
}

async function validarChamado(resultado, chamadoId) {
    const token = getToken();
    try {
        const res = await fetch(`/api/Chamado/validar-prova/${chamadoId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify(resultado)
        });

        if (!res.ok) throw new Error(await res.text());
        alert(`Chamado ${resultado.toLowerCase()} com sucesso!`);
        window.location.href = 'historico.html';
    } catch (err) {
        alert('Erro ao validar: ' + err.message);
        console.error(err);
    }
}