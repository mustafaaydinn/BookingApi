using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BookingApi.Delete
{
    public class Function
    {
        public async Task<APIGatewayHttpApiV2ProxyResponse> DeleteBookingAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {

                if (request == null)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                string idfrompath = request.PathParameters["id"];

                if (idfrompath == null)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                AmazonDynamoDBClient client = new AmazonDynamoDBClient();
                DynamoDBContext dbContext = new DynamoDBContext(client);
                
                Guid id = new Guid(idfrompath);
                var existingBooking = await dbContext.LoadAsync<BookingDto>(id, id);

                if (existingBooking is null)
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "No booking to delete",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };

                await dbContext.DeleteAsync<BookingDto>(existingBooking);

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = $"Booking with Id {idfrompath} Deleted",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}