using HasanKhan_Lab3_COMP306.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HasanKhan_Lab3_COMP306.DbData
{
    public interface IMovieRepository
    {
        // Retrieve all movies as an IQueryable
        IQueryable<Movies> Movies { get; }

        // Add Movies new
        Task AddMovie(Movies movies);

        // Get a single movie by ID
        Task<Movies> GetMovie(int id);

        // Get a single movie by title
        Task<Movies> GetMoviesByTitle(string title);

        // Save a new movie or update an existing one
        Task SaveMovie(Movies movie, string userEmail);

        // Delete a movie based on movie ID and user email
        Task<bool> DeleteMovie(int movieID, string userEmail);

        // List movies based on genre
        Task<IQueryable<Movies>> GetMoviesByGenre(Genre genre);

        // List movies based on rating
        Task<IQueryable<Movies>> GetMoviesByRating(double rating);

        // Add a comment to a movie
        Task AddComment(int movieID, string commentText, string userEmail);

        // Update a comment within 24 hours
        Task<bool> UpdateComment(int movieID, string originalCommentText, string newCommentText, string userEmail);

        // Retrieve comments for a specific movie
        Task<List<Comments>> GetCommentsList(int movieID);

        // Rate a movie
        Task RateMovie(int movieID, double rating);
    }
}