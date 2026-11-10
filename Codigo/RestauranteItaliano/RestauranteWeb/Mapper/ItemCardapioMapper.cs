using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
{
    /// <summary>
    /// Regras de conversão entre Itemcardapio e ItemCardapioModel que o Mapster não faz sozinho:
    /// no banco, "ativo" é TINYINT e "disponivel" é VARCHAR ('1' ou '0'); na tela, os dois são caixas de seleção.
    /// </summary>
    public static class ItemCardapioMapper
    {
        public static void Registrar(TypeAdapterConfig config)
        {
            config.NewConfig<Itemcardapio, ItemCardapioModel>()
                .Map(destino => destino.Ativo, origem => origem.Ativo == 1)
                .Map(destino => destino.Disponivel, origem => origem.Disponivel == "1")
                .Map(destino => destino.NomeRestaurante,
                     origem => origem.IdRestauranteNavigation != null ? origem.IdRestauranteNavigation.Nome : null)
                .Map(destino => destino.NomeGrupoCardapio,
                     origem => origem.IdGrupoCardapioNavigation != null ? origem.IdGrupoCardapioNavigation.Nome : null);

            config.NewConfig<ItemCardapioModel, Itemcardapio>()
                .Map(destino => destino.Id, origem => origem.Id ?? 0)
                .Map(destino => destino.Ativo, origem => origem.Ativo ? (sbyte)1 : (sbyte)0)
                .Map(destino => destino.Disponivel, origem => origem.Disponivel ? "1" : "0")
                .Map(destino => destino.IdRestaurante, origem => origem.IdRestaurante ?? 0)
                .Map(destino => destino.IdGrupoCardapio, origem => origem.IdGrupoCardapio ?? 0)
                .Ignore(destino => destino.IdRestauranteNavigation)
                .Ignore(destino => destino.IdGrupoCardapioNavigation)
                .Ignore(destino => destino.Pedidoitemcardapios);
        }
    }
}
