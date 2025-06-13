using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Astami.Data;
using Astami.Models;
using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Controllers
{
	[Authorize]
	public class ImmobileController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _environment;

		public ImmobileController(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			IWebHostEnvironment environment)
		{
			_context = context;
			_userManager = userManager;
			_environment = environment;
		}

		// GET: Lista immobili dell'agenzia
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			// Trova l'agenzia dell'utente corrente
			var agenzia = await _context.Agenzia
				.Include(a => a.Immobili)
				.ThenInclude(i => i.Immagini)
				.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

			if (agenzia == null)
			{
				TempData["ErrorMessage"] = "Agenzia non trovata.";
				return RedirectToAction("Dashboard", "Agency");
			}

			var viewModel = new ImmobiliIndexViewModel
			{
				Agenzia = agenzia,
				Immobili = agenzia.Immobili.OrderByDescending(i => i.DataCreazione).ToList()
			};

			return View(viewModel);
		}

		// GET: Form per aggiungere nuovo immobile
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

			var viewModel = new AggiungiImmobileViewModel
			{
				AgenziaId = agenzia.AgenziaId,
				TipoContratto = TipoContratto.Affitto,
				TipoImmobile = TipoImmobile.Appartamento,
				HasAscensore = false,
				HasBalcone = false,
				HasTerrazza = false,
				HasGiardino = false,
				HasParcheggio = false,
				HasCantina = false
			};

			return View(viewModel);
		}

		// POST: Aggiunge nuovo immobile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Aggiungi(AggiungiImmobileViewModel model)
		{
			if (!ModelState.IsValid)
			{
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
				// Crea l'immobile
				var immobile = new Immobile
				{
					ImmobileId = Guid.NewGuid(),
					Titolo = model.Titolo,
					Descrizione = model.Descrizione,
					Indirizzo = model.Indirizzo,
					Città = model.Città,
					Provincia = model.Provincia,
					CAP = model.CAP,
					TipoContratto = model.TipoContratto,
					TipoImmobile = model.TipoImmobile,
					PrezzoBase = model.PrezzoBase,
					Superficie = model.Superficie,
					NumeroLocali = model.NumeroLocali,
					NumeroBagni = model.NumeroBagni,
					Piano = model.Piano,
					HasAscensore = model.HasAscensore,
					HasBalcone = model.HasBalcone,
					HasTerrazza = model.HasTerrazza,
					HasGiardino = model.HasGiardino,
					HasParcheggio = model.HasParcheggio,
					HasCantina = model.HasCantina,
					ClasseEnergetica = model.ClasseEnergetica,
					SpeseCondominiali = model.SpeseCondominiali,
					Note = model.Note,
					DataCreazione = DateTime.UtcNow,
					Stato = StatoImmobile.Disponibile,
					IsPublished = false,
					AgenziaId = agenzia.AgenziaId,
					Agenzia = agenzia
				};

				_context.Immobile.Add(immobile);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				TempData["SuccessMessage"] = "Immobile aggiunto con successo! Ora puoi aggiungere le immagini.";
				return RedirectToAction("Dettagli", new { id = immobile.ImmobileId });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				ModelState.AddModelError("", "Si è verificato un errore durante l'aggiunta dell'immobile. Riprova.");
				return View(model);
			}
		}

		// GET: Dettagli immobile
		public async Task<IActionResult> Dettagli(Guid id)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var immobile = await _context.Immobile
				.Include(i => i.Agenzia)
				.Include(i => i.Immagini.OrderBy(img => img.Ordine))
				.Include(i => i.Documenti)
				.Include(i => i.Aste)
				.Include(i => i.Leads)
				.FirstOrDefaultAsync(i => i.ImmobileId == id);

			if (immobile == null)
			{
				TempData["ErrorMessage"] = "Immobile non trovato.";
				return RedirectToAction("Index");
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immobile.Agenzia.ApplicationUserId != user.Id)
			{
				TempData["ErrorMessage"] = "Non hai i permessi per visualizzare questo immobile.";
				return RedirectToAction("Index");
			}

			return View(immobile);
		}

		// GET: Form per modifica immobile
		public async Task<IActionResult> Modifica(Guid id)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var immobile = await _context.Immobile
				.Include(i => i.Agenzia)
				.FirstOrDefaultAsync(i => i.ImmobileId == id);

			if (immobile == null)
			{
				TempData["ErrorMessage"] = "Immobile non trovato.";
				return RedirectToAction("Index");
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immobile.Agenzia.ApplicationUserId != user.Id)
			{
				TempData["ErrorMessage"] = "Non hai i permessi per modificare questo immobile.";
				return RedirectToAction("Index");
			}

			var viewModel = new ModificaImmobileViewModel
			{
				ImmobileId = immobile.ImmobileId,
				AgenziaId = immobile.AgenziaId,
				Titolo = immobile.Titolo,
				Descrizione = immobile.Descrizione,
				Indirizzo = immobile.Indirizzo,
				Città = immobile.Città,
				Provincia = immobile.Provincia,
				CAP = immobile.CAP,
				TipoContratto = immobile.TipoContratto,
				TipoImmobile = immobile.TipoImmobile,
				PrezzoBase = immobile.PrezzoBase,
				Superficie = immobile.Superficie,
				NumeroLocali = immobile.NumeroLocali,
				NumeroBagni = immobile.NumeroBagni,
				Piano = immobile.Piano,
				HasAscensore = immobile.HasAscensore,
				HasBalcone = immobile.HasBalcone,
				HasTerrazza = immobile.HasTerrazza,
				HasGiardino = immobile.HasGiardino,
				HasParcheggio = immobile.HasParcheggio,
				HasCantina = immobile.HasCantina,
				ClasseEnergetica = immobile.ClasseEnergetica,
				SpeseCondominiali = immobile.SpeseCondominiali,
				Note = immobile.Note,
				Stato = immobile.Stato,
				IsPublished = immobile.IsPublished
			};

			return View(viewModel);
		}

		// POST: Modifica immobile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Modifica(ModificaImmobileViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}

			var immobile = await _context.Immobile
				.Include(i => i.Agenzia)
				.FirstOrDefaultAsync(i => i.ImmobileId == model.ImmobileId);

			if (immobile == null)
			{
				TempData["ErrorMessage"] = "Immobile non trovato.";
				return RedirectToAction("Index");
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immobile.Agenzia.ApplicationUserId != user.Id)
			{
				TempData["ErrorMessage"] = "Non hai i permessi per modificare questo immobile.";
				return RedirectToAction("Index");
			}

			try
			{
				// Aggiorna i dati dell'immobile
				immobile.Titolo = model.Titolo;
				immobile.Descrizione = model.Descrizione;
				immobile.Indirizzo = model.Indirizzo;
				immobile.Città = model.Città;
				immobile.Provincia = model.Provincia;
				immobile.CAP = model.CAP;
				immobile.TipoContratto = model.TipoContratto;
				immobile.TipoImmobile = model.TipoImmobile;
				immobile.PrezzoBase = model.PrezzoBase;
				immobile.Superficie = model.Superficie;
				immobile.NumeroLocali = model.NumeroLocali;
				immobile.NumeroBagni = model.NumeroBagni;
				immobile.Piano = model.Piano;
				immobile.HasAscensore = model.HasAscensore;
				immobile.HasBalcone = model.HasBalcone;
				immobile.HasTerrazza = model.HasTerrazza;
				immobile.HasGiardino = model.HasGiardino;
				immobile.HasParcheggio = model.HasParcheggio;
				immobile.HasCantina = model.HasCantina;
				immobile.ClasseEnergetica = model.ClasseEnergetica;
				immobile.SpeseCondominiali = model.SpeseCondominiali;
				immobile.Note = model.Note;
				immobile.Stato = model.Stato;
				immobile.IsPublished = model.IsPublished;
				immobile.DataModifica = DateTime.UtcNow;

				_context.Immobile.Update(immobile);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "Immobile modificato con successo!";
				return RedirectToAction("Dettagli", new { id = immobile.ImmobileId });
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "Si è verificato un errore durante la modifica dell'immobile. Riprova.");
				return View(model);
			}
		}

		// POST: Elimina immobile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Elimina(Guid immobileId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var immobile = await _context.Immobile
				.Include(i => i.Agenzia)
				.Include(i => i.Immagini)
				.Include(i => i.Documenti)
				.Include(i => i.Aste)
				.Include(i => i.Leads)
				.FirstOrDefaultAsync(i => i.ImmobileId == immobileId);

			if (immobile == null)
			{
				return Json(new { success = false, message = "Immobile non trovato" });
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immobile.Agenzia.ApplicationUserId != user.Id)
			{
				return Json(new { success = false, message = "Non hai i permessi per eliminare questo immobile" });
			}

			// Verifica che non ci siano aste attive
			if (immobile.Aste.Any(a => a.Stato == StatoAsta.Attiva))
			{
				return Json(new { success = false, message = "Impossibile eliminare l'immobile: ci sono aste attive" });
			}

			try
			{
				// Elimina tutte le entità correlate
				_context.ImmobileImmagine.RemoveRange(immobile.Immagini);
				_context.ImmobileDocumento.RemoveRange(immobile.Documenti);
				_context.Lead.RemoveRange(immobile.Leads);
				_context.Asta.RemoveRange(immobile.Aste);
				_context.Immobile.Remove(immobile);

				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = $"Immobile '{immobile.Titolo}' eliminato con successo"
				});
			}
			catch (Exception)
			{
				return Json(new { success = false, message = "Errore durante l'eliminazione dell'immobile" });
			}
		}

		// POST: Cambia stato pubblicazione
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CambiaStato(Guid immobileId, bool isPublished)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var immobile = await _context.Immobile
				.Include(i => i.Agenzia)
				.FirstOrDefaultAsync(i => i.ImmobileId == immobileId);

			if (immobile == null)
			{
				return Json(new { success = false, message = "Immobile non trovato" });
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immobile.Agenzia.ApplicationUserId != user.Id)
			{
				return Json(new { success = false, message = "Non hai i permessi per modificare questo immobile" });
			}

			try
			{
				immobile.IsPublished = isPublished;
				immobile.DataModifica = DateTime.UtcNow;

				_context.Immobile.Update(immobile);
				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = $"Immobile {(isPublished ? "pubblicato" : "rimosso dalla pubblicazione")} con successo",
					isPublished = isPublished
				});
			}
			catch (Exception)
			{
				return Json(new { success = false, message = "Errore durante il cambio stato" });
			}
		}

		// POST: Upload immagine
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadImmagine(Guid immobileId, IFormFile file, string descrizione)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var immobile = await _context.Immobile
				.Include(i => i.Agenzia)
				.Include(i => i.Immagini)
				.FirstOrDefaultAsync(i => i.ImmobileId == immobileId);

			if (immobile == null)
			{
				return Json(new { success = false, message = "Immobile non trovato" });
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immobile.Agenzia.ApplicationUserId != user.Id)
			{
				return Json(new { success = false, message = "Non hai i permessi per modificare questo immobile" });
			}

			if (file == null || file.Length == 0)
			{
				return Json(new { success = false, message = "Nessun file selezionato" });
			}

			// Verifica che sia un'immagine
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

			if (!allowedExtensions.Contains(extension))
			{
				return Json(new { success = false, message = "Formato file non supportato. Usa JPG, PNG, GIF o WebP" });
			}

			// Verifica dimensione (max 5MB)
			if (file.Length > 5 * 1024 * 1024)
			{
				return Json(new { success = false, message = "File troppo grande. Dimensione massima: 5MB" });
			}

			try
			{
				// Crea cartella se non esiste
				var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "immobili", immobileId.ToString());
				Directory.CreateDirectory(uploadsPath);

				// Genera nome file unico
				var fileName = $"{Guid.NewGuid()}{extension}";
				var filePath = Path.Combine(uploadsPath, fileName);

				// Salva file
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				// Crea record nel database
				var ordine = immobile.Immagini.Count > 0 ? immobile.Immagini.Max(i => i.Ordine) + 1 : 1;
				var isPrimary = !immobile.Immagini.Any();

				var immagine = new ImmobileImmagine
				{
					ImmobileImmagineId = Guid.NewGuid(),
					ImmobileId = immobileId,
					Url = $"/uploads/immobili/{immobileId}/{fileName}",
					Descrizione = descrizione,
					Ordine = ordine,
					IsPrimary = isPrimary,
					DataCaricamento = DateTime.UtcNow
				};

				_context.ImmobileImmagine.Add(immagine);
				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = "Immagine caricata con successo",
					immagineId = immagine.ImmobileImmagineId,
					url = immagine.Url,
					isPrimary = immagine.IsPrimary
				});
			}
			catch (Exception)
			{
				return Json(new { success = false, message = "Errore durante il caricamento dell'immagine" });
			}
		}

		// POST: Elimina immagine
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EliminaImmagine(Guid immagineId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Json(new { success = false, message = "Utente non autenticato" });
			}

			var immagine = await _context.ImmobileImmagine
				.Include(i => i.Immobile)
				.ThenInclude(im => im.Agenzia)
				.FirstOrDefaultAsync(i => i.ImmobileImmagineId == immagineId);

			if (immagine == null)
			{
				return Json(new { success = false, message = "Immagine non trovata" });
			}

			// Verifica che l'utente corrente sia il proprietario dell'agenzia
			if (immagine.Immobile.Agenzia.ApplicationUserId != user.Id)
			{
				return Json(new { success = false, message = "Non hai i permessi per eliminare questa immagine" });
			}

			try
			{
				// Elimina file fisico
				var filePath = Path.Combine(_environment.WebRootPath, immagine.Url.TrimStart('/'));
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}

				// Elimina record dal database
				_context.ImmobileImmagine.Remove(immagine);
				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = "Immagine eliminata con successo"
				});
			}
			catch (Exception)
			{
				return Json(new { success = false, message = "Errore durante l'eliminazione dell'immagine" });
			}
		}
	}

	// ViewModel per la lista immobili
	public class ImmobiliIndexViewModel
	{
		public Agenzia Agenzia { get; set; } = null!;
		public List<Immobile> Immobili { get; set; } = new List<Immobile>();
	}

	// ViewModel per aggiungere immobile
	public class AggiungiImmobileViewModel
	{
		public Guid AgenziaId { get; set; }

		[Required(ErrorMessage = "Il titolo è obbligatorio")]
		[StringLength(300, ErrorMessage = "Il titolo non può superare i 300 caratteri")]
		public string Titolo { get; set; } = string.Empty;

		[StringLength(2000, ErrorMessage = "La descrizione non può superare i 2000 caratteri")]
		public string? Descrizione { get; set; }

		[Required(ErrorMessage = "L'indirizzo è obbligatorio")]
		[StringLength(300, ErrorMessage = "L'indirizzo non può superare i 300 caratteri")]
		public string Indirizzo { get; set; } = string.Empty;

		[Required(ErrorMessage = "La città è obbligatoria")]
		[StringLength(100, ErrorMessage = "La città non può superare i 100 caratteri")]
		public string Città { get; set; } = string.Empty;

		[StringLength(100, ErrorMessage = "La provincia non può superare i 100 caratteri")]
		public string? Provincia { get; set; }

		[Required(ErrorMessage = "Il CAP è obbligatorio")]
		[StringLength(10, ErrorMessage = "Il CAP non può superare i 10 caratteri")]
		[RegularExpression(@"^\d{5}$", ErrorMessage = "Il CAP deve essere composto da 5 cifre")]
		public string CAP { get; set; } = string.Empty;

		[Required(ErrorMessage = "Il tipo di contratto è obbligatorio")]
		public TipoContratto TipoContratto { get; set; }

		[Required(ErrorMessage = "Il tipo di immobile è obbligatorio")]
		public TipoImmobile TipoImmobile { get; set; }

		[Required(ErrorMessage = "Il prezzo è obbligatorio")]
		[Range(0, double.MaxValue, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
		public decimal PrezzoBase { get; set; }

		[Range(0, 1000, ErrorMessage = "La superficie deve essere tra 0 e 1000 m²")]
		public int? Superficie { get; set; }

		[Range(0, 20, ErrorMessage = "Il numero di locali deve essere tra 0 e 20")]
		public int? NumeroLocali { get; set; }

		[Range(0, 10, ErrorMessage = "Il numero di bagni deve essere tra 0 e 10")]
		public int? NumeroBagni { get; set; }

		[Range(0, 50, ErrorMessage = "Il piano deve essere tra 0 e 50")]
		public int? Piano { get; set; }

		public bool HasAscensore { get; set; }
		public bool HasBalcone { get; set; }
		public bool HasTerrazza { get; set; }
		public bool HasGiardino { get; set; }
		public bool HasParcheggio { get; set; }
		public bool HasCantina { get; set; }

		[StringLength(50, ErrorMessage = "La classe energetica non può superare i 50 caratteri")]
		public string? ClasseEnergetica { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Le spese condominiali devono essere maggiori o uguali a 0")]
		public decimal? SpeseCondominiali { get; set; }

		[StringLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
		public string? Note { get; set; }
	}

	// ViewModel per modificare immobile
	public class ModificaImmobileViewModel : AggiungiImmobileViewModel
	{
		public Guid ImmobileId { get; set; }
		public StatoImmobile Stato { get; set; }
		public bool IsPublished { get; set; }
	}
}