using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using HasanKhan_Lab3_COMP306.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HasanKhan_Lab3_COMP306.DbData
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Use the DynamoDBContext from the service provider
            var context = serviceProvider.GetRequiredService<IDynamoDBContext>();

            // Seed Movies
            var movieList = await context.ScanAsync<Movies>(new List<ScanCondition>()).GetRemainingAsync();

            if (!movieList.Any())
            {
                var moviesToAdd = new List<Movies>
                {
                    new Movies
                    {
                        MovieID = 1,
                        Title = "Inception",
                        Director = new List<string> { "Christopher Nolan" },
                        ReleaseDate = DateTime.Parse("2010-07-16"),
                        Genre = Genre.ACTION,
                        Rating = 4.5,
                        S3URI = "s3://hasankhanlab3movies/Inception_2010_OfficialTrailer_1_Christopher_Nolan_Movie_HD.mp4",
                        Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a CEO.",
                        ImageUrl = "https://www.nolanfans.com/images/posters/inception/p4xfull.jpg",
                        Comments = new List<Comments> { new Comments("sadia@example.com", "Incredible!") },
                        UserEmail = "sadia@example.com"
                    },
                    new Movies
                    {
                        MovieID = 2,
                        Title = "Venom: Let There Be Carnage",
                        Director = new List<string> { "Andy Serkis" },
                        ReleaseDate = DateTime.Parse("2021-10-01"),
                        Genre = Genre.HORROR,
                        Rating = 4.0,
                        S3URI = "", // Add S3 path if available
                        Description = "Eddie Brock is still struggling to coexist with the shape-shifting extraterrestrial Venom.",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BNTFiNzBlYmEtMTcxZS00ZTEyLWJmYmQtMjYzYjAxNGQwODAzXkEyXkFqcGdeQXVyMTEyMjM2NDc2._V1_.jpg",
                        Comments = new List<Comments> { new Comments("admin@example.com", "An intense sequel!") },
                        UserEmail = "admin@example.com"
                    },
                    new Movies
                    {
                        MovieID = 3,
                        Title = "Greyhound",
                        Director = new List<string> { "Aaron Schneider" },
                        ReleaseDate = DateTime.Parse("2020-07-10"),
                        Genre = Genre.ACTION,
                        Rating = 3.0,
                        S3URI = "", // Add S3 path if available
                        Description = "A Navy commander leads a convoy of ships across the Atlantic during World War II while being pursued by German U-boats.",
                        ImageUrl = "https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcRqGlJ1E_dsszf-lreRdhk3LiSe9gK1SBzNnw63UIxXiyveYR4I",
                        Comments = new List<Comments> { new Comments("zahra@example.com", "A tense war story.") },
                        UserEmail = "zahra@example.com"
                    },
                    new Movies
                    {
                        MovieID = 4,
                        Title = "Tenet",
                        Director = new List<string> { "Christopher Nolan" },
                        ReleaseDate = DateTime.Parse("2020-08-12"),
                        Genre = Genre.THRILLER,
                        Rating = 4.0,
                        S3URI = "s3://hasankhanlab3movies/TENET.mp4", // Add S3 path if available
                        Description = "A CIA operative embarks on a time-bending mission to prevent World War III.",
                        ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzg0NGM2NjAtNmIxOC00MDJmLTg5ZmYtYzM0MTE4NWE2NzlhXkEyXkFqcGdeQXVyMTA4NjE0NjEy._V1_.jpg",
                        Comments = new List<Comments> { new Comments("hasan@example.com", "A mind-bending thriller.") },
                        UserEmail = "hasan@gmail.com"
                    }
                };

                // Add movies to DynamoDB
                foreach (var movie in moviesToAdd)
                {
                    await context.SaveAsync(movie);
                }
            }

            // Seed Users
            var userList = await context.ScanAsync<User>(new List<ScanCondition>()).GetRemainingAsync();

            if (!userList.Any())
            {
                var usersToAdd = new List<User>
                {
                    new User
                    {
                        UserEmail = "sadia@example.com",
                        Password = "12345"
                    },
                    new User
                    {
                        UserEmail = "sample@gmail.com",
                        Password = "12345"
                    }
                };

                foreach (var user in usersToAdd)
                {
                    await context.SaveAsync(user);
                }
            }
        }
    }
}