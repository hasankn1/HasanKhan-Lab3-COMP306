using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HasanKhan_Lab3_COMP306.Models;
using HasanKhan_Lab3_COMP306.DbData;
using Amazon.S3;


namespace HasanKhan_Lab3_COMP306.Controllers
{
    public class UsersController : Controller
    {
        static Helper conn1 = new Helper();
        AmazonS3Client amazonS3 = conn1.Connection();
        private readonly Lab3Comp306DbContext _context;

        public UsersController(Lab3Comp306DbContext context)
        {
            _context = context;
        }

        public ViewResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(User userLogin)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail == userLogin.UserEmail);
            if (user != null)
            {
                // Set up user session
                HttpContext.Session.SetString("UserEmail", user.UserEmail);
                return RedirectToAction("Index", "Movies");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }


        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup([Bind("UserEmail,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Signin");
            }
            return View(user);
        }

    }
}