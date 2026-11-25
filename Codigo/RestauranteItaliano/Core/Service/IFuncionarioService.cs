namespace Core.Service
{
    public interface IFuncionarioService
    {
        int Create(Funcionario funcionario);
        void Edit(Funcionario funcionario);
        void Delete(int id);
        Funcionario Get(int id);
        IEnumerable<Funcionario> GetAll();
        IEnumerable<Funcionario> GetByNome(string nome);
        IEnumerable<Funcionario> GetGarconsPorRestaurante(uint idRestaurante);
    }
}
