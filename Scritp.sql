-- =============================
-- CRIAÇÃO DO BANCO E TABELAS
-- =============================

-- 1) Criar banco de dados
CREATE DATABASE sistema_chamados_naa;

-- 2) Criar tabelas

CREATE TABLE coordenadores (
                               id SERIAL PRIMARY KEY,
                               matricula VARCHAR(50) NOT NULL,
                               senha VARCHAR(255) NOT NULL,
                               nome VARCHAR(255) NOT NULL,
                               email VARCHAR(255) NOT NULL,
                               perfil VARCHAR(50) NOT NULL
);

CREATE TABLE usuarios_npa (
                              id SERIAL PRIMARY KEY,
                              matricula VARCHAR(50) NOT NULL,
                              senha VARCHAR(255) NOT NULL,
                              nome VARCHAR(255) NOT NULL,
                              email VARCHAR(255) NOT NULL,
                              perfil VARCHAR(50) NOT NULL
);

CREATE TABLE cursos (
                        id SERIAL PRIMARY KEY,
                        nome VARCHAR(255) NOT NULL,
                        coordenador_id INT NOT NULL REFERENCES coordenadores(id)
);

CREATE TABLE alunos (
                        id SERIAL PRIMARY KEY,
                        nome VARCHAR(255) NOT NULL,
                        email VARCHAR(255) NOT NULL,
                        matricula VARCHAR(50) NOT NULL,
                        tipo_deficiencia VARCHAR(100) NOT NULL,
                        curso_id INT NOT NULL REFERENCES cursos(id)
);

CREATE TABLE chamados (
                          id SERIAL PRIMARY KEY,
                          aluno_id INT NOT NULL REFERENCES alunos(id),
                          curso_id INT NOT NULL REFERENCES cursos(id),
                          coordenador_id INT NOT NULL REFERENCES coordenadores(id),
                          npa_id INT REFERENCES usuarios_npa(id),
                          descricao TEXT NOT NULL,
                          data_abertura TIMESTAMP NOT NULL DEFAULT NOW(),
                          status VARCHAR(50) NOT NULL
);

CREATE TABLE provas_adaptadas (
                                  id SERIAL PRIMARY KEY,
                                  chamado_id INT NOT NULL REFERENCES chamados(id) ON DELETE CASCADE,
                                  arquivo_prova VARCHAR(500),
                                  data_envio TIMESTAMP DEFAULT NOW(),
                                  status VARCHAR(50),
                                  observacoes TEXT
);

