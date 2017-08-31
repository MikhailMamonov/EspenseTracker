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
using NLog.Common;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using AsyncHelpers = expenseTracker.Models.AsyncHelpers;
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
        public void UserRecordsExpensesIsEqualCorrectDataTest()
        {
            var currentUser = _context.Users.First();
            List<Expense> currentExpenses = new List<Expense>();
            Expense expense = new Expense();
            expense.DateAndTime = new DateTime(2017, 08, 08);
            expense.User = currentUser;
            expense.Id = 1;
            expense.Amount = 23;
            expense.Comment = "jhbhj";
            expense.Description = "jhbjhbj";
            currentExpenses.Add(expense);
            if (_controller.UserRecords(_context.Users.First().Id) is ViewResult result)
            {
                var expenses = (IEnumerable<Expense>) result.ViewData.Model;
                var enumerable = expenses as IList<Expense> ?? expenses.ToList();
                Assert.AreEqual(enumerable.ToList().Count, currentExpenses.Count);
                foreach (var exp in enumerable)
                {

                    Assert.AreEqual(currentExpenses.First().Id, exp.Id);
                    Assert.AreEqual(currentExpenses.First().Amount, exp.Amount);
                    Assert.AreEqual(currentExpenses.First().Comment, exp.Comment);
                    Assert.AreEqual(currentExpenses.First().DateAndTime.ToString(CultureInfo.InvariantCulture),
                        exp.DateAndTime.ToString(CultureInfo.InvariantCulture));
                    Assert.AreEqual(currentExpenses.First().Description, exp.Description);
                    Assert.AreEqual(currentExpenses.First().User.Id, exp.User.Id);
                }
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
            int totalAmount = 23;
            

            if (_controller.TotalAmount("32") is ViewResult result) { 
                Assert.AreEqual(totalAmount.ToString(), result.ViewBag.totalAmount);
                Assert.AreEqual(result.ViewBag.averageDay, (totalAmount / 7).ToString());
            }
        }

        [TestMethod()]
        public void TotalAmountCheckTotalAmountAndAverageDayOfEmptylistOfRecordTest()
        {
            int totalAmount = 0;


            if (_controller.TotalAmount("34") is ViewResult result)
            {
                Assert.AreEqual(totalAmount.ToString(), result.ViewBag.totalAmount);
                Assert.AreEqual(result.ViewBag.averageDay, (totalAmount / 7).ToString());
            }
        }


        [TestMethod()]
        public void DateFilterNonSpacingInIntervaltTest()
        {
            var startDate = new DateTime(2000,12,06);
            var finishDate =  new DateTime(2001, 12, 06);
            if (_controller.DateFilter(startDate,finishDate) is ViewResult result)
            {
                var expenses = (IEnumerable<Expense>)result.ViewData.Model;
                Assert.AreEqual(0,expenses.ToList().Count);
            }
        }

        [TestMethod()]
        public void DateFilterSpacingInIntervaltTest()
        {
            var startDate = new DateTime(2017, 07, 08);
            var finishDate = new DateTime(2017, 08, 09);
            if (_controller.DateFilter(startDate, finishDate) is ViewResult result)
            {
                var expenses = (IEnumerable<Expense>)result.ViewData.Model;
                Assert.AreEqual(1, expenses.ToList().Count);

            }
        }

        [TestMethod()]
        public void CreateViewIsNotNullTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Create(new Expense()) is Task<ActionResult> result)
            {
                Assert.IsNotNull(result);
            }
        }

        [TestMethod()]
        public void CreateModelIsValidTest()
        {
            Expense expense = new Expense();
            expense.Id = 2;
            expense.User = _context.Users.First();
            expense.Amount = 56;
            expense.Comment = "jhbjhb";
            expense.Description = "hghjhj";
            expense.DateAndTime = DateTime.MaxValue;
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Create(expense) is Task<ActionResult> result)
            {
                var redirectResult = (RedirectToRouteResult)result.Result;
                Assert.AreEqual("Index", redirectResult.RouteValues.Values.ToList().First());
            }

        }

        [TestMethod()]
        public void CreateModelIsNotValidTest()
        {
            var testExpense = new Expense(); 
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Create(testExpense) is Task<ActionResult> result)
            {
                Assert.IsFalse(result.IsCompleted);  
            }

        }



        [TestMethod()]
        public void CreateExpenseCreatedInDbContextForCurrentUserTest()
        {
            Expense expense = new Expense();
            expense.Id = 2;
            expense.User = _context.Users.First();
            expense.Amount = 56;
            expense.Comment = "jhbjhb";
            expense.Description = "hghjhj";
            expense.DateAndTime = DateTime.MaxValue;
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Create(expense) is Task<ActionResult> result)
            {
                var addedExpense = _context.Expenses.Find(2);
                Assert.AreEqual(addedExpense.Id, expense.Id);
                Assert.AreEqual(addedExpense.Amount, expense.Amount);
                Assert.AreEqual(addedExpense.Comment, expense.Comment);
                Assert.AreEqual(addedExpense.DateAndTime.ToString(CultureInfo.InvariantCulture), expense.DateAndTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(addedExpense.Description, expense.Description);
                Assert.AreEqual(addedExpense.User.Id, expense.User.Id);
            }

        }

        [TestMethod()]
        public void UpdateViewIsNotNullTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.Update(1)) is ViewResult result)
            {
                var expense = (Expense)result.ViewData.Model;
                Assert.IsNotNull(expense);
            }
        }

        [TestMethod()]
        public void UpdateViewGettingExpenseCorrectTest()
        {
            
            // ReSharper disable once PatternAlwaysOfType
            if( AsyncHelpers.RunSync<ActionResult>(() => _controller.Update(1))  is ViewResult result)
            {
                Expense testExpense = new Expense();
                testExpense.DateAndTime = new DateTime(2017, 08, 08);
                testExpense.User = _context.Users.First();
                testExpense.Id = 1;
                testExpense.Amount = 23;
                testExpense.Comment = "jhbhj";
                testExpense.Description = "jhbjhbj";
                var expense = (Expense)result.ViewData.Model;
                Assert.IsNotNull(expense);
                Assert.AreEqual(expense.Id,testExpense.Id);
                Assert.AreEqual(expense.Amount, testExpense.Amount);
                Assert.AreEqual(expense.Comment, testExpense.Comment);
                Assert.AreEqual(expense.Description, testExpense.Description);
                Assert.AreEqual(expense.User.Id, testExpense.User.Id);
                Assert.AreEqual(expense.DateAndTime.ToString(CultureInfo.InvariantCulture),
                    testExpense.DateAndTime.ToString(CultureInfo.InvariantCulture));
            }
        }

        //[TestMethod()]
        //public void UpdateViewSettingExpenseCorrectTest()
        //{

        //    Expense testExpense = new Expense();
        //    testExpense.DateAndTime = new DateTime(2018, 08, 08);
        //    testExpense.User = _context.Users.First();
        //    testExpense.Id = 1;
        //    testExpense.Amount = 500;
        //    testExpense.Comment = "hgvhg";
        //    testExpense.Description = "bvhbhb";

           
        //    // ReSharper disable once PatternAlwaysOfType
        //        if (_controller.Update(testExpense) is Task<ActionResult> result)
        //        {

        //            var expense = _context.Expenses.First();
        //            Assert.AreEqual(expense.Id, testExpense.Id);
        //            Assert.AreEqual(expense.Amount, testExpense.Amount);
        //            Assert.AreEqual(expense.Comment, testExpense.Comment);
        //            Assert.AreEqual(expense.Description, testExpense.Description);
        //            Assert.AreEqual(expense.User.Id, testExpense.User.Id);
        //            Assert.AreEqual(expense.DateAndTime.ToString(CultureInfo.InvariantCulture),
        //                testExpense.DateAndTime.ToString(CultureInfo.InvariantCulture));                
        //    }
        //}

        
        [TestMethod()]
        public void DetailsCorrectTest()
        {
            var testExpense = _context.Expenses.First();
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.Details(1)) is ViewResult result)
            {
                var expense = (Expense)result.ViewData.Model;
                Assert.AreEqual(expense.Id,testExpense.Id);
                Assert.AreEqual(expense.Amount, testExpense.Amount);
                Assert.AreEqual(expense.Comment, testExpense.Comment);
                Assert.AreEqual(expense.DateAndTime.ToString(CultureInfo.InvariantCulture), testExpense.DateAndTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(expense.User.Id, testExpense.User.Id);
            }
        }

        [TestMethod()]
        public void DetailsForUserCorrectTest()
        {
            var testExpense = _context.Expenses.First();
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.DetailsForUser(1)) is ViewResult result)
            {
                var expense = (Expense)result.ViewData.Model;
                Assert.AreEqual(expense.Id, testExpense.Id);
                Assert.AreEqual(expense.Amount, testExpense.Amount);
                Assert.AreEqual(expense.Comment, testExpense.Comment);
                Assert.AreEqual(expense.DateAndTime.ToString(CultureInfo.InvariantCulture), testExpense.DateAndTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(expense.User.Id, testExpense.User.Id);
            }
        }

        [TestMethod()]
        public void DeleteGetCorrectExpenseTest()
        {
            var actualExpense = _context.Expenses.First();
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.Delete(1)) is ViewResult result)
            {
                var expense = (Expense)result.ViewData.Model;
                Assert.IsNotNull(expense);
                Assert.AreEqual(expense.Id,actualExpense.Id);
                Assert.AreEqual(expense.Amount, actualExpense.Amount);
                Assert.AreEqual(expense.Comment, actualExpense.Comment);
                Assert.AreEqual(expense.DateAndTime.ToString(CultureInfo.InvariantCulture), actualExpense.DateAndTime.ToString(CultureInfo.InvariantCulture));
            }

           
        }

        [TestMethod()]
        public void DeleteConfirmedRedirectedSuccessTest()
        {

            // ReSharper disable once PatternAlwaysOfType
            if (_controller.DeleteConfirmed(1) is Task<ActionResult> result)
            {
                var redirectResult = (RedirectToRouteResult)result.Result;
                Assert.AreEqual("Index", redirectResult.RouteValues.Values.ToList().First());
            }
        }



        [TestMethod()]
        public void DeleteConfirmedDeleteSuccessTest()
        {
            
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.DeleteConfirmed(1)) is ViewResult result)
            {
                var expense = _context.Expenses.Find(1);
                Assert.IsNull(expense);
            }
        }

        [TestMethod()]
        public void DeleteForUserGetCorrectExpenseTest()
        {
            var actualExpense = _context.Expenses.First();
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.DeleteForUser(1)) is ViewResult result)
            {
                var expense = (Expense)result.ViewData.Model;
                Assert.IsNotNull(expense);
                Assert.AreEqual(expense.Id, actualExpense.Id);
                Assert.AreEqual(expense.Amount, actualExpense.Amount);
                Assert.AreEqual(expense.Comment, actualExpense.Comment);
                Assert.AreEqual(expense.DateAndTime.ToString(CultureInfo.InvariantCulture), actualExpense.DateAndTime.ToString(CultureInfo.InvariantCulture));
            }



        }

        [TestMethod()]
        public void DeleteConfirmedForUserRedirectedSuccessTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.DeleteConfirmed(1) is Task<ActionResult> result)
            {
                var redirectResult = (RedirectToRouteResult)result.Result;
                Assert.AreEqual("Index", redirectResult.RouteValues.Values.ToList().First());
            }
        }

        [TestMethod()]
        public void DeleteConfirmedForUserDeleteddSuccessTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync<ActionResult>(() => _controller.DeleteConfirmed(1)) is ViewResult result)
            {
                var expense = _context.Expenses.Find(1);
                Assert.IsNull(expense);
            }
        }


    }
}