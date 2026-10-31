namespace Core.Service
{
    public interface IItemCardapioService
    {
        uint Create(Itemcardapio itemCardapio);
        void Edit(Itemcardapio itemCardapio);
        void Delete(int id);
        Itemcardapio Get(int id);
        IEnumerable<Itemcardapio> GetAll();
    }
}
