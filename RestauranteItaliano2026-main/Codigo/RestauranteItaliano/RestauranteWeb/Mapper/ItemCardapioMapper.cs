using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
{
    public class ItemCardapioMapper
    {
        /// <summary>
        /// Regras de conversão que o Mapster não faz sozinho: no banco, "ativo" é TINYINT
        /// e "disponivel" é VARCHAR ('1' ou '0'); na tela, os dois são caixas de seleção.
        /// O construtor estático roda uma única vez, antes do primeiro uso da classe.
        /// </summary>
        static ItemCardapioMapper()
        {
            TypeAdapterConfig<Itemcardapio, ItemCardapioViewModel>.NewConfig()
                .Map(destino => destino.Ativo, origem => origem.Ativo == 1)
                .Map(destino => destino.Disponivel, origem => origem.Disponivel == "1")
                .Map(destino => destino.NomeRestaurante,
                     origem => origem.IdRestauranteNavigation != null ? origem.IdRestauranteNavigation.Nome : null)
                .Map(destino => destino.NomeGrupoCardapio,
                     origem => origem.IdGrupoCardapioNavigation != null ? origem.IdGrupoCardapioNavigation.Nome : null);

            TypeAdapterConfig<ItemCardapioViewModel, Itemcardapio>.NewConfig()
                .Map(destino => destino.Id, origem => origem.Id ?? 0)
                .Map(destino => destino.Ativo, origem => origem.Ativo ? (sbyte)1 : (sbyte)0)
                .Map(destino => destino.Disponivel, origem => origem.Disponivel ? "1" : "0")
                .Map(destino => destino.IdRestaurante, origem => origem.IdRestaurante ?? 0)
                .Map(destino => destino.IdGrupoCardapio, origem => origem.IdGrupoCardapio ?? 0)
                .Ignore(destino => destino.IdRestauranteNavigation)
                .Ignore(destino => destino.IdGrupoCardapioNavigation)
                .Ignore(destino => destino.Pedidoitemcardapios);
        }

        public static ItemCardapioViewModel ToViewModel(Itemcardapio itemCardapio)
        {
            return itemCardapio.Adapt<ItemCardapioViewModel>();
        }

        public static Itemcardapio ToEntity(ItemCardapioViewModel itemCardapioViewModel)
        {
            return itemCardapioViewModel.Adapt<Itemcardapio>();
        }

        public static IEnumerable<ItemCardapioViewModel> ToViewModel(IEnumerable<Itemcardapio>? itensCardapio)
        {
            return itensCardapio?.Adapt<IEnumerable<ItemCardapioViewModel>>() ?? new List<ItemCardapioViewModel>();
        }

        public static IEnumerable<Itemcardapio> ToEntity(IEnumerable<ItemCardapioViewModel>? itensCardapioViewModel)
        {
            return itensCardapioViewModel?.Adapt<IEnumerable<Itemcardapio>>() ?? new List<Itemcardapio>();
        }
    }
}
