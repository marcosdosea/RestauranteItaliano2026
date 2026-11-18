# 🍝 Restaurante Italiano

Sistema web de gestão de restaurantes, desenvolvido em **ASP.NET Core MVC** com **MySQL** como projeto de equipe do curso de Sistemas da Informação da **UFS**, sob orientação do prof. **Marcos Dósea**.

O sistema cobre o dia a dia de um restaurante: cadastros básicos (restaurantes, funcionários, cardápio, mesas e formas de pagamento) e a operação do salão (abrir o atendimento de uma mesa, registrar pedidos, receber pagamentos e fechar a conta).

Além do site, uma **Web API** publica o cardápio e as formas de pagamento para aplicações externas, como um aplicativo de celular.

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

## 🎨 Interface

O visual do sistema, chamado **La Carta · Trattoria**, imita o cardápio impresso de uma trattoria: papel de algodão, títulos com serifa, linhas pontilhadas ligando o nome ao preço e detalhes em vermelho pomodoro.

- **Gravuras próprias:** as imagens do site são linogravuras em duas tintas (preto e vermelho), geradas para o projeto e vetorizadas em SVG (`wwwroot/images/gravuras`): uma para cada grupo do cardápio, o prato vazio e o erro. A foto do forno da página inicial vai em AVIF e WebP, com JPG de reserva, e o fundo é uma textura de papel (`wwwroot/images`).
- **Estampa de fundo:** na página inicial, gravuras de ingredientes (vinho, tomate, manjericão, alho, parmesão e ramos) ficam espalhadas pelo fundo em tinta bem clara, flutuam devagar e se deslocam com o mouse em profundidades diferentes (`wwwroot/js/estampa.js`). No celular aparecem menos peças, e para quem pediu menos movimento elas ficam paradas.
- **Cardápio em 3D:** na página inicial, o resumo do dia é um cardápio dobrado feito só com CSS 3D (`perspective` e `rotateY`). Ele abre na primeira visita e se inclina com o mouse (`wwwroot/js/movimento.js`); no celular e para quem pediu menos movimento ele fica plano e parado.
- **Fontes locais:** Instrument Serif e Instrument Sans são servidas pelo próprio site, registradas no `libman.json`.

