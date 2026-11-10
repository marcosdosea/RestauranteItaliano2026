- Cria ItemCardapioService com CRUD e regras de negócio:
  - não permite nome repetido no mesmo restaurante
  - valida se o grupo do cardápio e o restaurante existem
  - não permite excluir item já usado em pedidos (orienta a desativar)
- Cria RestauranteService mínimo (Get e GetAll) para listar restaurantes;
  Create/Edit/Delete ficam para o CS05 (NotImplementedException)
- Cria ItemCardapioModel, ItemCardapioMapper (ativo TINYINT e disponivel '1'/'0' <-> bool),
  ItemCardapioController e as views Index, Create, Edit, Details e Delete
- Ajusta IItemCardapioService para usar uint e Get retornar Itemcardapio?
- Registra os services e o mapper no Program.cs e adiciona "Itens do Cardápio" no menu

obs: o Program.cs passa a usar a cultura pt-BR em todo o sistema,
para aceitar preços com vírgula (ex.: 29,90) e exibir valores em R$
