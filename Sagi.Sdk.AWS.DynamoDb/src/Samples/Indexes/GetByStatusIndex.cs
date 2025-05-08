using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Sagi.Sdk.AWS.DynamoDb.Indexes
{
    public class GetByStatusIndex : GlobalSecondaryIndex
    {
        public const string INDEX_NAME = "GET_BY_STATUS";

        public GetByStatusIndex()
        {
            IndexName = INDEX_NAME;
            KeySchema = [
                new("Status", KeyType.HASH),
                new("CreatedAt", KeyType.RANGE),
            ];

            Projection = new()
            {
                ProjectionType = ProjectionType.ALL,
            };
        }
    }

}
