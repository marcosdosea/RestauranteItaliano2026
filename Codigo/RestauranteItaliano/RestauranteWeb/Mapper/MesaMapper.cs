using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
{
    public class MesaMapper
    {
        static MesaMapper()
        {
            TypeAdapterConfig<Mesa, MesaViewModel>.NewConfig()
                .Map(destino => destino.NomeRestaurante,
                origem => origem.IdRestauranteNavigation != null ?
                origem.IdRestauranteNavigation.Nome : null);

            TypeAdapterConfig<MesaViewModel, Mesa>.NewConfig()
                .Map(destino => destino.Id, origem => origem.Id ?? 0)
                .Map(destino => destino.IdRestaurante, origem => origem.IdRestaurante ?? 0)
                .Ignore(destino => destino.IdRestauranteNavigation)
                .Ignore(destino => destino.Atendimentos);
        }

        public static MesaViewModel ToViewModel(Mesa mesa)
        {
            return mesa.Adapt<MesaViewModel>();
        }

        public static Mesa ToEntity(MesaViewModel mesaViewModel)
        {
            return mesaViewModel.Adapt<Mesa>();
        }

        public static IEnumerable<MesaViewModel> ToViewModel(IEnumerable<Mesa>? mesas)
        {
            return mesas?.Adapt<IEnumerable<MesaViewModel>>() ?? new List<MesaViewModel>();
        }

        public static IEnumerable<Mesa> ToEntity(IEnumerable<MesaViewModel>? mesasViewModel)
        {
            return mesasViewModel?.Adapt<IEnumerable<Mesa>>() ?? new List<Mesa>();
        }
    }
}
