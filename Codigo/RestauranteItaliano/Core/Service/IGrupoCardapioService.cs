namespace Core.Service
{
    public interface IGrupoCardapioService
    {
        uint Create(Grupocardapio grupoCardapio);
        void Edit(Grupocardapio grupoCardapio);
        void Delete(uint id);
        Grupocardapio Get(uint id);
        IEnumerable<Grupocardapio> GetAll();
    }
}
