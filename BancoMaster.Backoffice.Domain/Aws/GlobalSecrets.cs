using BancoMaster.Backoffice.Domain.Models.Configuracao;

namespace BancoMaster.Backoffice.Domain.Aws
{
    public static class GlobalSecrets
    {
        private static string _elasticUri;
        public static string ElasticSearchUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_elasticUri))
                {
                    _elasticUri = SecretsManager.GetSecretValueAsync("MAXIMA-ELASTICSEARCH-LOGGING").Result;
                }

                return _elasticUri;
            }
        }

        private static WiseConsActions wiseConsActions;
        public static WiseConsActions WiseConsActions
        {
            get
            {
                if (wiseConsActions == null)
                {
                    var secret = SecretsManager.GetSecretValueAsync("EP-APITARIFADOR-WISECONSAPI").Result;
                    if (!string.IsNullOrEmpty(secret))
                    {
                        wiseConsActions = Newtonsoft.Json.JsonConvert.DeserializeObject<WiseConsActions>(secret);
                    }
                }

                return wiseConsActions;
            }
        }

    }
}