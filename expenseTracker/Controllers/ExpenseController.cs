﻿using expenseTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace expenseTracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        protected string UserId { get; set; }
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> manager;
        
        public ExpenseController()
        {
            db = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: /Expense/
        // GET Expense for the logged in user
        public ActionResult Index()
        {
            ViewBag.Date = new DateTime(2000, 12, 06);
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.Expenses.ToList().Where(expense => expense.User.Id == currentUser.Id));
        }



        // GET: /Manage/UserRecords
        // GET Expenses for filled user
        [Authorize(Roles = "moderator")]
        [HttpGet]
        public ActionResult UserRecords(string id)
        {
            var currentUser = manager.FindById(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IEnumerable<Expense> expenses = db.Expenses.ToList().Where(expense => expense.User.Id == currentUser.Id);
            if (expenses == null)
            {
                return HttpNotFound();
            }

            UserId = id;
            return View(expenses);
        }



        [Authorize(Roles = "admin")]
        public async Task<ActionResult> All()
        {
        return View(await db.Expenses.ToListAsync());
        }

        [Authorize(Roles = "moderator")]
        // GET: /Expense/Create
        public ActionResult CreateForUser()
        {
            return View();
        }

        // POST: /Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateForUser([Bind(Include = "Id,Description,Comment,Amount,DateAndTime")] Expense expense)
        {
            var currentUser = User.Identity.GetUserId();
            bool fe = UserId.Equals(currentUser);
            var fcurrentUser = await manager.FindByIdAsync(currentUser);
            if (ModelState.IsValid)
            {
                expense.User = fcurrentUser;
                db.Expenses.Add(expense);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(expense);
        }





        // GET: /Expense/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Description,Comment,Amount,DateAndTime")] Expense expense)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                expense.User = currentUser;
                db.Expenses.Add(expense);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(expense);
        }

        // GET: /Expense/Update
        public async Task<ActionResult> Update(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = await db.Expenses.FindAsync(id);
            if (expense == null)
            {
                return HttpNotFound();
            }

            if (expense.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(expense);
        }

        // POST: /Expense/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([Bind(Include = "Id,Description,Comment,Amount,DateAndTime")] Expense expense)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                db.Entry(expense).State=System.Data.Entity.EntityState.Modified;
              //  db.Entry<Expense>(expense).Reload();
                await db.SaveChangesAsync();
                
                
                
                return RedirectToAction("Index");
            }
            return View(expense);
        }

        public async Task<ActionResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = await db.Expenses.FindAsync(id);
            if (expense == null)
            {
                return HttpNotFound();
            }

            if (expense.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(expense);
        }

        // GET: /Expense/Delete
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = await db.Expenses.FindAsync(id);
            if (expense == null)
            {
                return HttpNotFound();
            }

            if (expense.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(expense);
        }

        // POST: /Expense/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Expense expense = await db.Expenses.FindAsync(id);
            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }



    }
}