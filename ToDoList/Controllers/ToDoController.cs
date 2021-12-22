using Microsoft.AspNetCore.Mvc;
using ToDoList.Model;
using Amazon.DynamoDBv2;
using ToDoList.Services;
using Amazon.DynamoDBv2.Model;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {

        public static IAmazonDynamoDB _amazonDynamoDb;

        public ToDoController(IAmazonDynamoDB amazonDynamo)
        {
            _amazonDynamoDb = amazonDynamo;
        }

        public ToDoRepositry dynamo = new ToDoRepositry();

        [HttpGet]
        [Route("init")]
        public void InitialiseAsync()
        {
            _ = dynamo.InitialiseAsync(_amazonDynamoDb);
        }

        [HttpGet("{id}")]
        public GetItemResponse GetSingle(int id)
        {
            return dynamo.GetSingle(id, _amazonDynamoDb).Result;
        }

        [HttpPost]
        public PutItemResponse Post([FromBody] ToDo input)
        {
            return dynamo.Post(input, _amazonDynamoDb).Result;
        }

        [HttpPut("{id}")]
        public GetItemResponse Put(int id, [FromBody] ToDo input)
        {
            return dynamo.Put(id, input, _amazonDynamoDb).Result;
        }

        [HttpDelete("{id}")]
        public DeleteItemResponse Delete(int id)
        {
            return dynamo.Delete(id, _amazonDynamoDb).Result;
        }

        [HttpGet]
        public ScanResponse Get()
        {
            return dynamo.Get(_amazonDynamoDb).Result;
            
        }
    }
}