CREATE TABLE chamado_logs (
                              id SERIAL PRIMARY KEY,
                              chamado_id INT NOT NULL REFERENCES chamados(id) ON DELETE CASCADE,
                              acao TEXT NOT NULL,
                              usuario VARCHAR(50) NOT NULL,
                              datahora TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE historico_status (
                                  id SERIAL PRIMARY KEY,
                                  chamado_id INT NOT NULL REFERENCES chamados(id) ON DELETE CASCADE,
                                  status_anterior VARCHAR(50),
                                  status_atual VARCHAR(50),
                                  data_alteracao TIMESTAMP NOT NULL DEFAULT NOW(),
                                  comentario TEXT
);

-- =============================
-- INSERÇÃO DE DADOS FALSOS
-- =============================

-- Coordenadores
INSERT INTO coordenadores (matricula, senha, nome, email, perfil) VALUES
                                                                      ('C001', '1234', 'Carlos Alberto', 'carlos.alberto@faculdade.com', 'Coordenador'),
                                                                      ('C002', '1234', 'Maria Fernanda', 'maria.fernanda@faculdade.com', 'Coordenador'),
                                                                      ('C003', '1234', 'João Batista', 'joao.batista@faculdade.com', 'Coordenador');

-- Usuários NAA/NPA
INSERT INTO usuarios_npa (matricula, senha, nome, email, perfil) VALUES
                                                                     ('N001', '1234', 'Ana Paula', 'ana.paula@faculdade.com', 'NAA'),
                                                                     ('N002', '1234', 'Roberto Silva', 'roberto.silva@faculdade.com', 'NAA');

-- Cursos
INSERT INTO cursos (nome, coordenador_id) VALUES
                                              ('Engenharia de Software', 1),
                                              ('Pedagogia', 2),
                                              ('Direito', 3),
                                              ('Ciências Contábeis', 1),
                                              ('Enfermagem', 2);

-- Alunos
INSERT INTO alunos (nome, email, matricula, tipo_deficiencia, curso_id) VALUES
                                                                            ('Lucas Andrade', 'lucas.andrade@alunos.com', 'A001', 'Visual', 1),
                                                                            ('Fernanda Costa', 'fernanda.costa@alunos.com', 'A002', 'Auditiva', 1),
                                                                            ('Bruno Souza', 'bruno.souza@alunos.com', 'A003', 'Motora', 2),
                                                                            ('Juliana Lima', 'juliana.lima@alunos.com', 'A004', 'Intelectual', 2),
                                                                            ('Rafael Santos', 'rafael.santos@alunos.com', 'A005', 'Outra', 3),
                                                                            ('Amanda Torres', 'amanda.torres@alunos.com', 'A006', 'Visual', 3),
                                                                            ('Pedro Henrique', 'pedro.henrique@alunos.com', 'A007', 'Auditiva', 4),
                                                                            ('Mariana Alves', 'mariana.alves@alunos.com', 'A008', 'Motora', 4),
                                                                            ('João Vitor', 'joao.vitor@alunos.com', 'A009', 'Intelectual', 5),
                                                                            ('Isabela Martins', 'isabela.martins@alunos.com', 'A010', 'Visual', 5),
                                                                            ('Gabriel Rocha', 'gabriel.rocha@alunos.com', 'A011', 'Auditiva', 1),
                                                                            ('Carolina Mendes', 'carolina.mendes@alunos.com', 'A012', 'Motora', 2),
                                                                            ('Felipe Oliveira', 'felipe.oliveira@alunos.com', 'A013', 'Intelectual', 3),
                                                                            ('Larissa Dias', 'larissa.dias@alunos.com', 'A014', 'Outra', 4),
                                                                            ('Thiago Barbosa', 'thiago.barbosa@alunos.com', 'A015', 'Visual', 5);

-- Chamados
INSERT INTO chamados (aluno_id, curso_id, coordenador_id, npa_id, descricao, data_abertura, status) VALUES
                                                                                                        (1, 1, 1, 1, 'Solicitação de prova ampliada', '2025-08-01 10:00:00', 'Criado'),
                                                                                                        (2, 1, 1, 1, 'Solicitação de intérprete de Libras', '2025-08-02 14:30:00', 'Prova enviada'),
                                                                                                        (3, 2, 2, 2, 'Solicitação de prova em braille', '2025-08-03 09:15:00', 'Aprovado'),
                                                                                                        (4, 2, 2, 2, 'Prova adaptada com leitura em áudio', '2025-08-04 11:45:00', 'Reprovado'),
                                                                                                        (5, 3, 3, 1, 'Solicitação de tempo adicional', '2025-08-05 15:20:00', 'Concluído');

-- Logs
INSERT INTO chamado_logs (chamado_id, acao, usuario, datahora) VALUES
                                                                   (1, 'Chamado criado para o aluno A001', 'NAA', '2025-08-01 10:01:00'),
                                                                   (2, 'Chamado criado para o aluno A002', 'NAA', '2025-08-02 14:31:00'),
                                                                   (3, 'Prova enviada pelo coordenador', 'Coordenador', '2025-08-03 10:00:00'),
                                                                   (3, 'Prova aprovada pelo NAA', 'NAA', '2025-08-03 11:00:00'),
                                                                   (4, 'Prova enviada pelo coordenador', 'Coordenador', '2025-08-04 12:00:00'),
                                                                   (4, 'Prova reprovada pelo NAA', 'NAA', '2025-08-04 13:00:00'),
                                                                   (5, 'Prova enviada pelo coordenador', 'Coordenador', '2025-08-05 15:30:00'),
                                                                   (5, 'Chamado concluído', 'NAA', '2025-08-05 16:00:00');
