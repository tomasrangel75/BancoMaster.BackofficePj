using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text;

namespace BancoMaster.Backoffice.Domain.Aws
{
    public class SecretsManager
    {
        public static async Task<string> GetSecretValueAsync(string secretId)
        {
            try
            {
                var request = new GetSecretValueRequest()
                {
                    SecretId = secretId
                };

                using (var client = new AmazonSecretsManagerClient(RegionEndpoint.USEast1))
                {
                    var response = await client.GetSecretValueAsync(request);

                    if (response?.SecretString != null)
                    {
                        return response.SecretString;
                    }
                    else
                    {
                        using (var reader = new StreamReader(response.SecretBinary))
                        {
                            return Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                        }
                    }
                }
            }
            catch (ResourceNotFoundException ex)
            {
                throw new ResourceNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
        }

        public static string GetSecretValue(string secretId)
        {
            return Task.Factory.StartNew(() => GetSecretValueAsync(secretId)).Unwrap().GetAwaiter().GetResult();
        }
    }
}