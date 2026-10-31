namespace Core.Service
{
    public interface ITipoFuncionario
    {
        int Create(Tipofuncionario tipoFuncionario);
        void Edit(Tipofuncionario tipoFuncionario);
        void Delete(int id);
        Tipofuncionario Get(int id);
        IEnumerable<Tipofuncionario> GetAll();
    }
}
