namespace Core.Service
{
    public interface IItemCardapioService
    {
        uint Create(Itemcardapio itemCardapio);
        void Edit(Itemcardapio itemCardapio);
        void Delete(uint id);
        Itemcardapio? Get(uint id);
        IEnumerable<Itemcardapio> GetAll();
    }
}
