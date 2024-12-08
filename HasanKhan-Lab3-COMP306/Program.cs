using Microsoft.EntityFrameworkCore;
using HasanKhan_Lab3_COMP306.Models;
using Microsoft.Data.SqlClient;
using Amazon;
using Amazon.Runtime;
using HasanKhan_Lab3_COMP306.DbData;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;

namespace HasanKhan_Lab3_COMP306
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Retrieve AWS credentials from appsettings.json directly
            var awsAccessKeyId = builder.Configuration["AWSCredentials:AccesskeyID"];
            var awsSecretAccessKey = builder.Configuration["AWSCredentials:Secretaccesskey"];

            // Configure AWS options
            var awsOptions = new AWSOptions
            {
                Region = RegionEndpoint.CACentral1,
                Credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey)
            };

            // Use the connection string directly from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("Connection2RDS");

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<Lab3Comp306DbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register AWS services
            builder.Services.AddAWSService<IAmazonDynamoDB>(awsOptions);
            builder.Services.AddAWSService<IAmazonS3>(awsOptions);
            builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();

            // Add your repositories
            builder.Services.AddTransient<IMovieRepository, DynamoDBMovieRepository>();
            builder.Services.AddTransient<IUserRepository, EFUserRepository>();

            // Configure session state and HTTP context accessor
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}




/*
            builder.Configuration.AddSystemsManager("/MovieWebApp", new Amazon.Extensions.NETCore.Setup.AWSOptions
            {
                Region = RegionEndpoint.CACentral1,
                Credentials = new BasicAWSCredentials(
                    builder.Configuration["AWSCredentials:AccesskeyID"],
                    builder.Configuration["AWSCredentials:Secretaccesskey"])
            });

            // Configure connection string for SQL Server
            var connectionString = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("Connection2RDS"))
            {
                UserID = builder.Configuration["DbUser"],
                Password = builder.Configuration["DbPassword"]
            }.ConnectionString;
            
// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Lab3Comp306DbContext>(options =>
    options.UseSqlServer(connectionString));
*/