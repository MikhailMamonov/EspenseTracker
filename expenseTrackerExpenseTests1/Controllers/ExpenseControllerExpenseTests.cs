using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using expenseTracker.Controllers;
using expenseTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using HttpContext = System.Web.HttpContext;

namespace expenseTrackerExpenseTests1.Controllers
{
    [TestClass()]
    public class ExpenseControllerExpenseTests
    {
        // these are needed on every test
        ExpenseController controller;
     
        [TestInitialize]
        public void TestInitialize()
        {
           controller = new ExpenseController();
           
        }

        [TestMethod()]
        public void ExpenseControllerIndexTest()
        {
            var userMock = new Mock<IPrincipal>();
            userMock.Expect(p => p.IsInRole("admin")).Returns(true);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.ExpectGet(ctx => ctx.User)
                .Returns(userMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.ExpectGet(con => con.HttpContext)
                .Returns(contextMock.Object);

            controller.ControllerContext = controllerContextMock.Object;

            if (controller.Index() is ViewResult result) Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void IndexIndexTest()
        {
            Assert.Fail();
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