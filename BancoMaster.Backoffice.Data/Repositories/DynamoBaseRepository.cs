using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using BancoMaster.Backoffice.Domain.Interfaces.Repositories;

namespace BancoMaster.Backoffice.Data.Repositories
{
    public class DynamoBaseRepository<T> : IDynamoBaseRepository<T>
    {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;

        public DynamoBaseRepository()
        {
            _client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
            CriarTabelas();
            _context = new DynamoDBContext(_client);
        }

        private void CriarTabelas()
        {
            var consultaTabelas = _client.ListTablesAsync().Result;
            var listaTabelas = consultaTabelas.TableNames;

            var entidadesDynamo = from a in AppDomain.CurrentDomain.GetAssemblies()
                                  from t in a.GetTypes()
                                  let attributes = t.GetCustomAttributes(typeof(DynamoDBTableAttribute), true)
                                  where attributes != null && attributes.Length > 0
                                  select new { Type = t, Attributes = attributes.Cast<DynamoDBTableAttribute>() };

            foreach (var item in entidadesDynamo)
            {
                var entidade = item.Attributes?.FirstOrDefault();
                if (entidade != null && !listaTabelas.Contains(entidade.TableName))
                {
                    var request = new CreateTableRequest
                    {
                        TableName = entidade.TableName,
                        AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "S"
                        }
                    },
                        KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"
                        }
                    },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 5,
                            WriteCapacityUnits = 5
                        },
                    };

                    _client.CreateTableAsync(request);
                }
            }
        }

        public async Task SalvarAsync(T item)
        {
            await _context.SaveAsync(item);
        }

        public async Task<T> BuscarAsync(object id)
        {
            return await _context.LoadAsync<T>(id);
        }

        public async Task<IEnumerable<T>> BuscarPorScanAsync(List<ScanCondition> condicoes)
        {
            return await _context.ScanAsync<T>(condicoes).GetRemainingAsync();
        }

        public async Task<IEnumerable<T>> BuscarTodosAsync()
        {
            var condicoes = new List<ScanCondition>();
            return await _context.ScanAsync<T>(condicoes).GetRemainingAsync();
        }

        public async Task DeletarAsync(T item)
        {
            await _context.DeleteAsync(item);
        }
    }
}