using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Maverick.Models.User;
using Maverick.Models;
using Maverick.Models.History;

#pragma warning disable IDE0290

namespace Maverick.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        DbSet<AppUserModel> AppUsers { get; set; }
        DbSet<HistoryModel> History { get; set; }
        DbSet<InfoClientModel> InfoClients { get; set; }
    }
}
