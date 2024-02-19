using System.Net;

namespace BancoMaster.Backoffice.Domain.Dtos
{
    public class ResponseGenerico<T> where T : class
    {
        public HttpStatusCode CodigoHttp { get; set; }
        public T DadosRetorno { get; set; }
        public ErroResponse ErroRetorno { get; set; }
    }
}