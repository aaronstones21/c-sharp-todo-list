using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ToDoList.Model;

namespace ToDoList.Services
{
    public interface IToDo
    {

        Task<GetItemResponse> GetSingle(int id, IAmazonDynamoDB _amazonDynamoDb);
        Task<PutItemResponse> Post(ToDo input, IAmazonDynamoDB _amazonDynamoDb);
        Task<ScanResponse> Get(IAmazonDynamoDB _amazonDynamoDb);
        Task<DeleteItemResponse> Delete(int id, IAmazonDynamoDB _amazonDynamoDb);
        Task<GetItemResponse> Put(int id, ToDo input, IAmazonDynamoDB _amazonDynamoDb);
        Task InitialiseAsync(IAmazonDynamoDB _amazonDynamoDb);

    }
}
