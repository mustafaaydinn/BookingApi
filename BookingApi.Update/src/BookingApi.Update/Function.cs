using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BookingApi.Update
{
    public class Function
    {
        public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateBookingAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            try
            {
                var bookingRequest = JsonConvert.DeserializeObject<BookingDto>(request.Body);

                if (bookingRequest == null)
                {
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "Booking couldn't found",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                AmazonDynamoDBClient client = new AmazonDynamoDBClient();
                DynamoDBContext dbContext = new DynamoDBContext(client);
                var existingBooking = await dbContext.LoadAsync<BookingDto>(bookingRequest.Id, bookingRequest.Id);

                if (existingBooking is null)
                    return new APIGatewayHttpApiV2ProxyResponse
                    {
                        Body = "No booking to update ",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };


                existingBooking.Cancelled = bookingRequest.Cancelled;
                existingBooking.Cost = bookingRequest.Cost;
                existingBooking.BookingStartDate = bookingRequest.BookingStartDate;
                existingBooking.BookingEndDate = bookingRequest.BookingEndDate;
                existingBooking.Name = bookingRequest.Name;
                existingBooking.Notes = bookingRequest.Notes;
                existingBooking.RoomNumber = bookingRequest.RoomNumber;

                await dbContext.SaveAsync(existingBooking);

                var message = $"Booking with Id {bookingRequest?.Id} Updated";
                LambdaLogger.Log(message);
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = message,
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