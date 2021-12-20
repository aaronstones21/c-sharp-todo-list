using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToDoList.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private const string TableName = "ToDo";

        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public ToDoController(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
        }

        [HttpGet]
        [Route("init")]
        public async Task Initialise()
        {
            var request = new ListTablesRequest
            {
                Limit = 10
            };

            var response = await _amazonDynamoDb.ListTablesAsync(request);

            var results = response.TableNames;

            if (!results.Contains(TableName))
            {
                var createRequest = new CreateTableRequest
                {
                    TableName = TableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "N"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH",
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    }
                };

                await _amazonDynamoDb.CreateTableAsync(createRequest);
            }
        }

        [HttpGet("{id}")]
        public async Task<GetItemResponse> GetSingle(int id)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            var response = await _amazonDynamoDb.GetItemAsync(request);

            return response;
        }

        [HttpPost]
        public async Task Post([FromBody] ToDo input)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = input.Id.ToString() }},
                    { "Title", new AttributeValue { S = input.Title }}
                }
            };

            await _amazonDynamoDb.PutItemAsync(request);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] ToDo input)
        {
            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue { N = id.ToString() } } },
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    {"#A", "Title"},

                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    { ":title", new AttributeValue { S = input.Title }}
                },


                UpdateExpression = "SET #A = :title",
            };
            _amazonDynamoDb.UpdateItemAsync(request);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            var response = await _amazonDynamoDb.DeleteItemAsync(request);

            return StatusCode((int)response.HttpStatusCode);
        }

        [HttpGet]
        public async Task<ScanResponse> Get()
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            var response = await _amazonDynamoDb.ScanAsync(request);


            return response;
        }
    }
}
