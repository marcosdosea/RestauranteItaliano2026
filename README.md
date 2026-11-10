# 🍝 Restaurante Italiano

Sistema web de gestão de restaurantes, desenvolvido em **ASP.NET Core MVC** com **MySQL** como projeto de equipe do curso de Sistemas da Informação da **UFS**, sob orientação do prof. **Marcos Dósea**.

O sistema cobre o dia a dia de um restaurante: cadastros básicos (restaurantes, funcionários, cardápio, mesas e formas de pagamento) e a operação do salão (abrir o atendimento de uma mesa, registrar pedidos, receber pagamentos e fechar a conta).

---

## 📋 Funcionalidades

| Caso de uso | Descrição | Situação | Responsável |
|---|---|---|---|
| **CS01** – Manter Formas de Pagamento | Cadastra as formas aceitas para pagar um atendimento | ✅ Concluído | [José Matheus](https://github.com/jmatheus-dev) |
| **CS02** – Manter Tipos de Restaurante | Cadastra os tipos de restaurante (pizzaria, churrascaria, comida brasileira…) | ✅ Concluído | [luansnta](https://github.com/luansnta) |
| **CS03** – Manter Tipos de Funcionário | Cadastra os tipos de funcionário (garçom, cozinheiro, gerente…) | ✅ Concluído | [EduBarbosa01](https://github.com/EduBarbosa01) |
| **CS04** – Manter Grupos do Cardápio | Cadastra os grupos do cardápio (bebidas, entradas, sobremesas…) | ✅ Concluído | [KaiDark0](https://github.com/KaiDark0) |
| **CS05** – Manter Restaurante | Cadastra os restaurantes que usam a plataforma | ⏳ Pendente | — |
| **CS06** – Manter Funcionários | Cadastra os funcionários de cada restaurante | ⏳ Pendente | — |
| **CS07** – Manter Itens do Cardápio | Cadastra os itens oferecidos pelo restaurante | ✅ Concluído | [José Matheus](https://github.com/jmatheus-dev) |
| **CS08** – Manter Mesas do Restaurante | Cadastra as mesas gerenciadas pelo restaurante | ⏳ Pendente | — |
| **CS09** – Iniciar Atendimento | Abre o atendimento de uma mesa e registra os pedidos | ⏳ Pendente | — |
| **CS10** – Manter Pedido | Registra os itens pedidos durante o atendimento | ⏳ Pendente | — |
| **CS11** – Fazer Pagamento de um Atendimento | Registra pagamentos parciais ou totais da conta | ⏳ Pendente | — |
| **CS12** – Finalizar o Atendimento | Encerra o atendimento depois que a conta é paga | ⏳ Pendente | — |

> Os requisitos completos estão em [`Requisitos/`](Requisitos) e o modelo do banco em [`AnaliseProjeto/`](AnaliseProjeto).

---

## 🛠️ Tecnologias

- [.NET 10](https://dotnet.microsoft.com/) e ASP.NET Core MVC
- [Entity Framework Core](https://learn.microsoft.com/ef/core/) com o provedor `MySql.EntityFrameworkCore`
- [MySQL 8](https://dev.mysql.com/) e MySQL Workbench
- [Mapster](https://github.com/MapsterMapper/Mapster) para converter entidades em modelos de tela
- Bootstrap 5 e jQuery Validation (validação no navegador)

---

## 🏛️ Arquitetura

A solução `RestauranteItaliano.slnx` é dividida em camadas, seguindo o modelo visto em aula:

```
Codigo/RestauranteItaliano
├── Core            → entidades do banco (scaffold), RestauranteContext e interfaces dos serviços
├── Service         → regras de negócio (implementação das interfaces do Core)
├── Util            → atributos de validação reutilizáveis (CPF, CEP, telefones)
└── RestauranteWeb  → aplicação MVC: Controllers, Models (ViewModels), Mapper e Views
```

Como uma funcionalidade é organizada (exemplo do CS07):

| Camada | Arquivo | Papel |
|---|---|---|
| Core | `Itemcardapio.cs`, `Service/IItemCardapioService.cs` | Entidade e contrato do serviço |
| Service | `ItemCardapioService.cs` | Regras de negócio; lança `ServiceException` quando uma regra é violada |
| RestauranteWeb | `Models/ItemCardapioModel.cs` | Campos da tela e validações (`[Required]`, `[StringLength]`…) |
| RestauranteWeb | `Mapper/ItemCardapioMapper.cs` | Conversões que o Mapster não faz sozinho |
| RestauranteWeb | `Controllers/ItemCardapioController.cs` | Recebe as requisições e mostra os erros do serviço na tela |
| RestauranteWeb | `Views/ItemCardapio/*.cshtml` | Index, Create, Edit, Details e Delete |

Os serviços são registrados no `Program.cs` com `AddScoped`, e o sistema usa a cultura **pt-BR** (valores em R$ e decimais com vírgula).

---

## 🚀 Como executar

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) com a carga de trabalho **ASP.NET e desenvolvimento Web** (ou VS Code com C# Dev Kit)
- [MySQL Server 8](https://dev.mysql.com/downloads/mysql/) rodando na porta `3306`

### 1. Clone o repositório

```bash
git clone https://github.com/marcosdosea/RestauranteItaliano2026.git
```

### 2. Crie o banco de dados

No MySQL Workbench, abra e execute o script [`AnaliseProjeto/dump-criacao.sql`](AnaliseProjeto/dump-criacao.sql). Ele cria o banco `restaurante` com todas as tabelas.

Em seguida, execute este ajuste: no script, o `id` da tabela `FormaPagamento` está sem `AUTO_INCREMENT`, e sem ele o cadastro de formas de pagamento não salva.

```sql
SET FOREIGN_KEY_CHECKS = 0;
ALTER TABLE restaurante.FormaPagamento MODIFY id INT UNSIGNED NOT NULL AUTO_INCREMENT;
SET FOREIGN_KEY_CHECKS = 1;
```

### 3. Confira a conexão

A string de conexão fica em `Codigo/RestauranteItaliano/RestauranteWeb/appsettings.json`:

```json
"ConnectionStrings": {
  "RestauranteConnection": "server=localhost;port=3306;user=root;password=123456;database=Restaurante"
}
```

Se o seu MySQL usa outro usuário ou senha, ajuste aqui (sem fazer commit da sua senha pessoal).

### 4. Rode a aplicação

**Visual Studio:** abra `Codigo/RestauranteItaliano/RestauranteItaliano.slnx`, defina **RestauranteWeb** como projeto de inicialização e clique em ▶ **https**. Na primeira execução, aceite o certificado de desenvolvimento.

**Linha de comando:**

```bash
cd Codigo/RestauranteItaliano/RestauranteWeb
dotnet run
```

### Dados para testar

Enquanto o CS05 (Restaurantes) não estiver pronto, cadastre um restaurante direto no banco para poder criar itens do cardápio:

```sql
INSERT INTO restaurante.TipoRestaurante (nome) VALUES ('Italiano');
INSERT INTO restaurante.restaurante (idTipoRestaurante, cnpj, nome)
VALUES (LAST_INSERT_ID(), '00000000000000', 'Restaurante Italiano');
```

---

## 🤝 Como contribuir

1. Atualize a `main`: `git checkout main && git pull`
2. Crie uma branch para o caso de uso: `git checkout -b cs05-restaurantes`
3. Siga o padrão das funcionalidades já prontas (veja o CS04 – Grupos do Cardápio como referência):
   - regras de negócio no **Service**, lançando `ServiceException` com mensagens claras em português;
   - validações de tela no **Model**, com mensagens no formato `"O campo {0} é de preenchimento obrigatório."`;
   - registro do serviço no `Program.cs` e link no menu (`Views/Shared/_Layout.cshtml`).
4. Compile e teste antes de enviar.
5. Faça commits no padrão `feat: implementa CS05 - Manter Restaurante`.
6. Envie a branch e abra um **Pull Request** para a `main`, descrevendo o que mudou.

---

## 👥 Equipe

| | Integrante |
|---|---|
| Orientador | [Marcos Dósea](https://github.com/marcosdosea) |
| Desenvolvimento | [José Matheus](https://github.com/jmatheus-dev) |
| Desenvolvimento | [luansnta](https://github.com/luansnta) |
| Desenvolvimento | [EduBarbosa01](https://github.com/EduBarbosa01) |
| Desenvolvimento | [KaiDark0](https://github.com/KaiDark0) |

---

## 📄 Licença

Distribuído sob a licença **MIT**. Veja o arquivo [`LICENSE`](LICENSE).
