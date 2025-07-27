using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Samples.Entities;

namespace Sagi.Sdk.AWS.DynamoDb.Indexes
{
    public class GetByStatusIndex : GlobalSecondaryIndex
    {
        public const string INDEX_NAME = "GET_BY_STATUS";

        public GetByStatusIndex()
        {
            IndexName = INDEX_NAME;
            KeySchema = [
                new(nameof(Order.Status), KeyType.HASH),
                new(nameof(Order.CreatedAt), KeyType.RANGE),
            ];

            Projection = new()
            {
                ProjectionType = ProjectionType.ALL,
            };
        }
    }

}
