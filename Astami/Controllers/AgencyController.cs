using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Astami.Data;
using Astami.Models;
using Astami.Utilities.Constants;
using System.ComponentModel.DataAnnotations;

namespace Astami.Controllers
{
	[Authorize]
	public class AgencyController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AgencyController(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> RegisterAgency()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user != null)
			{			
				// Verifica se l'utente ha già un'agenzia
				var agenziaEsistente = await _context.Agenzia
					.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

				if (agenziaEsistente != null)
				{
					return RedirectToAction("Dashboard", "Agency");
				}
			}

			var piani = await _context.Abbonamento.ToListAsync();

			var viewModel = new AgencyRegistrationViewModel
			{
				PianoSelezionato = piani.FirstOrDefault(x => x.AbbonamentoId == (Guid)TempData["PianoSelezionatoId"]),
				AvailablePlans = piani
			};

			TempData["PianoSelezionatoId"] = viewModel.PianoSelezionato.AbbonamentoId;

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> RegisterAgency(AgencyRegistrationViewModel model)
		{
			if (!ModelState.IsValid)
			{
				model.AvailablePlans = await _context.Abbonamento.ToListAsync();
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			// Verifica se l'utente ha già un'agenzia
			var agenziaEsistente = await _context.Agenzia
				.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

			if (agenziaEsistente != null)
			{
				return RedirectToAction("Dashboard", "Agency");
			}

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// Crea l'agenzia
				var agenzia = new Agenzia
				{
					AgenziaId = Guid.NewGuid(),
					RagioneSociale = model.RagioneSociale,
					Descrizione = model.Descrizione,
					Indirizzo = model.Indirizzo,
					Città = model.Città,
					Provincia = model.Provincia,
					Regione = model.Regione,
					CAP = model.CAP,
					Telefono = model.Telefono,
					Email = model.Email,
					SitoWeb = model.SitoWeb,
					PartitaIVA = model.PartitaIVA,
					SDI = model.SDI,
					IsPublic = model.IsPublic,
					PinAccesso = model.IsPublic ? null : model.PinAccesso,
					ApplicationUserId = user.Id,
					ApplicationUser = user,
					AbbonamentoId = model.AbbonamentoId,
					Abbonamento = await _context.Abbonamento.FindAsync(model.AbbonamentoId),				
					DataRegistrazione = DateTime.UtcNow,
					IsActive = true
				};

				_context.Agenzia.Add(agenzia);

				// Crea la relazione AgenziaUtente come proprietario
				var agenziaUtente = new AgenziaUtente
				{
					AgenziaUtenteId = Guid.NewGuid(),
					AgenziaId = agenzia.AgenziaId,
					ApplicationUserId = user.Id,
					Agenzia = agenzia,
					ApplicationUser = user,
					Ruolo = RolesConstants.Manager,
					DataAssegnazione = DateTime.UtcNow,
					IsActive = true
				};

				_context.AgenziaUtente.Add(agenziaUtente);

				// Assegna il ruolo Partner all'utente se non ce l'ha già
				if (!await _userManager.IsInRoleAsync(user, RolesConstants.Partner))
				{
					await _userManager.AddToRoleAsync(user, RolesConstants.Partner);
				}

				// Conferma il piano selezionato se presente
				var pianoSelezionato = await _context.PianoSelezionato
					.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id && !p.Confermato);

				if (pianoSelezionato != null && pianoSelezionato.AbbonamentoId == model.AbbonamentoId)
				{
					pianoSelezionato.Confermato = true;
					_context.Update(pianoSelezionato);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				TempData["SuccessMessage"] = "Agenzia registrata con successo! Benvenuto in ASTAMI.";
				return RedirectToAction("Dashboard", "Agency");
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				ModelState.AddModelError("", "Si è verificato un errore durante la registrazione dell'agenzia. Riprova.");
				model.AvailablePlans = await _context.Abbonamento.ToListAsync();
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Dashboard()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var agenzia = await _context.Agenzia
				.Include(a => a.Abbonamento)
				.Include(a => a.AgenziaUtenti)
				.ThenInclude(au => au.ApplicationUser)
				.Include(a => a.Immobili)
				.Include(a => a.Leads)
				.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

			if (agenzia == null)
			{
				return RedirectToAction("RegisterAgency");
			}

			return View(agenzia);
		}

		// Metodo per la selezione piano dalla registrazione agenzia
		[HttpPost]
		public async Task<IActionResult> SelectPlanForAgency(Guid abbonamentoId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var abbonamento = await _context.Abbonamento.FindAsync(abbonamentoId);
			if (abbonamento == null)
			{
				return Json(new { success = false, message = "Piano non trovato" });
			}

			// Aggiorna o crea la selezione del piano
			var pianoEsistente = await _context.PianoSelezionato
				.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

			if (pianoEsistente != null)
			{
				pianoEsistente.AbbonamentoId = abbonamentoId;
				pianoEsistente.Confermato = false;
				_context.Update(pianoEsistente);
			}
			else
			{
				var nuovoPiano = new PianoSelezionato
				{
					PianoSelezionatoId = Guid.NewGuid(),
					ApplicationUserId = user.Id,
					AbbonamentoId = abbonamentoId,
					Abbonamento = abbonamento,
					Confermato = false
				};
				_context.PianoSelezionato.Add(nuovoPiano);
			}

			await _context.SaveChangesAsync();

			return Json(new
			{
				success = true,
				planName = abbonamento.Nome,
				planPrice = abbonamento.Prezzo,
				message = "Piano selezionato con successo"
			});
		}
	}

