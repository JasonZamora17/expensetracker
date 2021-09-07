using System.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private ExpenseTrackerContext db;
        public HomeController(ExpenseTrackerContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "is already in use");
                }
            }

            // in case any above custom errors were added
            if (ModelState.IsValid == false)
            {
                // so error messages will be displayed
                return View("Index");
            }

            // PW Hasher
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            // if createdat = datetime.now is not in model, must add it here
            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("FullName", newUser.FullName());
            return RedirectToAction("Index", "Expense");
        }

        [HttpPost("/login")]
        public IActionResult Login(LoginUser loginUser)
        {
            // dont reveal which was wrong in case they are a hacker
            string genericErrMsg = "Invalid Email and/or Password.";
            if (ModelState.IsValid == false)
            {
                // display validation errors
                return View("Index");
            }
            // check if user email trying to login is in the database
            User dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
            // if dbUser is null, they arent in the database, and display non descript err msg
            if (dbUser == null)
            {
                ModelState.AddModelError("LoginEmail", genericErrMsg);
                return View("Index");
            }
            // user found b/c above return didnt run
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            PasswordVerificationResult pwCompareResult = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);
            // go to PasswordVerificationResult definition to see what result ints mean
            if (pwCompareResult == 0)
            {
                ModelState.AddModelError("LoginEmail", genericErrMsg);
                return View("Index");
            }
            // no returns/no errors
            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("FullName", dbUser.FullName());
            return RedirectToAction("Index", "Expense");
        }

        [HttpGet("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
