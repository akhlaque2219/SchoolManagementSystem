using Microsoft.AspNetCore.Identity;
using SchoolManagement.Models;

namespace SchoolManagement.Data
{
    public static class IdentitySeed
    {
        public static readonly string[] Roles = { "Admin", "Teacher", "Staff", "Student" };

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin
            await CreateUser(userManager, new ApplicationUser
            {
                UserName     = "admin@school.edu",
                Email        = "admin@school.edu",
                FirstName    = "Super",
                LastName     = "Admin",
                PhoneNumber  = "555-0001",
                EmailConfirmed = true,
                IsActive     = true,
                PrimaryRole  = "Admin",
                CreatedAt    = DateTime.UtcNow
            }, "Admin@123456", "Admin");

            // Seed Staff
            await CreateUser(userManager, new ApplicationUser
            {
                UserName     = "staff@school.edu",
                Email        = "staff@school.edu",
                FirstName    = "Office",
                LastName     = "Staff",
                PhoneNumber  = "555-0002",
                EmailConfirmed = true,
                IsActive     = true,
                PrimaryRole  = "Staff",
                CreatedAt    = DateTime.UtcNow
            }, "Staff@123456", "Staff");

            // Seed Teachers (linked to teacher records Id=1..4)
            var teachers = new[]
            {
                ("john.smith@school.edu",    "John",    "Smith",    1),
                ("sarah.johnson@school.edu", "Sarah",   "Johnson",  2),
                ("michael.davis@school.edu", "Michael", "Davis",    3),
                ("emily.wilson@school.edu",  "Emily",   "Wilson",   4),
            };
            foreach (var (email, first, last, tid) in teachers)
            {
                await CreateUser(userManager, new ApplicationUser
                {
                    UserName       = email,
                    Email          = email,
                    FirstName      = first,
                    LastName       = last,
                    EmailConfirmed = true,
                    IsActive       = true,
                    PrimaryRole    = "Teacher",
                    TeacherId      = tid,
                    CreatedAt      = DateTime.UtcNow
                }, "Teacher@123456", "Teacher");
            }

            // Seed Students (linked to student records Id=1..8)
            var students = new[]
            {
                ("alice@student.edu",  "Alice", "Brown",    1),
                ("bob@student.edu",    "Bob",   "Taylor",   2),
                ("carol@student.edu",  "Carol", "Anderson", 3),
                ("david@student.edu",  "David", "Martinez", 4),
                ("emma@student.edu",   "Emma",  "Garcia",   5),
                ("frank@student.edu",  "Frank", "Lee",      6),
                ("grace@student.edu",  "Grace", "White",    7),
                ("henry@student.edu",  "Henry", "Harris",   8),
            };
            foreach (var (email, first, last, sid) in students)
            {
                await CreateUser(userManager, new ApplicationUser
                {
                    UserName       = email,
                    Email          = email,
                    FirstName      = first,
                    LastName       = last,
                    EmailConfirmed = true,
                    IsActive       = true,
                    PrimaryRole    = "Student",
                    StudentId      = sid,
                    CreatedAt      = DateTime.UtcNow
                }, "Student@123456", "Student");
            }
        }

        private static async Task CreateUser(UserManager<ApplicationUser> um, ApplicationUser user, string password, string role)
        {
            if (await um.FindByEmailAsync(user.Email!) == null)
            {
                var result = await um.CreateAsync(user, password);
                if (result.Succeeded)
                    await um.AddToRoleAsync(user, role);
            }
        }
    }
}
