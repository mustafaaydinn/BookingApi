using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BookingApi.Create
{
    public class Function
    {
        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateBookingAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log("message-1");
                var bookingRequest = JsonConvert.DeserializeObject<BookingDto>(request.Body);

                if (bookingRequest == null)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                var guid = Guid.NewGuid();
                bookingRequest.Pk = guid;
                bookingRequest.Sk = guid;
                bookingRequest.Id = guid;

                AmazonDynamoDBClient client = new AmazonDynamoDBClient();
                DynamoDBContext dbContext = new DynamoDBContext(client);
                await dbContext.SaveAsync(bookingRequest);
                var message = $"Booking with Id {bookingRequest?.Id} Created";
                LambdaLogger.Log(message);
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = message,
                    StatusCode = (int)HttpStatusCode.Created
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