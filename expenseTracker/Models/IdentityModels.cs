﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace expenseTracker.Models
{

    //model User
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Range(0, 100, ErrorMessage = "Требуется возвраст от 0 до 100.")]
        public int Age { get; set; } // add propertiy age

        [Required, EmailAddress, Display(Name = "Email")]
        public override string Email { get; set; }


        // add records of expenses
        public virtual ICollection<Expense> Expenses { get; set; }


        

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.SetIsPersistent(true);
            return userIdentity;
        }
    }



    // Model expense
    public class Expense
    {
        //foreign key
        public int Id { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите свое описание затраты")]
        //description
        public string Description { get; set; }
        //comment
        [Required(ErrorMessage = "Пожалуйста, введите Комментарий")]
        public string Comment { get; set; }
        //general amount
        [Required(ErrorMessage = "Пожалуйста, введите сумму")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Требуется положительная сумма.")]
        public int Amount { get; set; }
        //date and time
        [Required(ErrorMessage = "Пожалуйста, введите дату и время")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateAndTime { get; set; }
        //user reference
        public virtual ApplicationUser User { get; set; }

    }

    //Db context
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Expense> Expenses { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}