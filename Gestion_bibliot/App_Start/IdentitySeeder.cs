using Gestion_bibliot;
using Gestion_bibliot.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

public class IdentitySeeder
{
    public static void Seed()
    {
        using (var context = new ApplicationDbContext())
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Seed roles
            string[] roles = { "Admin", "Librarian", "Student" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                    roleManager.Create(new IdentityRole(role));
            }

            // Seed default admin
            if (userManager.FindByEmail("admin@biblio.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@biblio.com",
                    Email = "admin@biblio.com",
                    FullName = "Admin User",
                    Role = "Admin",                     // Obligatoire !
                    CIN = "ADMIN123",                   // Obligatoire !
                    DateOfBirth = DateTime.Parse("1990-01-01"), // Obligatoire !
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                userManager.Create(user, "Admin123!");
                userManager.AddToRole(user.Id, "Admin");
            }

            if (userManager.FindByEmail("librarian@biblio.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "librarian@biblio.com",
                    Email = "librarian@biblio.com",
                    FullName = "Librarian User",
                    Role = "Librarian",
                    CIN = "LIBR123",
                    DateOfBirth = DateTime.Parse("1985-05-15"),
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                userManager.Create(user, "Librarian123!");
                userManager.AddToRole(user.Id, "Librarian");
            }

            // Student
            if (userManager.FindByEmail("student@biblio.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "student@biblio.com",
                    Email = "student@biblio.com",
                    FullName = "Student User",
                    Role = "Student",
                    CIN = "STUD123",
                    DateOfBirth = DateTime.Parse("2000-09-20"),
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                userManager.Create(user, "Student123!");
                userManager.AddToRole(user.Id, "Student");
            }


        }
    }
}
