using System;
using System.Runtime.Serialization;

namespace BancoMaster.Backoffice.Domain.Exceptions
{
    [Serializable]
    public class MasterException : Exception
    {
        protected MasterException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {
            Origem = info.GetString("Origem");
        }

        public MasterException(string origem, string mensagem) : base(mensagem)
        {
            Origem = origem;
        }

        public MasterException(string mensagem) : base(mensagem)
        {
        }

        public MasterException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
        public MasterException(string origem, string mensagem, Exception innerException) : base(mensagem, innerException)
        {
            Origem = origem;
        }
        public string CodigoErro { get; set; }

        public string Origem { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("Origem", Origem);
        }
    }
}
