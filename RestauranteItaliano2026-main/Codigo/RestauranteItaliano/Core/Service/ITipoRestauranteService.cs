namespace Core.Service
{
    public interface ITipoRestauranteService
    {
        uint Create(Tiporestaurante tipoRestaurante);
        void Edit(Tiporestaurante tipoRestaurante);
        void Delete(uint id);
        Tiporestaurante? Get(uint id);
        IEnumerable<Tiporestaurante> GetAll();
    }
}
