using Astami.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Astami.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

		public DbSet<Abbonamento> Abbonamento { get; set; }
		public DbSet<PianoSelezionato> PianoSelezionato { get; set; }
	}
}
