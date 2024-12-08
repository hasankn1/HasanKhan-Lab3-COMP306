using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using HasanKhan_Lab3_COMP306.Models;

namespace HasanKhan_Lab3_COMP306.DbData
{
    public class DynamoDBMovieRepository : IMovieRepository
    {
        private readonly IDynamoDBContext _dbContext;

        public DynamoDBMovieRepository(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // AddMovies new
        public async Task AddMovie(Movies movies)
        {
            await _dbContext.SaveAsync(movies);
        }

        public IQueryable<Movies> Movies => _dbContext.ScanAsync<Movies>(null).GetRemainingAsync().Result.AsQueryable();

        
        public async Task<Movies> GetMovie(int id)
        {
            return await _dbContext.LoadAsync<Movies>(id); // Load the movie by ID
        }
        

        public async Task<Movies> GetMoviesByTitle(string title)
        {
            return await _dbContext.LoadAsync<Movies>(title); // Load the movie by title
        }

        public async Task SaveMovie(Movies movie, string userEmail)
        {
            movie.UserEmail = userEmail; // Assign the logged-in user
            await _dbContext.SaveAsync(movie); // Save movie to DynamoDB
        }

        public async Task<bool> DeleteMovie(int movieID, string title)
        {
            // Load the movie by ID
            var movie = await _dbContext.LoadAsync<Movies>(movieID, title);


            // Check if the movie exists and if the userEmail matches the uploader's email
            if (movie != null)
            {
                await _dbContext.DeleteAsync(movie);
                return true; // Successfully deleted
            }

            // Movie not found or user is not authorized to delete it
            return false;
        }

        /*
        public async Task<Movies> DeleteMovie(int movieID, string userEmail)
        {
            var movie = await GetMovie(movieID);
            if (movie?.UserEmail == userEmail)
            {
                await _dbContext.DeleteAsync(movie); // Delete the movie if it belongs to the user
                return movie;
            }
            return null; // Return null if movie does not belong to user
        }
        */

        public async Task<IQueryable<Movies>> GetMoviesByGenre(Genre genre)
        {
            var conditions = new List<ScanCondition> { new ScanCondition("Genre", ScanOperator.Equal, genre) };
            return (await _dbContext.ScanAsync<Movies>(conditions).GetRemainingAsync()).AsQueryable();
        }

        public async Task<IQueryable<Movies>> GetMoviesByRating(double rating)
        {
            var conditions = new List<ScanCondition> { new ScanCondition("Rating", ScanOperator.GreaterThanOrEqual, rating) };
            return (await _dbContext.ScanAsync<Movies>(conditions).GetRemainingAsync()).AsQueryable();
        }

        public async Task AddComment(int movieID, string commentText, string userEmail)
        {
            var movie = await GetMovie(movieID);
            if (movie != null)
            {
                var comment = new Comments(userEmail, commentText);
                movie.Comments.Add(comment); // Add new comment
                await _dbContext.SaveAsync(movie); // Save updated movie back to DynamoDB
            }
        }

        public async Task<bool> UpdateComment(int movieID, string originalCommentText, string newCommentText, string userEmail)
        {
            var movie = await GetMovie(movieID);
            if (movie != null)
            {
                var commentToUpdate = movie.Comments.FirstOrDefault(c => c.Text == originalCommentText && c.UserEmail == userEmail);
                if (commentToUpdate != null)
                {
                    if ((DateTime.UtcNow - commentToUpdate.DateTime).TotalHours <= 24) // Check if within 24 hours
                    {
                        commentToUpdate.Text = newCommentText; // Update comment text
                        await _dbContext.SaveAsync(movie); // Save updated movie back to DynamoDB
                        return true; // Update successful
                    }
                    else
                    {
                        // Optionally return some indication that the time limit was exceeded
                        return false; // Update not allowed (time limit exceeded)
                    }
                }
            }
            return false; // Movie not found or comment not found
        }

        public async Task<List<Comments>> GetCommentsList(int movieID)
        {
            var movie = await GetMovie(movieID);
            return movie?.Comments ?? new List<Comments>(); // Return comments or empty list
        }

        public async Task RateMovie(int movieID, double rating)
        {
            var movie = await GetMovie(movieID);
            if (movie != null)
            {
                movie.Rating = rating; // Update rating
                await _dbContext.SaveAsync(movie); // Save updated movie back to DynamoDB
            }
        }
    }
}