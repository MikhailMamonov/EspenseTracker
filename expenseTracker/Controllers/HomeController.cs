using expenseTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace expenseTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> manager;

        public HomeController()
        {
            db = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index()
        {
            int hour = DateTime.Now.Hour;
            ViewBag.Greeting = hour < 12 ? "Доброе утро" : "Доброго дня";

            IList<string> roles = new List<string> { "Роль не определена" };
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
            .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
                roles = userManager.GetRoles(user.Id);
            return View(roles);
        }

        [Authorize(Roles = "admin")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ViewResult RsvpForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RsvpForm(Expense expense)
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

        public async Task<ActionResult> Create([Bind(Include = "ExpenseId,Description,Comment,Amount,DateAndTime")] Expense expense)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (ModelState.IsValid) {
                expense.User = currentUser;
                db.Expenses.Add(expense);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(expense);
        }
    }
}