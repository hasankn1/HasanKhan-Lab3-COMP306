using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime.Internal.Auth;
using HasanKhan_Lab3_COMP306.DbData;
using HasanKhan_Lab3_COMP306.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HasanKhan_Lab3_COMP306.Models
{
    [DynamoDBTable("Movies")]
    public class Movies
    {
        [DynamoDBHashKey]
        public int MovieID { get; set; }

        [DynamoDBRangeKey]
        [Required(ErrorMessage = "A Movie Title is required!")]
        public string Title { get; set; }

        [DynamoDBProperty]
        public List<string> Director { get; set; } = new List<string>();

        [DynamoDBProperty]
        public string S3URI { get; set; }

        [DynamoDBProperty]
        public string ImageUrl { get; set; }

        [DynamoDBProperty(typeof(GenreConverter))]
        [DynamoDBGlobalSecondaryIndexHashKey("GenreIndex")]
        public Genre Genre { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [DynamoDBProperty]
        [DynamoDBGlobalSecondaryIndexRangeKey("RatingIndex")]
        public double Rating { get; set; } = 0.0;

        [DynamoDBProperty]
        public List<Comments> Comments { get; set; } = new List<Comments>();

        [DynamoDBProperty]
        public string UserEmail { get; set; }

        [NotMapped]
        public IFormFile MovieFile { get; set; }

        [NotMapped]
        public string PreSignedUrl { get; set; }

        public string Substring(string desc)
        {
            return string.IsNullOrEmpty(desc) ? string.Empty : (desc.Length > 60 ? desc.Substring(0, 60) + "..." : desc);
        }
    }
}