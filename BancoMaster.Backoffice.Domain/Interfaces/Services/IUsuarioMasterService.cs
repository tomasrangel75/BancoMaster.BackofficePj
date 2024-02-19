using BancoMaster.Backoffice.Domain.Dtos;
using BancoMaster.Backoffice.Domain.Dtos.CadastroDto;

namespace BancoMaster.Backoffice.Domain.Interfaces.Services
{
    public interface IUsuarioMasterService
    {
        Task<ResponseGenerico<ConsultaResponse>> ConsultaCnpjAsync(string cnpj);
    }
}