using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class ItemController : Controller
    {
        private ExpenseTrackerContext db;
        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }
        public ItemController(ExpenseTrackerContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Item> itemList = db.Items.ToList();
            return View("Index", itemList);
        }

        public IActionResult Create()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        // POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Item newItem)
        {
            db.Items.Add(newItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}