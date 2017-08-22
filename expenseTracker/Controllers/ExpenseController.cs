using expenseTracker.Models;
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
    /// <summary>
    /// Controller for manage expenses of user
    /// </summary>
    [Authorize]
    public class ExpenseController : Controller
    {
        protected string UserId
        {
            get
            {
                return (string)Session["UserId"] ?? "hi";
            }
            set
            {
                Session["UserId"] = value;
            }
        }
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


        [Authorize(Roles = "moderator")]
        // GET: /Expense/CreateForUser
        //Create record for user
        public ActionResult CreateForUser()
        {
            return View();
        }

        // POST: /Expense/CreateForUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateForUser([Bind(Include = "Id,Description,Comment,Amount,DateAndTime")] Expense expense)
        {
           
            var currentUser = await manager.FindByIdAsync(UserId);
            if (ModelState.IsValid)
            {
                expense.User = currentUser;
                db.Expenses.Add(expense);
                await db.SaveChangesAsync();
                return RedirectToAction("GetAllUsers", "Manage");
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


        // GET: /Expense/UpdateForUser
        //Update record of user
        [Authorize(Roles = "moderator")]
        public async Task<ActionResult> UpdateForUser(int? id)
        {
            var currentUser = await manager.FindByIdAsync(UserId);
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
        public async Task<ActionResult> UpdateForUser([Bind(Include = "Id,Description,Comment,Amount,DateAndTime")] Expense expense)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                db.Entry(expense).State = System.Data.Entity.EntityState.Modified;
                //  db.Entry<Expense>(expense).Reload();
                await db.SaveChangesAsync();



                return RedirectToAction("GetAllUsers", "Manage");
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


        //Details for user
        [Authorize(Roles = "moderator")]
        public async Task<ActionResult> DetailsForUser(int? id)
        {
            var currentUser = await manager.FindByIdAsync(UserId);
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



        // GET: /Expense/DeleteForUser
        [Authorize(Roles = "moderator")]
        public async Task<ActionResult> DeleteForUser(int? id)
        {
            var currentUser = await manager.FindByIdAsync(UserId);
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
        [Authorize(Roles = "moderator")]
        [HttpPost, ActionName("DeleteForUser")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedForUser(int id)
        {
            Expense expense = await db.Expenses.FindAsync(id);
            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();
            return RedirectToAction("GetAllUsers", "Manage");
        }


    }
}