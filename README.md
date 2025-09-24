# ProjectManager MVC (ASP.NET Core 8 + Identity + MySQL)

Aplicação **ASP.NET Core MVC** com **Identity** (login e papéis) e **Entity Framework Core** (MySQL via **Pomelo**).
Permite **gerenciar usuários, projetos, equipes e tarefas**, com **regras de autorização por papel**:

- **Admin**: gerencia tudo (Usuários, Projetos, Equipes, Tarefas).
- **Manager**: gerencia **Projetos**, **Equipes** e **Tarefas**.
- **Collaborator**: **não cria/edita** cadastros; **pode apenas atualizar o status das próprias tarefas**.

---

## 1) O que o sistema faz

- **Autenticação** com e‑mail/senha (Cookie) e **autorização por papéis** (policies e roles do Identity).
- **Usuários** (Admin): CRUD + definição de papel (Admin/Manager/Collaborator).
- **Projetos** (Admin/Manager): CRUD, com **Gerente** (usuário) obrigatório e **status** (`Planned`, `InProgress`, `Done`, `Cancelled`).
- **Equipes** (Admin/Manager): CRUD + **vínculo de membros (usuários)** e **vínculo de projetos** (N:N).
- **Tarefas**: criadas por Admin/Manager; **Collaborator** altera **apenas o próprio** `Status` (ex.: `InProgress`, `Done`, etc.).
- **UI** com Bootstrap 5: layout simples, responsivo, com navbar e footer sticky.

---

## 2) Tecnologias e como se comunicam

- **ASP.NET Core 8 (MVC)**: roteia requisições → `Controllers` → `Services` → `DbContext`.
- **Identity**: autenticação (Login/Logout), usuários, papéis e políticas. Usa tabelas `AspNet*`.
- **EF Core + Pomelo MySQL**: mapeamento objeto‑relacional; gera e aplica **Migrations** e executa comandos no MySQL.
- **DI (Injeção de dependência)**: `Services` registrados em `Program.cs` (`IUserService`, `IProjectService`, `ITeamService`, `ITaskService`).

Fluxo (alto nível):

```
Browser → Middleware (Auth Cookie) → Controller → Service → ApplicationDbContext → MySQL
                                      ↓
                                    View (Razor) → HTML + Bootstrap
```

- **Cookies** guardam o estado autenticado do usuário.  
- **Policies** (`RequireAdmin`, `RequireManagerOrAdmin`) aplicam acesso por ação/controlador.

---

## 3) Estrutura de pastas (simplificada)

```
ProjectManagerMvc/
├─ Controllers/
│  ├─ AccountController.cs        (Login/Logout)
│  ├─ HomeController.cs
│  ├─ ProjectsController.cs       (Admin/Manager)
│  ├─ TasksController.cs          (Admin/Manager CRUD; Collaborator: UpdateMyStatus)
│  └─ TeamsController.cs          (Admin/Manager)
├─ Data/
│  ├─ ApplicationDbContext.cs     (DbSets, relacionamentos)
│  └─ SeedData.cs                 (cria roles e usuário admin)
├─ Models/
│  ├─ ApplicationUser.cs          (IdentityUser estendido: FullName, CPF, Position)
│  ├─ Enums.cs                    (ProjectStatus, TaskStatus)
│  ├─ Project.cs, Team.cs, TaskItem.cs
│  └─ JoinEntities.cs             (N:N: ProjectTeam, TeamUser)
├─ Services/
│  ├─ (I)UserService.cs
│  ├─ (I)ProjectService.cs
│  ├─ (I)TeamService.cs
│  └─ (I)TaskService.cs
├─ ViewModels/
│  ├─ LoginViewModel.cs
│  └─ UserViewModels.cs           (Create/Edit/List)
├─ Views/
│  ├─ Shared/_Layout.cshtml       (Navbar, footer sticky, Bootstrap)
│  ├─ Account/Login.cshtml
│  ├─ Home/Index.cshtml
│  ├─ Users/...                   (Admin)
│  ├─ Projects/...                (Admin/Manager)
│  ├─ Teams/...                   (Admin/Manager, vínculos)
│  └─ Tasks/...                   (lista, CRUD, UpdateMyStatus)
├─ appsettings.json               (ConnectionStrings, Seed)
├─ Program.cs                     (DI, Identity, Policies, EF MySQL, Migrate + Seed)
└─ ProjectManagerMvc.csproj       (referências NuGet)
```

