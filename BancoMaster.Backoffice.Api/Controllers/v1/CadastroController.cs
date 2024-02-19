using BancoMaster.Backoffice.Domain.Dtos.CadastroDto;
using BancoMaster.Backoffice.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BancoMaster.Backoffice.Api.Controllers.v1
{
    [Route("usuario-master-api/v1/[controller]")]
    [ApiController]
   
    public class UsuarioMasterController : ControllerBase
    {
        private readonly ILogger<UsuarioMasterController> _logger;
        private readonly IUsuarioMasterService _usuarioMasterService;

        public UsuarioMasterController(ILogger<UsuarioMasterController> logger, IUsuarioMasterService usuarioMasterService)
        {
            _logger = logger;
            _usuarioMasterService = usuarioMasterService;
        }

        [HttpGet("consulta-empresa")]
        [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConsultaEmpresaPorCnpj()
        {
            var cpfCnpj = "cnpj";

            _logger.LogInformation("Operação => Chamada Rota Consultar CNPJ - CpfCpnj: {cpfCpnj}", cpfCnpj);

            var response = await _usuarioMasterService.ConsultaCnpjAsync(cpfCnpj);

            if (response.CodigoHttp == HttpStatusCode.OK)
            {
                return Ok(response.DadosRetorno);
            }
            else
            {
                return StatusCode((int)response.CodigoHttp, response.ErroRetorno);
            }
        }
    }
}
