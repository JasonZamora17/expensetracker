using System;
using System.Collections.Generic;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private ExpenseTrackerContext db;
        public ExpenseController(ExpenseTrackerContext context)
        {
            db = context;
        }

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

        public IActionResult Index()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            IEnumerable<Expense> expenseList = db.Expenses;
            return View(expenseList);
        }

        // GET-Create 
        public IActionResult Create()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //GET-Delete
        public IActionResult Delete(int? id)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = db.Expenses.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        // GET-Update
        public IActionResult Update(int? id)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = db.Expenses.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        // POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense newExpense)
        {
            if (ModelState.IsValid)
            {
                db.Expenses.Add(newExpense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newExpense);
        }
        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int ExpenseId)
        {
            var obj = db.Expenses.Find(ExpenseId);
            if (obj == null)
            {
                return NotFound();
            }
            db.Expenses.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST-Update (same as create, bute change to update())
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Expense expense)
        {
            if (ModelState.IsValid)
            {
                db.Expenses.Update(expense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expense);
        }
    }
}