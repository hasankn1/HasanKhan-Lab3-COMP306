using Amazon.DynamoDBv2.DataModel;

namespace HasanKhan_Lab3_COMP306.DbData
{
    public class Comments
    {
        [DynamoDBProperty]
        public string UserEmail { get; set; }

        [DynamoDBProperty]
        public string Text { get; set; }

        [DynamoDBProperty]
        public DateTime DateTime { get; set; }

        public Comments() { } // Parameterless constructor for deserialization

        public Comments(string userEmail, string text)
        {
            UserEmail = userEmail;
            Text = text;
            DateTime = DateTime.UtcNow; // Store in UTC for consistency
        }
    }
}
