# 🎥 Movie Streaming App

## 🌟 Overview

This project is a cloud-native application built using ASP.NET Core MVC and AWS services to mimic a streaming platform. The app allows registered users to upload, manage, and rate movies, as well as leave comments. It integrates seamlessly with AWS services like Elastic Beanstalk, RDS, DynamoDB, and S3 for scalable deployment and storage.

## 🛠️ Features

1. User Management

    Registration: Store user information in AWS RDS (Microsoft SQL Server).
    Login: Secure authentication to access the platform.

2. Movie Management

    Add New Movie: Upload video files to AWS S3 with metadata stored in DynamoDB.
    Modify or Delete Movies: Users can only manage their uploaded movies.
    Search Movies: Filter by:
        Ratings: (e.g., movies with ratings > 9).
        Genre: (e.g., Science Fiction).

3. Comments and Ratings

    Add, view, and rate movies.
    Modify comments and ratings within 24 hours of posting.

4. Cloud-Native Features

    Parameter Store: Securely store credentials and sensitive information.
    Elastic Beanstalk Deployment: Publish the application for scalable hosting.

## 🎯 Project Goals

    Develop a cloud-native application integrating AWS services.
    Implement full CRUD operations for movies and comments.
    Ensure code is readable, efficient, and maintainable.
    Deploy and manage the application using AWS Elastic Beanstalk.

## 🧰 Technical Stack

    Languages: C#, HTML, CSS
    Framework: ASP.NET Core MVC
    Database: AWS RDS (User Data), DynamoDB (Movie Metadata)
    Cloud Services: AWS S3, Elastic Beanstalk, Parameter Store

## 📋 Instructions

1. AWS Setup

    RDS:
        Create a SQL Server database for user registration and login.

    DynamoDB:
       Create a new table named 'Movies'.
       Add secondary indexes for:
         Ratings: Filter movies by rating.
         Genre: Filter movies by genre.
   S3:

    Store movie files securely in an AWS S3 bucket.

Parameter Store:

    Add credentials and configure the app to fetch them securely.
    
## 🖼️ Screenshots

Home Page:
![Screenshot 2024-12-08 130927](https://github.com/user-attachments/assets/a92c8f9b-4733-4fd4-986f-4618402f9573)

Sign In Page:
![Screenshot 2024-12-08 130933](https://github.com/user-attachments/assets/b397f00f-93d1-43cf-a697-3ba019329157)

## 📜 License

This project is licensed under the MIT License.
