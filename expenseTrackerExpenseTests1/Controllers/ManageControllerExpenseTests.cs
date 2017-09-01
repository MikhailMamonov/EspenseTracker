using Microsoft.VisualStudio.TestTools.UnitTesting;
using expenseTracker.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using expenseTracker.Models;
using expenseTrackerExpenseTests1;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Moq;
// ReSharper disable PatternAlwaysOfType


namespace expenseTracker.Controllers.ExpenseTests
{
    [TestClass()]
    public class ManageControllerExpenseTests
    {


        // these are needed on every test
        ManageController _controller;
        private ApplicationDbContext _context;

        public ApplicationUserManager UserManager { get; set; }
        public ApplicationRoleManager RoleManager { get; set; }


        [TestInitialize]
        public void SetupContext()
        {
            _context = new ApplicationDbContext();
            AppDbInitializer init = new AppDbInitializer();
            init.InitializeDatabase(_context);
            string username = "somemail@mail.ru";
            

            var controllerContext = new Mock<ControllerContext>();

            var identity = new GenericIdentity(username, "ApplicationCookie");
            var nameIdentifierClaim = new Claim(ClaimTypes.NameIdentifier, _context.Users.First().Id);
            identity.AddClaim(nameIdentifierClaim);
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity).Returns(identity);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(x => x.HttpContext.Session["UserId"]).Returns(_context.Users.First().Id);

            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            RoleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(_context));



            //            IdentityFactoryOptions<ApplicationUserManager> mockOptions = new Mock<IdentityFactoryOptions<ApplicationUserManager>>().Object;
            //            var mockOwiinContext = new Mock<OwinContext>();
            //            var fakeUserManager = new Mock<ApplicationUserManager>(ApplicationUserManager.Create(mockOptions, mockOwiinContext));
            _controller = new ManageController(UserManager,null,RoleManager);

            _controller.ControllerContext = controllerContext.Object;
        }



        [TestMethod()]
        public void ManageControllerIndexTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Index(new ManageController.ManageMessageId()) is Task<ActionResult> result)
            {
                Assert.IsNotNull(result);
            }
        }

        [TestMethod()]
        public void ManageControllerGetAllUsersIsNotNullTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.GetAllUsers() is Task<ActionResult> result)
            {
                Assert.IsNotNull(result);
            }
        }


        [TestMethod()]
        public void ManageControllerGetAllUsersDataIsCorrectTest()
        {
            List<ApplicationUser> testUsers = new List<ApplicationUser>();
            var admin = new ApplicationUser { Email = "somemail@mail.ru", UserName = "somemail@mail.ru" };


            testUsers.Add(admin);
            // ReSharper disable once PatternAlwaysOfType
            if (AsyncHelpers.RunSync(() => _controller.GetAllUsers()) is ViewResult result)
            {
                var users = (List<ApplicationUser>)result.ViewData.Model;
                for (int i = 0; i < users.Count; i++)
                {
                    var getUser = users[i];
                    Assert.AreEqual(getUser.Age, testUsers[i].Age);
                    Assert.AreEqual(getUser.Email, testUsers[i].Email);
                    Assert.AreEqual(getUser.EmailConfirmed, testUsers[i].EmailConfirmed);
                    Assert.AreEqual(getUser.UserName, testUsers[i].UserName);
                }
            }
        }


        [TestMethod()]
        public void ManageControllerCreateUserViewNotNullTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.CreateUser() is ViewResult result)
            {
                Assert.IsNotNull(result);
            }
        }

        [TestMethod()]
        public void ManageControllerCreateUserGetDataIsCorrectTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.CreateUser() is ViewResult result)
            {
                var roles = new List<SelectListItem>()
                {
                    new SelectListItem {Value = "admin", Text = "admin"},
                    new SelectListItem {Value = "moderator", Text = "moderator"},
                    new SelectListItem {Value = "user", Text = "user"}
            };
                var gettingRoles = (List<SelectListItem>) result.ViewBag.Roles;
                Assert.IsNotNull(gettingRoles);
                Assert.AreEqual(roles.Count, gettingRoles.Count);
                for (int i = 0; i < gettingRoles.Count; i++)
                {
                    var gettingRole = gettingRoles[i];
                    Assert.AreEqual(gettingRole.Text,roles[i].Text);
                    if (gettingRole.Value != null) Assert.AreEqual(gettingRole.Value, actual: roles[i].Value);
                }
            }
        }

        [TestMethod()]
        public void ManageControllerCreateUserFormIsValidTest()
        {
            var viewModel = new RegisterViewModel();
            viewModel.Age = 50;
            viewModel.Email = "m@mail.ru";
            viewModel.Password = "mM2010_";
            viewModel.ConfirmPassword = "mM2010_";

            // ReSharper disable once PatternAlwaysOfType
            if (_controller.CreateUser(viewModel ,"admin") is Task<ActionResult> result)
            {
                var redirectResult = (RedirectToRouteResult)result.Result;
                Assert.AreEqual("GetAllUsers", redirectResult.RouteValues.Values.ToList().First());
            }
        }

        [TestMethod()]
        public void ManageControllerCreateUserUserCreateInDbIsCorrectTest()
        {
            var viewModel = new RegisterViewModel();
            viewModel.Age = 50;
            viewModel.Email = "m@mail.ru";
            viewModel.Password = "mM2010_";
            viewModel.ConfirmPassword = "mM2010_";

            
            if (_controller.CreateUser(viewModel, "admin") is Task<ActionResult> result)
            {
                // ReSharper disable once PatternAlwaysOfType
                var createdUser = _context.Users.Local.ToList().First();
                Assert.AreEqual(viewModel.Age, createdUser.Age);
                Assert.AreEqual(viewModel.Email, createdUser.UserName);
                Assert.AreEqual(viewModel.Email, createdUser.Email);
                
            }
        }

        [TestMethod()]
        public void GetAllUsersIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateUserIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EditIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EditIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DetailsViewIsNotNullCorrectTest()
        {
            var testUser = _context.Users.Local.ToList().First();
            if (AsyncHelpers.RunSync(() => _controller.Details(_context.Users.Local.ToList().First().Id)) is ViewResult
                result)
            {
                Assert.IsNotNull(result);
            }
        }

        [TestMethod()]
        public void DetailsViewResponseIsCorrectTest()
        {
            var testUser = _context.Users.Local.ToList().First();
            if (AsyncHelpers.RunSync(() => _controller.Details(_context.Users.Local.ToList().First().Id)) is ViewResult result)
            {
                var user = (ApplicationUser)result.ViewData.Model;
              

                    Assert.AreEqual(user.Age, testUser.Age);
                    Assert.AreEqual(user.Email, testUser.Email);
                    Assert.AreEqual(user.EmailConfirmed, testUser.EmailConfirmed);
                    Assert.AreEqual(user.UserName, testUser.UserName);
                }
        }

        [TestMethod()]
        public void DeleteUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteConfirmedUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RoleAddToUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteRoleForUserIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IndexIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveLoginIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddPhoneNumberIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddPhoneNumberIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EnableTwoFactorAuthenticationIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisableTwoFactorAuthenticationIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void VerifyPhoneNumberIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void VerifyPhoneNumberIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemovePhoneNumberIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangePasswordIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangePasswordIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPasswordIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPasswordIndexTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ManageLoginsIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LinkLoginIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LinkLoginCallbackIndexTest()
        {
            Assert.Fail();
        }
    }
}