**Relacionamentos principais**:
- `Project` 1:N `TaskItem`
- `Project` N:N `Team` via `ProjectTeam`
- `Team` N:N `ApplicationUser` via `TeamUser`
- `TaskItem` → `Assignee` (usuário) opcional
- `Project` → `Manager` (usuário) obrigatório

---

## 4) Pré‑requisitos

- **.NET SDK 8.0** (LTS)
- **MySQL Server 8.x** (porta padrão 3306)
- **Visual Studio** (workload “ASP.NET e desenvolvimento web”) ou CLI `.NET`
- (Opcional) **dotnet‑ef** para CLI de migrations:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 5) Configuração e Execução (passo a passo)

1. **Ajuste a connection string** em `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Port=3306;Database=ProjectManagerDb;User=root;Password=YourStrongPassword!;TreatTinyAsBoolean=true;"
   }
   ```
   - Se a porta não for 3306, altere `Port`.
   - Se usa root sem senha, deixe `Password=` vazio.
   - Se precisar, adicione `;SslMode=Preferred`.

2. **Gerar e aplicar migrations** (uma das opções):
   - **Visual Studio / Package Manager Console**:
     ```powershell
     Add-Migration InitialCreate
     Update-Database
     ```
   - **CLI**:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

3. **Seed (usuário admin)**  
   O `Program.cs` chama `SeedData.InitializeAsync()` ao subir a app.  
   Configure em `appsettings.json`:
   ```json
   "Seed": {
     "AdminEmail": "admin@local",
     "AdminPassword": "Admin@12345!"
   }
   ```
   > A senha deve atender às políticas do Identity (≥8, 1 maiúscula, 1 dígito, 1 símbolo).

4. **Executar**  
   - **Visual Studio**: F5 (ProjectManagerMvc como startup).  
   - **CLI**:
     ```bash
     dotnet run
     ```
   Acesse `https://localhost:7161` e faça login.

---

## 6) Fluxo de uso recomendado

1. Entre como **Admin** (`admin@local` / `Admin@12345!`).  
2. **Usuários** → **Novo**: crie **Managers** e **Collaborators** (defina a Role).  
3. **Projetos**: crie e selecione um **Gerente** (Manager/Admin).  
4. **Equipes**: crie; em **Detalhes**, **adicione membros** e **vincule projetos**.  
5. **Tarefas**: crie e atribua ao colaborador.  
6. Faça **logout** e entre como **Collaborator** → em **Tarefas**, altere **apenas o status das suas tarefas**.

---

## 7) Políticas de Acesso (resumo)

- `UsersController`: `[Authorize(Policy = "RequireAdmin")]`
- `ProjectsController` / `TeamsController`: `[Authorize(Policy = "RequireManagerOrAdmin")]`
- `TasksController`:
  - Listar/criar/editar/excluir: Manager/Admin
  - `UpdateMyStatus`: `Roles = "Collaborator,Manager,Admin"` → só altera **se for o próprio responsável**.

---

## 8) Troubleshooting

- **Erro de senha do seed**: ajuste `AdminPassword` (ex.: `Admin@12345!`).  
- **Acesso ao MySQL falhou**: verifique `User/Password/Port`; teste no MySQL Workbench.  
- **Porta diferente**: acrescente `;Port=3307` (ou a sua).  
- **Conflito `TaskStatus`**: qualifique com `ProjectManagerMvc.Models.TaskStatus`.  
- **SSL/TLS**: se necessário, `;SslMode=Preferred` na connection string.  
- **Migrations ausentes**: gere `Add-Migration InitialCreate` e rode `Update-Database`.

---

## 9) Personalizações rápidas

- **Política de senha** (`Program.cs` → `AddIdentity`):
  ```csharp
  options.Password.RequiredLength = 8;
  options.Password.RequireDigit = true;
  options.Password.RequireUppercase = true;
  options.Password.RequireNonAlphanumeric = true;
  ```
- **Seed**: altere e‑mail/senha no `appsettings.json`.
- **UI**: edite `Views/Shared/_Layout.cshtml` (navbar/footer), e as views em cada área.

---

## 10) Licença / Uso
Projeto didático, pronto para servir como base em trabalhos acadêmicos e provas técnicas.  
Sugestões de melhoria: paginação, validações de CPF, logs de auditoria, anexos em tarefas.
