using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BookingApi.Get
{
    public class Function
    {
        public async Task<BookingDto?> GetBookingByIdAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                AmazonDynamoDBClient client = new AmazonDynamoDBClient();
                DynamoDBContext dbContext = new DynamoDBContext(client);
                string idfrompath = request.PathParameters["id"];
                Guid id = new Guid(idfrompath);
                var booking = await dbContext.LoadAsync<BookingDto>(id, id);
               
                if (booking == null)
                {
                    LambdaLogger.Log("No booking");
                    return null;
                }

                return booking;
            }
            catch (Exception ex)
            {
                LambdaLogger.Log(ex.Message);
                return null;
            }
        }
    }
}