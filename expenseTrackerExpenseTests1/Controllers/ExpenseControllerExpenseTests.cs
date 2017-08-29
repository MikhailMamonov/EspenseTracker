using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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

        [TestInitialize]
        public void SetupContext()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            AppDbInitializer init = new AppDbInitializer();
            init.InitializeDatabase(context);
            Expense expense = new Expense();
            expense.DateAndTime = new DateTime(2017, 08, 08);
            expense.User = context.Users.First();

            string username = "somemail@mail.ru";
            _controller = new ExpenseController();

            var controllerContext = new Mock<ControllerContext>();

            var identity = new GenericIdentity(username, "ApplicationCookie");
            var nameIdentifierClaim = new Claim(ClaimTypes.NameIdentifier, context.Users.First().Id);
            identity.AddClaim(nameIdentifierClaim);
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity).Returns(identity);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            _controller.ControllerContext = controllerContext.Object;
        }


        [TestMethod()]
        public void ExpenseControllerIndexViewIsNotNullTest()
        {

           
            if (_controller.Index() is ViewResult result) Assert.IsNotNull(result);

        }

        private static HttpContext CreateHttpContext(bool userLoggedIn)
        {
            var httpContext = new HttpContext(
                new System.Web.HttpRequest(string.Empty, "http://sample.com", string.Empty),
                new HttpResponse(new StringWriter())
            )
            {
                User = userLoggedIn
                    ? new GenericPrincipal(new GenericIdentity("userName"), new string[0])
                    : new GenericPrincipal(new GenericIdentity(string.Empty), new string[0])
            };

            return httpContext;
        }


        [TestMethod()]
        public void IndexIndexTest()
        {
            
            List<string> weeks = new List<string>(){"32"};
            if (_controller.Index() is ViewResult result)
            CollectionAssert.AreEqual(weeks, result.ViewBag.weeks);
        }

        [TestMethod()]
        public void UserRecordsIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateForUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateForUserIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TotalAmountIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TotalAmountIndexTest1()
        {
            Assert.Fail();
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