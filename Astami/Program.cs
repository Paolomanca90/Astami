// Program.cs
using Astami.Data;
using Astami.Models;
using Astami.Utilities.Roles;
using Astami.Utilities.Subscriptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
	throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurazione Identity con ApplicationUser personalizzato
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	// Configurazione password
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = true;
	options.Password.RequireLowercase = true;

	// Configurazione utente
	options.User.RequireUniqueEmail = true;

	// Configurazione lockout
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
	options.Lockout.MaxFailedAccessAttempts = 5;

	// Configurazione conferma email (disabilitata per sviluppo)
	options.SignIn.RequireConfirmedAccount = false;
	options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Aggiungere servizi MVC
builder.Services.AddControllersWithViews();

// Configurazione per servizi aggiuntivi
builder.Services.AddScoped<IEmailSender, EmailSender>(); // Da implementare per invio email

var app = builder.Build();

// Inizializzazione del database e seeding
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<ApplicationDbContext>();

		// Assicurarsi che il database sia creato
		context.Database.EnsureCreated();

		// Eseguire le migrazioni se necessario
		if (context.Database.GetPendingMigrations().Any())
		{
			context.Database.Migrate();
		}

		// Inizializzare ruoli
		await RolesInitializer.InitializeRoles(services);

		// Inizializzare abbonamenti
		await SubscriptionsInitializer.InitializeSubscriptions(context);

		// Creare utente admin di default se non esiste
		await CreateDefaultAdminUser(services);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred creating the DB.");
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// Metodo per creare utente admin di default
static async Task CreateDefaultAdminUser(IServiceProvider serviceProvider)
{
	var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
	var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	string adminEmail = "admin@astami.co";
	string adminPassword = "Admin123!";

	if (await userManager.FindByEmailAsync(adminEmail) == null)
	{
		var adminUser = new ApplicationUser
		{
			UserName = adminEmail,
			Email = adminEmail,
			EmailConfirmed = true,
			Nome = "Admin",
			Cognome = "Astami",
			DataRegistrazione = DateTime.UtcNow
		};

		var result = await userManager.CreateAsync(adminUser, adminPassword);

		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(adminUser, Astami.Utilities.Constants.RolesConstants.Admin);
		}
	}
}

// Implementazione temporanea del servizio Email
public class EmailSender : IEmailSender
{
	public Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		// TODO: Implementare invio email reale (SendGrid, SMTP, etc.)
		// Per ora log nel console
		Console.WriteLine($"Sending email to {email}: {subject}");
		return Task.CompletedTask;
	}
}