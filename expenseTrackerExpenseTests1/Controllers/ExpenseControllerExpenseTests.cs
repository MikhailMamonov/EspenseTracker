using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using expenseTracker;
using expenseTracker.Controllers;
using expenseTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;
using HttpContext = System.Web.HttpContext;
using HttpResponse = System.Web.HttpResponse;

namespace expenseTrackerExpenseTests1.Controllers
{
    [TestClass()]
    public class ExpenseControllerExpenseTests
    {
        // these are needed on every test
        ExpenseController _controller;

        private ApplicationDbContext _context;

        [TestInitialize]
        public void SetupContext()
        {
            _context = new ApplicationDbContext();
            AppDbInitializer init = new AppDbInitializer();
            init.InitializeDatabase(_context);
            Expense expense = new Expense();
            expense.DateAndTime = new DateTime(2017, 08, 08);
            expense.User = _context.Users.First();

            string username = "somemail@mail.ru";
            _controller = new ExpenseController();

            var controllerContext = new Mock<ControllerContext>();

            var identity = new GenericIdentity(username, "ApplicationCookie");
            var nameIdentifierClaim = new Claim(ClaimTypes.NameIdentifier, _context.Users.First().Id);
            identity.AddClaim(nameIdentifierClaim);
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity).Returns(identity);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(x => x.HttpContext.Session["UserId"]).Returns(_context.Users.First().Id);

            _controller.ControllerContext = controllerContext.Object;
        }


        [TestMethod()]
        public void IndexViewIsNotNullTest()
        {
            if (_controller.Index() is ViewResult result) Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void IndexCorrectWeekNumberComputingTest()
        {
            
            List<string> weeks = new List<string>(){"32"};
            if (_controller.Index() is ViewResult result)
            CollectionAssert.AreEqual(weeks, result.ViewBag.weeks);
        }

        [TestMethod()]
        public void IndexCurrentUserIsCorrectTest()
        {
            if (_controller.Index() is ViewResult result)
            {
                var expenses =(IEnumerable<Expense>) result.ViewData.Model;
                Assert.AreEqual(_context.Users.First().Id, expenses.ToList().First().User.Id);
            }
        }


        [TestMethod()]
        public void UserRecordsExpensesIsNotNullTest()
        {
            if (_controller.UserRecords(_context.Users.First().Id) is ViewResult result)
            {
                var expenses = (IEnumerable<Expense>)result.ViewData.Model;
                Assert.IsNotNull(expenses.ToList());
            }
        }


        [TestMethod()]
        public void UserRecordsCurrentUserIsCorrectTest()
        {
            if (_controller.UserRecords(_context.Users.First().Id) is ViewResult result)
            {
                var expenses = (IEnumerable<Expense>)result.ViewData.Model;
                Assert.AreEqual(_context.Users.First().Id, expenses.ToList().First().User.Id);
            }
        }

        [TestMethod()]
        public void UserRecordsExpensesIsEqualCorrectTest()
        {
            IEnumerable<Expense> currentExpenses = _context.Expenses.ToList().Where(expense => expense.User.Id == _context.Users.First().Id);
            if (_controller.UserRecords(_context.Users.First().Id) is ViewResult result)
            {
                var expenses = (IEnumerable<Expense>)result.ViewData.Model;
                Assert.AreEqual(currentExpenses.ToString(), expenses.ToString());
            }
        }

        [TestMethod()]
        public void CreateForUserResultIsNotNullTest()
        {
            _controller.UserRecords(_context.Users.First().Id);
            Expense expense = new Expense();
            expense.Id = 2;
            expense.User = _context.Users.First();
            expense.Amount = 56;
            expense.Comment = "jhbjhb";
            expense.Description = "hghjhj";
            expense.DateAndTime = DateTime.MaxValue;
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.CreateForUser(expense) is Task<ActionResult> result)
            {
                Assert.IsNotNull(result);
            }

        }

        [TestMethod()]
        public void CreateForUserModelIsValidTest()
        {
            _controller.UserRecords(_context.Users.First().Id);
            Expense expense = new Expense();
            expense.Id = 2;
            expense.User = _context.Users.First();
            expense.Amount = 56;
            expense.Comment = "jhbjhb";
            expense.Description = "hghjhj";
            expense.DateAndTime = DateTime.MaxValue;
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.CreateForUser(expense) is Task<ActionResult> result)
            {
                var redirectResult =(RedirectToRouteResult)result.Result;
                Assert.AreEqual("GetAllUsers", redirectResult.RouteValues.Values.ToList().First());
            }

        }

        [TestMethod()]
        public void CreateForUserExpenseCreatedInDbContextForCurrentUserTest()
        {
            _controller.UserRecords(_context.Users.First().Id);
            Expense expense = new Expense();
            expense.Id = 2;
            expense.User = _context.Users.First();
            expense.Amount = 56;
            expense.Comment = "jhbjhb";
            expense.Description = "hghjhj";
            expense.DateAndTime = DateTime.MaxValue;
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.CreateForUser(expense) is Task<ActionResult> result)
            {
                var addedExpense = _context.Expenses.ToList().Last();
                Assert.AreEqual(addedExpense.Id, expense.Id);
                Assert.AreEqual(addedExpense.Amount, expense.Amount);
                Assert.AreEqual(addedExpense.Comment, expense.Comment);
                Assert.AreEqual(addedExpense.DateAndTime.ToString(CultureInfo.InvariantCulture), expense.DateAndTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(addedExpense.Description, expense.Description);
                Assert.AreEqual(addedExpense.User.Id, expense.User.Id);
            }

        }


        [TestMethod()]
        public void TotalAmountViewIsNotNullTest()
        {
            if (_controller.TotalAmount("32") is ViewResult result)
                Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TotalAmountCheckTotalAmountAndAverageDayTest()
        {
            int totalAmount = 0;
            var expenses = _context.Expenses.ToList().Where(expense => expense.User.Id == _context.Users.First().Id);
            foreach (var i in expenses)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(i.DateAndTime);
                if (CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(i.DateAndTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == Int32.Parse("32"))
                {
                    totalAmount += i.Amount;
                }
            }

            if (_controller.TotalAmount("32") is ViewResult result) { 
                Assert.AreEqual(totalAmount.ToString(), result.ViewBag.totalAmount);
                Assert.AreEqual(result.ViewBag.averageDay, (totalAmount / 7).ToString());
            }
        }

        [TestMethod()]
        public void DateFilterIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateForUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateForUserIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DetailsIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DetailsForUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteConfirmedIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteForUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteConfirmedForUserIndexTest()
        {
Assert.Fail();
        }

       
    }
}