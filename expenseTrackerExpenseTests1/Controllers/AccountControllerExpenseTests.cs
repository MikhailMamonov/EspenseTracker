using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using expenseTracker;
using expenseTracker.Controllers;
using expenseTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication.Internal;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NSubstitute;
using NUnit.Framework.Constraints;

namespace expenseTrackerExpenseTests1.Controllers
{
    [TestClass()]
    public class AccountControllerExpenseTests
    {
        // these are needed on every test
        AccountController _controller;
        private ApplicationDbContext _context;

        public ApplicationUserManager UserManager { get; set; }
        public ApplicationSignInManager SigninManager { get; set; }
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
            



            //            IdentityFactoryOptions<ApplicationUserManager> mockOptions = new Mock<IdentityFactoryOptions<ApplicationUserManager>>().Object;
            //            var mockOwiinContext = new Mock<OwinContext>();
            //            var fakeUserManager = new Mock<ApplicationUserManager>(ApplicationUserManager.Create(mockOptions, mockOwiinContext));
            _controller = new AccountController(UserManager, null, null);

            _controller.ControllerContext = controllerContext.Object;
        }

        [TestMethod()]
        public void ManageControllerRegisterViewIsNotNullTest()
        {
            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Register() is ViewResult result)
            {
                var model = (RegisterViewModel)result.ViewData.Model;
                Assert.IsNotNull(result);
            }
        }

        [TestMethod()]
        public void ManageControllerRegisterIsSuccessAddedInDbTest()
        {
            var viewModel = new RegisterViewModel
            {
                Age = 50,
                Email = "m@mail.ru",
                Password = "mM2010_",
                ConfirmPassword = "mM2010_"
            };

            // ReSharper disable once PatternAlwaysOfType
            if (_controller.Register(viewModel) is Task<ActionResult> result)
            {
                var user = _context.Users.ToList().Last();
                Assert.AreNotEqual(_context.Users.Local.Count,1);
                Assert.AreEqual(user.Age,viewModel.Age);
                Assert.AreEqual(user.Email,viewModel.Email);
                Assert.AreEqual(user.UserName,viewModel.Email);
            }
        }


        [TestMethod()]
        public void ManageControllerLoginIsSuccessTest()
        {
            var viewModel = new RegisterViewModel
            {
                Age = 50,
                Email = "m@mail.ru",
                Password = "mM2010_",
                ConfirmPassword = "mM2010_"
            };

            if (_controller.Register(viewModel) is Task<ActionResult> r)
            {
             
            }
            var loginView = new LoginViewModel
                {
                    Email = "m@mail.ru",
                    Password = "mM2010_",
                    RememberMe = true

                };

                // ReSharper disable once PatternAlwaysOfType
                if (_controller.Login(loginView,"/") is Task<ActionResult> result)
                {
                    
                    Assert.IsNotNull(_controller.SignInManager);
                    
                }
            }
        }

    }