	// ViewModel per la registrazione agenzia
	public class AgencyRegistrationViewModel
	{
		public ApplicationUser User { get; set; }
		public Abbonamento PianoSelezionato { get; set; }
		public List<Abbonamento> AvailablePlans { get; set; } = new List<Abbonamento>();

		[Required(ErrorMessage = "La ragione sociale è obbligatoria")]
		[StringLength(200, ErrorMessage = "La ragione sociale non può superare i 200 caratteri")]
		public string RagioneSociale { get; set; } = string.Empty;

		[StringLength(1000, ErrorMessage = "La descrizione non può superare i 1000 caratteri")]
		public string? Descrizione { get; set; }

		[Required(ErrorMessage = "L'indirizzo è obbligatorio")]
		[StringLength(300, ErrorMessage = "L'indirizzo non può superare i 300 caratteri")]
		public string Indirizzo { get; set; } = string.Empty;

		[Required(ErrorMessage = "La città è obbligatoria")]
		[StringLength(100, ErrorMessage = "La città non può superare i 100 caratteri")]
		public string Città { get; set; } = string.Empty;

		[Required(ErrorMessage = "La provincia è obbligatoria")]
		[StringLength(100, ErrorMessage = "La provincia non può superare i 100 caratteri")]
		public string Provincia { get; set; } = string.Empty;

		[Required(ErrorMessage = "La regione è obbligatoria")]
		[StringLength(100, ErrorMessage = "La regione non può superare i 100 caratteri")]
		public string Regione { get; set; } = string.Empty;

		[Required(ErrorMessage = "Il CAP è obbligatorio")]
		[StringLength(10, ErrorMessage = "Il CAP non può superare i 10 caratteri")]
		[RegularExpression(@"^\d{5}$", ErrorMessage = "Il CAP deve essere composto da 5 cifre")]
		public string CAP { get; set; } = string.Empty;

		[StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri")]
		[Phone(ErrorMessage = "Inserisci un numero di telefono valido")]
		public string? Telefono { get; set; }

		[StringLength(200, ErrorMessage = "L'email non può superare i 200 caratteri")]
		[EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
		public string? Email { get; set; }

		[StringLength(200, ErrorMessage = "Il sito web non può superare i 200 caratteri")]
		[Url(ErrorMessage = "Inserisci un URL valido")]
		public string? SitoWeb { get; set; }

		[StringLength(20, ErrorMessage = "La Partita IVA non può superare i 20 caratteri")]
		[RegularExpression(@"^[0-9]{11}$", ErrorMessage = "La Partita IVA deve essere composta da 11 cifre")]
		public string? PartitaIVA { get; set; }

		[StringLength(20, ErrorMessage = "Il codice SDI non può superare i 20 caratteri")]
		public string? SDI { get; set; }

		public bool IsPublic { get; set; } = true;

		[StringLength(100, ErrorMessage = "Il PIN non può superare i 100 caratteri")]
		public string? PinAccesso { get; set; }

		[Required(ErrorMessage = "Devi selezionare un piano")]
		public Guid AbbonamentoId { get; set; }

		public bool AccettaTermini { get; set; }
		public bool AccettaPrivacy { get; set; }
	}
}