namespace Core.Service
{
    public interface ITipoFuncionario
    {
        uint Create(Tipofuncionario tipoFuncionario);
        void Edit(Tipofuncionario tipoFuncionario);
        void Delete(uint id);
        Tipofuncionario? Get(uint id);
        IEnumerable<Tipofuncionario> GetAll();
    }
}
