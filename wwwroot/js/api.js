async function apiFetch(url, options = {}) {
    const res = await fetch(url, options);
    if (!res.ok) {
        const error = await res.json().catch(() => null);
        throw new Error(error?.mensagem ?? `Erro na requisição: ${res.status}`);
    }
    return await res.json();
}

export async function login(matricula, senha) {
    return apiFetch('/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ matricula, senha })
    });
}

export async function getTodosChamados(token) {
    return apiFetch('/api/Chamado/todos', {
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function getChamadosPendentes(token) {
    const todos = await getTodosChamados(token);
    return todos.filter(c => ['Criado', 'Prova enviada'].includes(c.status));
}

export async function getChamadosAvaliados(token) {
    const todos = await getTodosChamados(token);
    return todos.filter(c => ['Aprovado', 'Reprovado', 'Concluído'].includes(c.status));
}

export async function getChamadoPorId(token, chamadoId) {
    return apiFetch(`/api/Chamado/${chamadoId}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function avaliarChamado(token, chamadoId, data) {
    return apiFetch(`/api/Chamado/avaliar/${chamadoId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(data)
    });
}

export async function criarChamado(token, data) {
    return apiFetch('/api/Chamado/criar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(data)
    });
}

export async function getCursos(token) {
    return apiFetch('/api/Cursos', {
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function getChamadosPendentesProva(token) {
    return apiFetch('/api/Chamado/pendentes-prova', {
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function getAlunoPorMatricula(token, matricula) {
    return apiFetch(`/api/Chamado/aluno/${matricula}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function enviarAnexos(token, chamadoId, files) {
    const formData = new FormData();
    for (let file of files) {
        formData.append('arquivos', file);
    }
    const res = await fetch(`/api/Chamado/anexos/${chamadoId}`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
        body: formData
    });
    if (!res.ok) throw new Error('Erro ao enviar anexos');
}

export function getDownloadProvaUrl(chamadoId) {
    return `/api/Chamado/download-prova/${chamadoId}`;
}