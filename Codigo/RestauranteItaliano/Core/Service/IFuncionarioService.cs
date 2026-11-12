using Core.Datatables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IFuncionarioService
    {
        uint Create(Funcionario funcionario);
        void Edit(Funcionario funcionario);
        void Delete(uint id);
        Funcionario? Get(uint id);
        IEnumerable<Funcionario> GetAll();
        IEnumerable<Funcionario> GetByNome(string nome);
        DatatableResponse<Funcionario> GetDataPage(DatatableRequest request);

    }
}
