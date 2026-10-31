namespace Core.Service
{
    public interface ITipoRestauranteService
    {
        int Create(Tiporestaurante tipoRestaurante);
        void Edit(Tiporestaurante tipoRestaurante);
        void Delete(int id);
        Tiporestaurante Get(int id);
        IEnumerable<Tiporestaurante> GetAll();
    }
}
