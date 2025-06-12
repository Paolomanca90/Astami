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
	public class CollaboratoreController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public CollaboratoreController(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		// GET: Lista collaboratori dell'agenzia
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			// Trova l'agenzia dell'utente corrente
			var agenzia = await _context.Agenzia
				.Include(a => a.AgenziaUtenti)
				.ThenInclude(au => au.ApplicationUser)
				.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

			if (agenzia == null)
			{
				TempData["ErrorMessage"] = "Agenzia non trovata.";
				return RedirectToAction("Dashboard", "Agency");
			}

			// Verifica che l'utente sia il proprietario dell'agenzia
			if (agenzia.ApplicationUserId != user.Id)
			{
				TempData["ErrorMessage"] = "Non hai i permessi per gestire i collaboratori di questa agenzia.";
				return RedirectToAction("Dashboard", "Agency");
			}

			var viewModel = new CollaboratoriIndexViewModel
			{
				Agenzia = agenzia,
				Collaboratori = agenzia.AgenziaUtenti.Where(au => au.IsActive).ToList()
			};

			return View(viewModel);
		}

		// GET: Form per aggiungere un nuovo collaboratore
		public async Task<IActionResult> Aggiungi()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var agenzia = await _context.Agenzia
				.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

			if (agenzia == null)
			{
				TempData["ErrorMessage"] = "Agenzia non trovata.";
				return RedirectToAction("Dashboard", "Agency");
			}

			var viewModel = new AggiungiCollaboratoreViewModel
			{
				AgenziaId = agenzia.AgenziaId,
				RuoliDisponibili = new List<string>
				{
					RolesConstants.Manager,
					RolesConstants.Agente,
					RolesConstants.Collaboratore
				}
			};

			return View(viewModel);
		}

		// POST: Aggiunge un nuovo collaboratore
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Aggiungi(AggiungiCollaboratoreViewModel model)
		{
			if (!ModelState.IsValid)
			{
				model.RuoliDisponibili = new List<string>
				{
					RolesConstants.Manager,
					RolesConstants.Agente,
					RolesConstants.Collaboratore
				};
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var agenzia = await _context.Agenzia
				.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id && a.AgenziaId == model.AgenziaId);

			if (agenzia == null)
			{
				TempData["ErrorMessage"] = "Agenzia non trovata o non hai i permessi necessari.";
				return RedirectToAction("Index");
			}

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				ApplicationUser collaboratore;
				bool isNewUser = false;

				// Verifica se l'utente esiste già
				collaboratore = await _userManager.FindByEmailAsync(model.Email);

				if (collaboratore == null)
				{
					// Crea nuovo utente
					collaboratore = new ApplicationUser
					{
						UserName = model.Email,
						Email = model.Email,
						Nome = model.Nome,
						Cognome = model.Cognome,
						PhoneNumber = model.Telefono,
						EmailConfirmed = true // Conferma automaticamente
					};

					var result = await _userManager.CreateAsync(collaboratore, model.Password);
					if (!result.Succeeded)
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
						model.RuoliDisponibili = new List<string>
						{
							RolesConstants.Manager,
							RolesConstants.Agente,
							RolesConstants.Collaboratore
						};
						return View(model);
					}

					isNewUser = true;
				}
				else
				{
					// Verifica se l'utente è già associato a questa agenzia
					var esisteGia = await _context.AgenziaUtente
						.AnyAsync(au => au.AgenziaId == agenzia.AgenziaId &&
									   au.ApplicationUserId == collaboratore.Id &&
									   au.IsActive);

					if (esisteGia)
					{
						ModelState.AddModelError("Email", "Questo utente è già un collaboratore di questa agenzia.");
						model.RuoliDisponibili = new List<string>
						{
							RolesConstants.Manager,
							RolesConstants.Agente,
							RolesConstants.Collaboratore
						};
						return View(model);
					}

					// Aggiorna i dati dell'utente esistente se necessario
					if (string.IsNullOrEmpty(collaboratore.Nome) && !string.IsNullOrEmpty(model.Nome))
						collaboratore.Nome = model.Nome;
					if (string.IsNullOrEmpty(collaboratore.Cognome) && !string.IsNullOrEmpty(model.Cognome))
						collaboratore.Cognome = model.Cognome;
					if (string.IsNullOrEmpty(collaboratore.PhoneNumber) && !string.IsNullOrEmpty(model.Telefono))
						collaboratore.PhoneNumber = model.Telefono;

					await _userManager.UpdateAsync(collaboratore);
				}

				// Assegna il ruolo al collaboratore
				if (!await _userManager.IsInRoleAsync(collaboratore, model.Ruolo))
				{
					await _userManager.AddToRoleAsync(collaboratore, model.Ruolo);
				}

				// Crea la relazione AgenziaUtente
				var agenziaUtente = new AgenziaUtente
				{
					AgenziaUtenteId = Guid.NewGuid(),
					AgenziaId = agenzia.AgenziaId,
					ApplicationUserId = collaboratore.Id,
					Agenzia = agenzia,
					ApplicationUser = collaboratore,
					Ruolo = model.Ruolo,
					DataAssegnazione = DateTime.UtcNow,
					IsActive = true
				};

				_context.AgenziaUtente.Add(agenziaUtente);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				TempData["SuccessMessage"] = $"Collaboratore {(isNewUser ? "creato e" : "")} aggiunto con successo!";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				ModelState.AddModelError("", "Si è verificato un errore durante l'aggiunta del collaboratore. Riprova.");
				model.RuoliDisponibili = new List<string>
				{
					RolesConstants.Manager,
					RolesConstants.Agente,
					RolesConstants.Collaboratore
				};
				return View(model);
			}
		}

		// POST: Rimuove un collaboratore
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Rimuovi(Guid agenziaUtenteId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var agenziaUtente = await _context.AgenziaUtente
				.Include(au => au.Agenzia)
				.Include(au => au.ApplicationUser)
				.FirstOrDefaultAsync(au => au.AgenziaUtenteId == agenziaUtenteId);

			if (agenziaUtente == null)
			{
				return Json(new { success = false, message = "Collaboratore non trovato" });
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (agenziaUtente.Agenzia.ApplicationUserId != user.Id)
			{
				return Json(new { success = false, message = "Non hai i permessi per rimuovere questo collaboratore" });
			}

			// Non permettere di rimuovere se stesso
			if (agenziaUtente.ApplicationUserId == user.Id)
			{
				return Json(new { success = false, message = "Non puoi rimuovere te stesso dall'agenzia" });
			}

			try
			{
				// Disattiva la relazione invece di eliminarla
				agenziaUtente.IsActive = false;
				_context.AgenziaUtente.Update(agenziaUtente);
				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = $"Collaboratore {agenziaUtente.ApplicationUser.Nome} {agenziaUtente.ApplicationUser.Cognome} rimosso con successo"
				});
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Errore durante la rimozione del collaboratore" });
			}
		}

		// GET: Dettagli di un collaboratore
		public async Task<IActionResult> Dettagli(Guid agenziaUtenteId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var agenziaUtente = await _context.AgenziaUtente
				.Include(au => au.Agenzia)
				.Include(au => au.ApplicationUser)
				.FirstOrDefaultAsync(au => au.AgenziaUtenteId == agenziaUtenteId);

			if (agenziaUtente == null)
			{
				TempData["ErrorMessage"] = "Collaboratore non trovato.";
				return RedirectToAction("Index");
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (agenziaUtente.Agenzia.ApplicationUserId != user.Id)
			{
				TempData["ErrorMessage"] = "Non hai i permessi per visualizzare questo collaboratore.";
				return RedirectToAction("Index");
			}

			return View(agenziaUtente);
		}

		// POST: Cambia ruolo di un collaboratore
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CambiaRuolo(Guid agenziaUtenteId, string nuovoRuolo)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var agenziaUtente = await _context.AgenziaUtente
				.Include(au => au.Agenzia)
				.Include(au => au.ApplicationUser)
				.FirstOrDefaultAsync(au => au.AgenziaUtenteId == agenziaUtenteId);

			if (agenziaUtente == null)
			{
				return Json(new { success = false, message = "Collaboratore non trovato" });
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (agenziaUtente.Agenzia.ApplicationUserId != user.Id)
			{
				return Json(new { success = false, message = "Non hai i permessi per modificare questo collaboratore" });
			}

			// Verifica che il nuovo ruolo sia valido
			var ruoliValidi = new[] { RolesConstants.Manager, RolesConstants.Agente, RolesConstants.Collaboratore };
			if (!ruoliValidi.Contains(nuovoRuolo))
			{
				return Json(new { success = false, message = "Ruolo non valido" });
			}

			try
			{
				// Rimuovi il ruolo precedente e assegna il nuovo
				var ruoliAttuali = await _userManager.GetRolesAsync(agenziaUtente.ApplicationUser);
				var ruoliDaRimuovere = ruoliAttuali.Where(r => ruoliValidi.Contains(r)).ToList();

				if (ruoliDaRimuovere.Any())
				{
					await _userManager.RemoveFromRolesAsync(agenziaUtente.ApplicationUser, ruoliDaRimuovere);
				}

				await _userManager.AddToRoleAsync(agenziaUtente.ApplicationUser, nuovoRuolo);

				// Aggiorna il ruolo nella tabella AgenziaUtente
				agenziaUtente.Ruolo = nuovoRuolo;
				_context.AgenziaUtente.Update(agenziaUtente);
				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = $"Ruolo cambiato con successo a {nuovoRuolo}",
					nuovoRuolo = nuovoRuolo
				});
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Errore durante il cambio del ruolo" });
			}
		}
	}

	// ViewModel per la lista collaboratori
	public class CollaboratoriIndexViewModel
	{
		public Agenzia Agenzia { get; set; } = null!;
		public List<AgenziaUtente> Collaboratori { get; set; } = new List<AgenziaUtente>();
	}

	// ViewModel per aggiungere collaboratore
	public class AggiungiCollaboratoreViewModel
	{
		public Guid AgenziaId { get; set; }

		[Required(ErrorMessage = "Il nome è obbligatorio")]
		[StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
		public string Nome { get; set; } = string.Empty;

		[Required(ErrorMessage = "Il cognome è obbligatorio")]
		[StringLength(100, ErrorMessage = "Il cognome non può superare i 100 caratteri")]
		public string Cognome { get; set; } = string.Empty;

		[Required(ErrorMessage = "L'email è obbligatoria")]
		[EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
		[StringLength(200, ErrorMessage = "L'email non può superare i 200 caratteri")]
		public string Email { get; set; } = string.Empty;

		[StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri")]
		[Phone(ErrorMessage = "Inserisci un numero di telefono valido")]
		public string? Telefono { get; set; }

		[Required(ErrorMessage = "La password è obbligatoria")]
		[StringLength(100, ErrorMessage = "La password deve essere lunga almeno {2} caratteri.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Il ruolo è obbligatorio")]
		public string Ruolo { get; set; } = RolesConstants.Agente;

		public List<string> RuoliDisponibili { get; set; } = new List<string>();
	}
}