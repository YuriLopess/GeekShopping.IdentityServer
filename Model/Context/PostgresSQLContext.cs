using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.IdentityServer.Model.Context;

public class PostgresSQLContext : IdentityDbContext<ApplicationUser>
{
    public PostgresSQLContext(DbContextOptions<PostgresSQLContext> options) : base(options) { }
}
