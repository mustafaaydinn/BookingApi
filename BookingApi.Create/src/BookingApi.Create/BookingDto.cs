using Amazon.DynamoDBv2.DataModel;

namespace BookingApi.Create
{
    [DynamoDBTable("bookingapi")]
    public class BookingDto
    {
        [DynamoDBHashKey("Pk")]
        public Guid Pk { get; set; }

        [DynamoDBRangeKey(AttributeName = "Sk")]
        public Guid Sk { get; set; }

        [DynamoDBProperty("Id")]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int RoomNumber { get; set; }

        public DateTime BookingStartDate { get; set; }

        public DateTime? BookingEndDate { get; set; }

        public decimal Cost { get; set; }

        public string? Notes { get; set; }

        public bool? Cancelled { get; set; }
    }
}