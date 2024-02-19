using BancoMaster.Backoffice.Domain.Dtos;
using BancoMaster.Backoffice.Domain.Dtos.CadastroDto;
using BancoMaster.Backoffice.Domain.Interfaces.Services;

namespace BancoMaster.Backoffice.Service.Services
{
    public class UsuarioMasterService : IUsuarioMasterService
    {
        public UsuarioMasterService()
        {
        }

        public async Task<ResponseGenerico<ConsultaResponse>> ConsultaCnpjAsync(string cnpj)
        {
            var response = new ResponseGenerico<ConsultaResponse>();

            return response;
        }

    }
}