using System.Security.Claims;
using Duende.IdentityModel;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public void Initialize()
        {
            EnsureRole(IdentityConfiguration.Admin);
            EnsureRole(IdentityConfiguration.Customer);

            EnsureUser(new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@geekshopping.com",
                EmailConfirmed = true,
                FirstName = "Geek",
                LastName = "Admin"
            }, _configuration["SeedUsers:AdminPassword"], IdentityConfiguration.Admin);

            EnsureUser(new ApplicationUser
            {
                UserName = "customer",
                Email = "customer@geekshopping.com",
                EmailConfirmed = true,
                FirstName = "Geek",
                LastName = "Customer"
            }, _configuration["SeedUsers:CustomerPassword"], IdentityConfiguration.Customer);
        }

        private void EnsureRole(string role)
        {
            if (_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult()) return;
            ThrowIfFailed(_roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult(), $"create role '{role}'");
        }

        private void EnsureUser(ApplicationUser user, string? password, string role)
        {
            if (_userManager.FindByNameAsync(user.UserName!).GetAwaiter().GetResult() != null) return;

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException($"Password for seed user '{user.UserName}' is not configured. Set it with dotnet user-secrets.");

            ThrowIfFailed(_userManager.CreateAsync(user, password).GetAwaiter().GetResult(), $"create user '{user.UserName}'");
            ThrowIfFailed(_userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult(), $"add user '{user.UserName}' to role '{role}'");
            ThrowIfFailed(_userManager.AddClaimsAsync(user, new[]
            {
                new Claim(JwtClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtClaimTypes.Role, role)
            }).GetAwaiter().GetResult(), $"add claims to user '{user.UserName}'");
        }

        private static void ThrowIfFailed(IdentityResult result, string action)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to {action}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        }
    }
}
