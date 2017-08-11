using expenseTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace expenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
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
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.Expenses.ToList().Where(todo => todo.User.Id == currentUser.Id));
        }

        // GET: /ToDo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ToDo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ExpenseId,Description,Comment,Amount,DateAndTime")] Expense expense)
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
    }
}