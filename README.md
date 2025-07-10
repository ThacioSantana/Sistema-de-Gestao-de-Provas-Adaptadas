# 📚 Sistema de Gestão de Provas Adaptadas

Projeto acadêmico desenvolvido em parceria com o setor NPA/NAA para automatizar o processo de solicitação e acompanhamento de provas adaptadas destinadas a alunos com necessidades específicas (como autismo, baixa visão, entre outras).

## 🚀 Objetivo

Substituir o processo manual realizado via e-mail por um sistema centralizado e automatizado, trazendo mais **transparência**, **organização** e **eficiência** para alunos, responsáveis, coordenadores de curso e o setor pedagógico.

---

## ⚙️ Funcionalidades

- 📌 **Abertura de chamado**: Registro de solicitação pelo NPA/NAA com envio automático de e-mails para os envolvidos.
- 📩 **Notificações em tempo real**: Cada etapa do fluxo envia atualizações por e-mail (ex: envio da prova, aprovação ou reprovação).
- 📂 **Gestão de provas adaptadas**: Coordenadores podem enviar provas, que passam por análise e podem ser reprovadas e reenviadas.
- 🔒 **Controle de acesso**: Diferenciação entre perfis (coordenador, NPA/NAA, aluno/responsável).

> ⚠️ **Nota:** A funcionalidade de envio de e-mails ainda não foi testada, pois não foi possível configurar um provedor gratuito durante o desenvolvimento.

---

## 🛠 Tecnologias Utilizadas

| Camada        | Tecnologias                      |
|---------------|----------------------------------|
| Back-end      | C# (.NET Core)                   |
| Front-end     | HTML, CSS, JavaScript            |
| Banco de Dados| PostgreSQL                       |

---

## 👥 Equipe

- **Thacio Santana** – Desenvolvimento do back-end (C#)
- **Alexandre Dantas** – Desenvolvimento do front-end (HTML, CSS, JS)

---

## 📦 Como Executar o Projeto

> Requisitos:
- .NET SDK 6.0 ou superior
- PostgreSQL
- Visual Studio ou VS Code

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/nome-do-repositorio.git
