
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ToDoList.Model;

namespace ToDoList.Services
{
    public class ToDoRepositry:IToDo
    {

        public string TableName = "ToDo";

        public async Task InitialiseAsync(IAmazonDynamoDB _amazonDynamoDb)
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

        public async Task<GetItemResponse> GetSingle(int id, IAmazonDynamoDB _amazonDynamoDb)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            return await _amazonDynamoDb.GetItemAsync(request);
        }

        public async Task<PutItemResponse> Post(ToDo input, IAmazonDynamoDB _amazonDynamoDb)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = input.Id.ToString() }},
                    { "Title", new AttributeValue { S = input.Title }},
                    { "Status", new AttributeValue { S = input.Status }},
                    { "Description", new AttributeValue { S = input.Description }},
                    { "Timestamp", new AttributeValue { S =  DateTime.Now.ToString() }},
                }
            };

            return await _amazonDynamoDb.PutItemAsync(request);

        }

        public Task<GetItemResponse> Put(int id, ToDo input, IAmazonDynamoDB _amazonDynamoDb)
        {
            _ = Delete(id, _amazonDynamoDb);

            input.Id = id;

            _ = Post(input, _amazonDynamoDb);

            return GetSingle(id, _amazonDynamoDb);

        }

        public async Task<DeleteItemResponse> Delete(int id, IAmazonDynamoDB _amazonDynamoDb)
        {
            var request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            return await _amazonDynamoDb.DeleteItemAsync(request);

        }

        public async Task<ScanResponse> Get(IAmazonDynamoDB _amazonDynamoDb)
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            return await _amazonDynamoDb.ScanAsync(request);

        }
    }
}
