using Amazon.S3;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HasanKhan_Lab3_COMP306.Models;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3.Model;
using HasanKhan_Lab3_COMP306.DbData;

namespace HasanKhan_Lab3_COMP306.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonS3 _s3Client;
        private readonly string BUCKET_NAME = "hasankhanlab3movies";
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IDynamoDBContext dynamoDbContext, IAmazonS3 s3Client, ILogger<MoviesController> logger)
        {
            _dbContext = dynamoDbContext;
            _s3Client = s3Client;
            _logger = logger;
        }

        /*
        public async Task<IActionResult> Index()
        {
            var movies = await _dbContext.ScanAsync<Movies>(null).GetRemainingAsync();
            return View(movies);
        }
        */

        public async Task<IActionResult> Index(string title = null, Genre? genre = null, double? rating = null)
        {
            // Initialize ScanOperationConfig to retrieve all movies without specific conditions
            var config = new ScanOperationConfig();
            var movies = await _dbContext.FromScanAsync<Movies>(config).GetRemainingAsync();

            // Apply filters if specified
            if (!string.IsNullOrWhiteSpace(title))
            {
                movies = movies.Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (genre.HasValue)
            {
                movies = movies.Where(m => m.Genre == genre.Value).ToList();
            }

            if (rating.HasValue)
            {
                movies = movies.Where(m => m.Rating >= rating.Value).ToList();
            }

            // Store the current filter values in ViewData to persist in the view
            ViewData["CurrentTitle"] = title;
            ViewData["CurrentGenre"] = genre?.ToString();
            ViewData["CurrentRating"] = rating?.ToString();

            return View(movies);
        }

        public async Task<IActionResult> Search(string mName)
        {
            // Define scan conditions to search for the movie by title
            var conditions = new List<ScanCondition> { new ScanCondition("Title", ScanOperator.Equal, mName) };

            // Scan the Movies table for matching titles
            var movies = await _dbContext.ScanAsync<Movies>(conditions)
                                          .GetRemainingAsync()
                                          .ConfigureAwait(false);

            // Check if any movies were found
            if (movies.Count == 0)
            {
                return NotFound(); // Return 404 if no movie found
            }

            // Assuming you want to redirect to details of the first matching movie
            // You may want to modify this to pass the correct movie ID or details
            return RedirectToAction("Details", new { id = movies[0].MovieID });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new Movies());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movies movie, IFormFile movieFile)
        {
            if (ModelState.IsValid)
            {
                movie.UserEmail = HttpContext.Session.GetString("UserEmail");

                // Check if the file is not null and has content
                if (movieFile != null && movieFile.Length > 0)
                {
                    var fileName = Path.GetFileName(movieFile.FileName);
                    var uniqueFileName = Guid.NewGuid() + "_" + fileName;

                    _logger.LogInformation("Generated unique file name: {UniqueFileName}", uniqueFileName);

                    // Upload the file to S3 and get the URI
                    var s3Uri = await UploadFileToS3(movieFile, uniqueFileName); // Upload and get the S3 URI
                    movie.S3URI = s3Uri;

                    _logger.LogInformation("Uploaded file to S3. S3 URI: {S3URI}", s3Uri);

                    // Generate pre-signed URL for temporary access
                    var preSignedUrl = GeneratePreSignedURL(s3Uri);
                    movie.PreSignedUrl = preSignedUrl; // Store it if you need to show it to the user

                    _logger.LogInformation("Generated pre-signed URL: {PreSignedUrl}", preSignedUrl);
                }
                else
                {
                    ModelState.AddModelError("S3URI", "File upload is required."); // Add model error if no file uploaded
                    return View(movie);
                }

                // Save the movie to DynamoDB only after getting the S3 URI and pre-signed URL if needed
                await _dbContext.SaveAsync(movie);
                TempData["Created"] = "Movie added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(movie); // Return the view with validation errors if model state is invalid
        }

        // Method to upload file to S3 and get the S3 URI
        private async Task<string> UploadFileToS3(IFormFile file, string fileName)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var request = new PutObjectRequest
                {
                    BucketName = BUCKET_NAME,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.Private // Adjust ACL if needed
                };

                await _s3Client.PutObjectAsync(request);
                return $"s3://{request.BucketName}/{request.Key}"; // S3 URI for internal use
            }
        }

        // Method to generate a pre-signed URL for the uploaded file
        private string GeneratePreSignedURL(string s3Uri)
        {
            var bucketName = BUCKET_NAME;
            var key = s3Uri.Substring(s3Uri.LastIndexOf('/') + 1); // Extract file key from S3 URI

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.Now.AddMinutes(30), // Adjust expiration as needed
                Protocol = Protocol.HTTPS
            };

            return _s3Client.GetPreSignedURL(request);
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _dbContext.LoadAsync<Movies>(id.Value);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _dbContext.LoadAsync<Movies>(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movies movie)
        {
            if (id != movie.MovieID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _dbContext.SaveAsync(movie); // Update movie in DynamoDB
                TempData["Updated"] = "Movie has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id, string title)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _dbContext.LoadAsync<Movies>(id.Value, title);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string title)
        {

            //var movie1 = movies[0].MovieID;
            // Use the current logged-in user's email
            string userEmail = HttpContext.Session.GetString("UserEmail");
            //var user = movie.UserEmail;

            var movie = await _dbContext.LoadAsync<Movies>(id, title);
            var user = movie.UserEmail;

            // Attempt to delete the movie
            if (user == userEmail) // Validate the user email against the movie's uploader
                {
                    // Attempt to delete the movie
                    bool isDeleted = await _movieRepository.DeleteMovie(id, title);

                    if (isDeleted)
                    {
                        TempData["Success"] = "Movie deleted successfully!";
                    }
                    else
                    {
                        TempData["Error"] = "Error deleting the movie.";
                    }
                }
                else
                {
                    TempData["Error"] = "You can only delete movies that you uploaded.";
                }
            return RedirectToAction("Index");
        }

        private async Task<bool> MovieExists(int id)
        {
            var movie = await _dbContext.LoadAsync<Movies>(id);
            return movie != null; // Returns true if the movie exists
        }



        public async Task GetCommentsList(Movies movie)
        {
            double calculateAvgRate = 0;
            double sumRate = 0;

            // Load the movie from DynamoDB
            var currentMovie = await _dbContext.LoadAsync<Movies>(movie.MovieID);
            if (currentMovie != null)
            {
                // Define scan conditions to get comments
                var conditions = new List<ScanCondition> { new ScanCondition("MovieID", ScanOperator.Equal, movie.MovieID) };
                var movieComments = await _dbContext.ScanAsync<Movies>(conditions)
                    .GetRemainingAsync()
                    .ConfigureAwait(false);

                // Process the comments and calculate the average rating
                foreach (var result in movieComments)
                {
                    if (result != null)
                    {
                        movieComments.Add(result);
                        sumRate += result.Rating;
                        calculateAvgRate++;
                    }
                }

                if (calculateAvgRate > 0)
                {
                    movie.Rating = sumRate / calculateAvgRate; // Calculate average rating
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int movieID, Comments comments)
        {
            // Load the existing movie
            var movie = await _dbContext.LoadAsync<Movies>(movieID);
            if (movie == null)
            {
                return NotFound();
            }

            // Add the new comment
            movie.Comments.Add(comments);

            // Save the updated movie back to DynamoDB
            await _dbContext.SaveAsync(movie);

            TempData["CommentAdded"] = "Comment added successfully!";
            return RedirectToAction("Details", new { id = movieID });
        }



        [HttpPost]
        public async Task<IActionResult> DownloadMovie(int id)
        {
            var movie = await _dbContext.LoadAsync<Movies>(id); // Load the movie from DynamoDB
            if (movie?.S3URI != null)
            {
                try
                {
                    string keyName = movie.S3URI; // Assuming S3URI contains the file key
                    var request = new GetPreSignedUrlRequest
                    {
                        BucketName = BUCKET_NAME,
                        Key = keyName,
                        Expires = DateTime.Now.AddMinutes(15)
                    };
                    string url = _s3Client.GetPreSignedURL(request);
                    return Redirect(url); // Redirect to the presigned URL for download
                }
                catch (Exception)
                {
                    TempData["Failure"] = "File download failed. Please try again later.";
                    return RedirectToAction("Index");
                }
            }
            TempData["NoFile"] = "No file exists to download.";
            return RedirectToAction("Index");
        }
    }
}