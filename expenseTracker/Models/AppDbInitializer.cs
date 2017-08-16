﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace expenseTracker.Models
{
        public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                // создаем две роли
                var role1 = new IdentityRole { Name = "admin" };
                var role2 = new IdentityRole { Name = "user" };
                var role3 = new IdentityRole { Name = "moderator" };

            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);

            // создаем пользователей
            var admin = new ApplicationUser { Email = "somemail@mail.ru", UserName = "somemail@mail.ru" };
                string password = "ad46D_ewr3";
                var result = userManager.Create(admin, password);

                // если создание пользователя прошло успешно
                if (result.Succeeded)
                {
                    // добавляем для пользователя роль
                    userManager.AddToRole(admin.Id, role1.Name);
                    userManager.AddToRole(admin.Id, role2.Name);
                    userManager.AddToRole(admin.Id, role3.Name);
            }

                base.Seed(context);
            }
        }
    }