- **Página inicial:** funciona como o índice de um livro. Mostra uma saudação conforme a hora do dia, atalhos para os cadastros mais usados, um resumo do cardápio (itens disponíveis e em falta e os grupos com mais itens) e os casos de uso prontos organizados em capítulos, com a quantidade de registros de cada um. Os que ainda não foram implementados aparecem numa única linha, no fim da página.
- **Navegação por capítulos:** o menu agrupa as telas prontas em capítulos (Cardápio, Equipe e Configurações). Salão, Pedidos e Pagamentos entram no menu quando tiverem a primeira tela pronta.
- **Avisos:** depois de salvar, editar ou excluir, aparece um aviso verde no canto da tela, que some sozinho. Erros aparecem em vermelho e ficam até serem fechados.
- **Cardápio:** os itens ficam numa tabela com [DataTables](https://datatables.net/), seguindo a orientação do professor para páginas com muitos registros que o usuário precisa consultar ou ordenar. A busca (por nome, detalhes ou grupo), o filtro por grupo, a ordenação por coluna e a paginação são feitos no servidor. Dá para marcar um item em falta direto na tabela.
- **Erros:** um filtro global (`Filter/CustomExceptionFilter.cs`) leva qualquer erro que escape dos controllers para a tela de erro, com uma mensagem em português. Erros do MySQL são traduzidos pelo número (por exemplo, 1451: registro vinculado a outros dados).
- **Celular:** todas as telas se adaptam a telas pequenas, e as ações ficam sempre visíveis em telas de toque.
- **Acessibilidade e validação:** HTML semântico, navegação por teclado, atributos `aria-*` e respeito à opção de reduzir animações. As páginas passam sem erros nos validadores de [HTML](https://validator.w3.org/nu/) e de [CSS](https://jigsaw.w3.org/css-validator/) do W3C.

O estilo fica todo em `RestauranteWeb/wwwroot/css/site.css`, com variáveis de cor em português (`--cor-tinta`, `--cor-papel`…) e classes no padrão BEM em português (`linha-carta__nome`, `aviso--erro`): cada elemento fica dentro do próprio bloco, e o CSS é dividido em seções, uma por bloco. O layout único das páginas é `Views/Shared/_Layout.cshtml`.

Os ícones são desenhados uma única vez em `Views/Shared/_Icones.cshtml`. Cada tela usa um deles com `<svg class="icone" aria-hidden="true"><use href="#icone-voltar"></use></svg>`, e o tamanho, a cor e o traço vêm do CSS, sem `width`, `height` nem `style` no HTML.

---

## 🛠️ Tecnologias

- [.NET 10](https://dotnet.microsoft.com/) e ASP.NET Core MVC
- [Entity Framework Core](https://learn.microsoft.com/ef/core/) com o provedor `MySql.EntityFrameworkCore`
- [MySQL 8](https://dev.mysql.com/) e MySQL Workbench
- [Mapster](https://github.com/MapsterMapper/Mapster) para converter entidades em modelos de tela
- Bootstrap 5 e jQuery Validation (validação no navegador)
- [DataTables](https://datatables.net/) 3.1.5 nas listas com muitos registros (busca, ordenação e paginação no servidor)
- [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) (Swashbuckle) para documentar e testar a Web API
- [MSTest](https://learn.microsoft.com/dotnet/core/testing/unit-testing-mstest-intro), [Moq](https://github.com/devlooped/moq) e o provedor **InMemory** do Entity Framework Core nos testes unitários

---

## 🏛️ Arquitetura

A solução `RestauranteItaliano.slnx` é dividida em camadas, seguindo o modelo visto em aula:

```
Codigo/RestauranteItaliano
├── Core               → entidades do banco (scaffold), RestauranteContext, interfaces dos serviços e DTOs
├── Service            → regras de negócio (implementação das interfaces do Core)
├── ServiceTest        → testes unitários dos serviços (MSTest + banco em memória)
├── Util               → atributos de validação reutilizáveis (CPF, CEP, telefones)
├── RestauranteWeb     → aplicação MVC: Controllers, Models (ViewModels), Mapper e Views
├── RestauranteWebTest → testes unitários dos controllers (MSTest + Moq)
├── RestauranteAPI     → Web API: Controllers, Models, Mappers e filtro de erros
└── RestauranteAPITest → testes unitários da Web API (MSTest + Moq)
```

Como uma funcionalidade é organizada (exemplo do CS07):

| Camada | Arquivo | Papel |
|---|---|---|
| Core | `Itemcardapio.cs`, `Service/IItemCardapioService.cs` | Entidade e contrato do serviço |
| Service | `ItemCardapioService.cs` | Regras de negócio; lança `ServiceException` quando uma regra é violada |
| ServiceTest | `ItemCardapioServiceTests.cs` | Testa as regras do serviço em um banco em memória |
| RestauranteWeb | `Models/ItemCardapioViewModel.cs` | ViewModel: campos da tela e validações (`[Required]`, `[StringLength]`…) |
| RestauranteWeb | `Mapper/ItemCardapioMapper.cs` | Converte entidade em modelo de tela e vice-versa (`ToViewModel` e `ToEntity`) |
| RestauranteWeb | `Controllers/ItemCardapioController.cs` | Recebe as requisições e mostra os erros do serviço na tela |
| RestauranteWeb | `Views/ItemCardapio/*.cshtml` | Index, Create, Edit, Details e Delete |
| RestauranteWebTest | `Controllers/ItemCardapioControllerTests.cs` | Testa o controller com o serviço simulado (Moq) |

Os serviços são registrados no `Program.cs` com `AddTransient`, e o sistema usa a cultura **pt-BR** (valores em R$ e decimais com vírgula).

Consultas que juntam dados de várias tabelas devolvem um **DTO** (pasta `Core/DTO`). É o caso do resumo da página inicial: o `ResumoService` faz as contagens no banco e devolve um `ResumoDTO`.

Listas com muitos registros usam o **DataTables** com processamento no servidor, como no projeto de referência:

| Camada | Arquivo | Papel |
|---|---|---|
| Core | `Datatables/DatatableRequest.cs`, `Datatables/DatatableResponse.cs` | Pedido do DataTables (página, busca, ordenação) e resposta (itens da página e totais) |
| Service | `ItemCardapioService.GetDataPage` | Aplica busca, filtro, ordenação e paginação na consulta (`Count`, `Skip` e `Take` no banco) |
| RestauranteWeb | `ItemCardapioController.GetDataPage` | Action `[HttpPost]` chamada pela tabela; devolve a página em JSON |
| RestauranteWeb | `Views/ItemCardapio/Index.cshtml`, `wwwroot/js/itens-cardapio.js` | Monta a tabela e liga a busca e o filtro por grupo |
| RestauranteWeb | `wwwroot/lib/datatables.net/` | Biblioteca DataTables e a tradução `pt-BR.json` (registrada no `libman.json`) |

### Web API

O projeto `RestauranteAPI` segue o modelo da aula de Web API: controladores `[ApiController]` que herdam de `ControllerBase` e usam os **mesmos services** do site, sem repetir regra de negócio. As respostas são em JSON, com `200 OK`, `201 Created`, `400 Bad Request` ou `404 Not Found`, e erros do banco podem devolver `409` ou `500`.

| Método | Rota | O que faz |
|---|---|---|
| GET | `/api/FormasPagamento` | Lista as formas de pagamento |
| GET | `/api/FormasPagamento/{id}` | Busca uma forma de pagamento |
| POST | `/api/FormasPagamento` | Cadastra uma forma de pagamento |
| PUT | `/api/FormasPagamento/{id}` | Edita uma forma de pagamento |
| DELETE | `/api/FormasPagamento/{id}` | Exclui uma forma de pagamento |
| GET | `/api/ItensCardapio` | Lista os itens do cardápio |
| GET | `/api/ItensCardapio/{id}` | Busca um item |
| POST | `/api/ItensCardapio` | Cadastra um item |
| PUT | `/api/ItensCardapio/{id}` | Edita um item |
| PUT | `/api/ItensCardapio/{id}/disponibilidade` | Marca o item em falta ou disponível |
| DELETE | `/api/ItensCardapio/{id}` | Exclui um item |

O filtro `Filter/HttpResponseExceptionFilter.cs` transforma os erros em respostas com mensagem em português: regra violada no serviço (`ServiceException`) vira `400`, e os erros do MySQL viram `409` (registro vinculado ou repetido), `400` (chave estrangeira inválida) ou `500`. O arquivo `RestauranteAPI.http` traz exemplos prontos de cada chamada.

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

Se você criou o banco com uma versão antiga do script, execute também este ajuste. Nela, o `id` da tabela `FormaPagamento` estava sem `AUTO_INCREMENT`, e sem ele o cadastro de formas de pagamento não salva. Quem cria o banco com o script atual não precisa fazer nada.

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

### 5. Rode a Web API (opcional)

**Visual Studio:** defina **RestauranteAPI** como projeto de inicialização e clique em ▶ **https**. O navegador abre o **Swagger** em `https://localhost:7300/swagger`, onde dá para testar cada rota.

**Linha de comando:**

```bash
cd Codigo/RestauranteItaliano/RestauranteAPI
dotnet run
```

A API usa a mesma string de conexão, em `RestauranteAPI/appsettings.json`, e por linha de comando fica em `http://localhost:5160/swagger`.

### Dados para testar a aplicação

Enquanto o CS05 (Restaurantes) não estiver pronto, cadastre um restaurante direto no banco para poder criar itens do cardápio:

```sql
INSERT INTO restaurante.TipoRestaurante (nome) VALUES ('Italiano');
INSERT INTO restaurante.restaurante (idTipoRestaurante, cnpj, nome)
VALUES (LAST_INSERT_ID(), '00000000000000', 'Restaurante Italiano');
```

Para ver a página inicial e o cardápio com conteúdo, você pode incluir também alguns grupos, itens e formas de pagamento (rode depois do script acima):

```sql
SET @restaurante = (SELECT MAX(id) FROM restaurante.restaurante);

INSERT INTO restaurante.GrupoCardapio (nome) VALUES ('Entradas');
SET @entradas = LAST_INSERT_ID();
INSERT INTO restaurante.GrupoCardapio (nome) VALUES ('Massas');
SET @massas = LAST_INSERT_ID();
INSERT INTO restaurante.GrupoCardapio (nome) VALUES ('Sobremesas');
SET @sobremesas = LAST_INSERT_ID();

INSERT INTO restaurante.itemcardapio (nome, preco, detalhes, ativo, disponivel, idRestaurante, idGrupoCardapio) VALUES
  ('Bruschetta al pomodoro', 28.00, 'pão italiano, tomate e manjericão', 1, '1', @restaurante, @entradas),
  ('Spaghetti alla carbonara', 62.00, 'guanciale, ovos e pecorino', 1, '1', @restaurante, @massas),
  ('Gnocchi al pesto', 54.00, NULL, 1, '0', @restaurante, @massas),
  ('Tiramisù', 26.00, 'mascarpone, café e cacau', 1, '1', @restaurante, @sobremesas);

INSERT INTO restaurante.FormaPagamento (nome) VALUES ('Dinheiro'), ('Pix'), ('Cartão de crédito');
```

---

## 🧪 Testes

A solução tem três projetos de teste, no mesmo formato do projeto de referência da disciplina:

| Projeto | O que testa | Como |
|---|---|---|
| `ServiceTest` | Regras de negócio dos serviços | Banco em memória (`Microsoft.EntityFrameworkCore.InMemory`), criado do zero a cada teste |
| `RestauranteWebTest` | Controllers do site | Serviços simulados com Moq |
| `RestauranteAPITest` | Controllers da Web API | Serviços simulados com Moq |

Os testes não usam o MySQL, então rodam mesmo sem o banco instalado.

Situação atual: **135 testes, todos aprovados**.

| Arquivo | Testes | O que cobre |
|---|---|---|
| `ServiceTest/FormaPagamentoServiceTests.cs` | 13 | Cadastro, edição, exclusão e consultas; nome repetido; exclusão bloqueada quando a forma já foi usada em um pagamento |
| `ServiceTest/ItemCardapioServiceTests.cs` | 27 | Cadastro, edição, exclusão e consultas; grupo ou restaurante inexistente; nome repetido no mesmo restaurante; item já pedido; marcar em falta e disponível; página do DataTables (busca, filtro por grupo, ordenação e paginação) |
| `ServiceTest/ResumoServiceTests.cs` | 4 | Números da página inicial: itens ativos, disponíveis e em falta, grupos com mais itens e quantidade de cada cadastro |
| `RestauranteWebTest/Controllers/FormaPagamentoControllerTests.cs` | 17 | Todas as ações do controller, avisos de sucesso, erros do serviço e páginas inexistentes (404) |
| `RestauranteWebTest/Controllers/ItemCardapioControllerTests.cs` | 21 | Todas as ações do controller, incluindo a página em JSON para o DataTables, listas do formulário, aviso de erro ao marcar em falta e páginas inexistentes (404) |
| `RestauranteWebTest/Controllers/HomeControllerTests.cs` | 3 | Resumo da página inicial e funcionamento com o banco fora do ar |
| `RestauranteWebTest/Filter/CustomExceptionFilterTests.cs` | 8 | Tela de erro com a mensagem certa para erro inesperado, regra de negócio e erros do MySQL |
| `RestauranteWebTest/Models/GravurasTests.cs` | 18 | Gravura certa para cada grupo do cardápio, pelas palavras-chave do nome |
| `RestauranteAPITest/Controllers/FormasPagamentoControllerTests.cs` | 9 | Todas as rotas: listagem, busca, cadastro (201), edição, exclusão, dados inválidos (400) e registro inexistente (404) |
| `RestauranteAPITest/Controllers/ItensCardapioControllerTests.cs` | 11 | Todas as rotas, incluindo marcar em falta ou disponível |
| `RestauranteAPITest/Filter/HttpResponseExceptionFilterTests.cs` | 4 | Resposta 400 com a mensagem da regra de negócio, status próprio de `HttpResponseException` e erros inesperados sem tratamento |

**Visual Studio:** menu **Teste → Executar Todos os Testes** (ou `Ctrl+R, A`). O resultado aparece no **Gerenciador de Testes**.

**Linha de comando:**

```bash
cd Codigo/RestauranteItaliano
dotnet test
```

Os métodos seguem o nome `Metodo_QuandoCondicao_DeveResultado` e são divididos em `// Arrange`, `// Act` e `// Assert`.

---

## 🤝 Como contribuir

1. Atualize a `main`: `git checkout main && git pull`
2. Crie uma branch para o caso de uso: `git checkout -b cs05-restaurantes`
3. Siga o padrão das funcionalidades já prontas (veja o CS07 – Itens do Cardápio como referência):
   - regras de negócio no **Service**, lançando `ServiceException` com mensagens claras em português;
   - o `GetAll` do serviço devolve a consulta com `AsNoTracking()`, sem `ToList()`;
   - validações de tela na **ViewModel** (`Models/NomeViewModel.cs`), com mensagens no formato `"O campo {0} é de preenchimento obrigatório."`;
   - conversão entre entidade e modelo de tela em uma classe da pasta **Mapper**, com `ToViewModel` e `ToEntity`;
   - registro do serviço no `Program.cs` com `AddTransient` e entrada no mapa do sistema (`Models/Navegacao.cs`), que alimenta o menu e a página inicial;
   - em listas com muitos registros que o usuário precisa consultar ou ordenar, use o DataTables com `GetDataPage` (veja o CS07);
   - testes do serviço em `ServiceTest` e do controller em `RestauranteWebTest`.
4. Compile e rode os testes antes de enviar.
5. Antes do commit, confira na lista de alterações que não há arquivos das pastas `bin/` e `obj/`. Elas são geradas pela compilação e já estão no `.gitignore`.
6. Faça commits no padrão `feat: implementa CS05 - Manter Restaurante`.
7. Envie a branch e abra um **Pull Request** para a `main`, descrevendo o que mudou.

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
