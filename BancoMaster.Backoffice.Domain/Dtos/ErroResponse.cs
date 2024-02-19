using System.Net;

namespace BancoMaster.Backoffice.Domain.Dtos
{
    public class ErroResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Mensagem { get; set; }
    }
}