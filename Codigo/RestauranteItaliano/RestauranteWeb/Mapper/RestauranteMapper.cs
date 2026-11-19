using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
{
    public class RestauranteMapper
    {
        static RestauranteMapper()
        {
            TypeAdapterConfig<Restaurante, RestauranteViewModel>.NewConfig()
                .Map(destino => destino.NomeTipoRestaurante, 
                origem => origem.IdTipoRestauranteNavigation != null ?
                origem.IdTipoRestauranteNavigation.Nome : null);

            TypeAdapterConfig<RestauranteViewModel, Restaurante>.NewConfig()
                .Map(destino => destino.Id, origem => origem.Id ?? 0)
                .Map(destino => destino.IdTipoRestaurante, origem => origem.IdTipoRestaurante ?? 0)
                .Ignore(destino => destino.Funcionarios)
                .Ignore(destino => destino.Itemcardapios)
                .Ignore(destino => destino.Mesas);
        }

        public static RestauranteViewModel ToViewModel(Restaurante restaurante)
        {
            return restaurante.Adapt<RestauranteViewModel>();
        }

        public static Restaurante ToEntity(RestauranteViewModel restauranteViewModel)
        {
            return restauranteViewModel.Adapt<Restaurante>();
        }

        public static IEnumerable<RestauranteViewModel> ToViewModel(IEnumerable<Restaurante>? restaurantes)
        {
            return restaurantes?.Adapt<IEnumerable<RestauranteViewModel>>() ?? new List<RestauranteViewModel>();
        }

        public static IEnumerable<Restaurante> ToEntity(IEnumerable<RestauranteViewModel>? restaurantesViewModel)
        {
            return restaurantesViewModel?.Adapt<IEnumerable<Restaurante>>() ?? new List<Restaurante>();
        }

    }
}
