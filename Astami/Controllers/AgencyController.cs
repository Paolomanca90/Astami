using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Astami.Data;
using Astami.Models;
using Astami.Utilities.Constants;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Stripe.Checkout;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;
using Stripe;
using Session = Stripe.Checkout.Session;

namespace Astami.Controllers
{
	public class AgencyController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AgencyController(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			SignInManager<ApplicationUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> RegisterAgency(Guid? planId = null)
		{
			// Se l'utente è già autenticato, verifica se ha già un'agenzia
			if (User.Identity.IsAuthenticated)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user != null)
				{
					var agenziaEsistente = await _context.Agenzia
						.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

					if (agenziaEsistente != null)
					{
						return RedirectToAction("Dashboard", "Agency");
					}
				}
			}

			var piani = await _context.Abbonamento.ToListAsync();

			// Determina il piano selezionato
			Abbonamento pianoSelezionato = null;

			if (planId.HasValue)
			{
				pianoSelezionato = piani.FirstOrDefault(x => x.AbbonamentoId == planId.Value);
			}
			else if (TempData["PianoSelezionatoId"] != null && Guid.TryParse(TempData["PianoSelezionatoId"].ToString(), out Guid tempPlanId))
			{
				pianoSelezionato = piani.FirstOrDefault(x => x.AbbonamentoId == tempPlanId);
			}

			// Se non c'è un piano selezionato, usa il primo disponibile
			pianoSelezionato ??= piani.FirstOrDefault();

			var viewModel = new AgencyRegistrationViewModel
			{
				PianoSelezionato = pianoSelezionato,
				AvailablePlans = piani,
				AbbonamentoId = pianoSelezionato?.AbbonamentoId ?? Guid.Empty,
				IsUserAuthenticated = User.Identity.IsAuthenticated
			};

			// Mantieni il piano selezionato in TempData
			if (pianoSelezionato != null)
			{
				TempData["PianoSelezionatoId"] = pianoSelezionato.AbbonamentoId;
			}

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
				model.IsUserAuthenticated = User.Identity.IsAuthenticated;
				return View(model);
			}

			ApplicationUser user = null;
			bool isNewUser = false;

			// Se l'utente non è autenticato, deve creare un nuovo account
			if (!User.Identity.IsAuthenticated)
			{
				// Verifica che i dati utente siano forniti
				if (string.IsNullOrEmpty(model.UserEmail) || string.IsNullOrEmpty(model.UserPassword))
				{
					ModelState.AddModelError("", "Email e password sono obbligatori per creare un nuovo account.");
					model.AvailablePlans = await _context.Abbonamento.ToListAsync();
					model.IsUserAuthenticated = false;
					return View(model);
				}

				// Verifica che l'email non sia già in uso
				var existingUser = await _userManager.FindByEmailAsync(model.UserEmail);
				if (existingUser != null)
				{
					ModelState.AddModelError("UserEmail", "Questa email è già registrata. Effettua il login prima di registrare l'agenzia.");
					model.AvailablePlans = await _context.Abbonamento.ToListAsync();
					model.IsUserAuthenticated = false;
					return View(model);
				}

				// Crea nuovo utente
				user = new ApplicationUser
				{
					UserName = model.UserEmail,
					Email = model.UserEmail,
					Nome = model.UserNome,
					Cognome = model.UserCognome,
					EmailConfirmed = true // Conferma automaticamente per semplificare
				};

				isNewUser = true;
			}
			else
			{
				// Utente già autenticato
				user = await _userManager.GetUserAsync(User);
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
			}

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// Se è un nuovo utente, crealo nel database
				if (isNewUser)
				{
					var result = await _userManager.CreateAsync(user, model.UserPassword);
					if (!result.Succeeded)
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
						model.AvailablePlans = await _context.Abbonamento.ToListAsync();
						model.IsUserAuthenticated = false;
						return View(model);
					}

					// Assegna il ruolo Manager al nuovo utente
					await _userManager.AddToRoleAsync(user, RolesConstants.Manager);
				}

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
					IsActive = false
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
					Ruolo = RolesConstants.Partner,
					DataAssegnazione = DateTime.UtcNow,
					IsActive = true
				};

				_context.AgenziaUtente.Add(agenziaUtente);

				// Assegna il ruolo Partner all'utente se non ce l'ha già
				if (!await _userManager.IsInRoleAsync(user, RolesConstants.Partner))
				{
					await _userManager.AddToRoleAsync(user, RolesConstants.Partner);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				// Se è un nuovo utente, effettua il login automatico
				if (isNewUser)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);
				}

				return RedirectToAction("CreatCheckoutSession", new {id = user.Id, abbonamentoId = model.AbbonamentoId});
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				ModelState.AddModelError("", "Si è verificato un errore durante la registrazione dell'agenzia. Riprova.");
				model.AvailablePlans = await _context.Abbonamento.ToListAsync();
				model.IsUserAuthenticated = User.Identity.IsAuthenticated;
				return View(model);
			}
		}

		public IActionResult CreateCheckoutSession(string id, Guid abbonamentoId)
		{
			if (id == null)
				return BadRequest("Utente non loggato");
			ViewBag.AbbonamentoId = abbonamentoId;
			return View();
		}

		[HttpPost]
		public IActionResult CreateCheckoutSession(Guid abbonamentoId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return BadRequest("L'utente non esiste.");
			}

			var piano = _context.Abbonamento.FirstOrDefault(x => x.AbbonamentoId == abbonamentoId);
			if (piano == null)
			{
				return BadRequest("Piano non trovato.");
			}

			var total = piano.Prezzo;
			var agenziaId = _context.Agenzia.OrderByDescending(x => x.DataRegistrazione).FirstOrDefault(x => x.ApplicationUserId == userId && x.IsActive == false)?.AgenziaId;
			if (agenziaId == null)
			{
				return BadRequest("Agenzia non trovata.");
			}

			var customerId = CreateOrGetStripeCustomer();

			var options = new SessionCreateOptions
			{
				Customer = customerId,
				PaymentMethodTypes = new List<string> { "card", "klarna", "paypal", "samsung_pay", "sepa_debit" },
				LineItems = new List<SessionLineItemOptions>
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = Convert.ToInt64(total * 100),
							Currency = "eur",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = "Registrazione Astami",
								Metadata = new Dictionary<string, string>
								{
									{"P_IVA", GetCustomerMetadata("P_IVA")},
									{"AgenziaId", agenziaId.ToString()}
								}
							},
						},
						Quantity = 1,
					},
				},
				Mode = "payment",
				SuccessUrl = $"{Request.Scheme}://{Request.Host}/Partner/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}",
				CancelUrl = Url.Action("PaymentCancel", "Partner", null, Request.Scheme),
				PaymentIntentData = new SessionPaymentIntentDataOptions
				{
					Metadata = new Dictionary<string, string>
					{
						{"P_IVA", GetCustomerMetadata("P_IVA")},
						{"AgenziaId", agenziaId.ToString()}
					}
				}
			};

			var service = new SessionService();
			var session = service.Create(options);

			return Json(new { url = session.Url });
		}

		private string CreateOrGetStripeCustomer()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var agenzia = _context.Agenzia.OrderByDescending(x => x.DataRegistrazione).FirstOrDefault(x => x.ApplicationUserId == userId && x.IsActive == false);

			var customerService = new CustomerService();

			var searchOptions = new CustomerSearchOptions
			{
				Query = $"email:'{agenzia?.Email}' AND metadata['P_IVA']:'{agenzia?.PartitaIVA}'"
			};

			var existingCustomers = customerService.Search(searchOptions);
			if (existingCustomers.Any())
				return existingCustomers.First().Id;

			var customerOptions = new CustomerCreateOptions
			{
				Email = agenzia?.Email,
				Name = agenzia?.RagioneSociale,
				Address = new AddressOptions
				{
					City = agenzia?.Città,
					Country = "IT",
					Line1 = agenzia?.Indirizzo,
					PostalCode = agenzia?.CAP,
					State = agenzia?.Provincia
				},
				Phone = agenzia?.Telefono,
				Metadata = new Dictionary<string, string>
				{
					{"P_IVA", agenzia?.PartitaIVA}
				}
			};

			var newCustomer = customerService.Create(customerOptions);
			return newCustomer.Id;
		}

		private string GetCustomerMetadata(string key)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var agenzia = _context.Agenzia.OrderByDescending(x => x.DataRegistrazione).FirstOrDefault(x => x.ApplicationUserId == userId && x.IsActive == false);
			return key switch
			{
				"P_IVA" => agenzia?.PartitaIVA,
				_ => string.Empty
			};
		}

		public IActionResult PaymentSuccess(string sessionId)
		{
			var service = new SessionService();
			Session session = service.Get(sessionId);

			if (session.PaymentStatus == "paid")
			{
				var paymentIntentService = new PaymentIntentService();
				PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var agenzia = _context.Agenzia.OrderByDescending(x => x.DataRegistrazione).FirstOrDefault(x => x.ApplicationUserId == userId && x.IsActive == false);
				if (agenzia != null)
				{
					agenzia.IsActive = true;
					_context.Agenzia.Update(agenzia);
					var pianoSelezionato = _context.PianoSelezionato.FirstOrDefault(x => x.ApplicationUserId == userId && x.Confermato == false);
					if (pianoSelezionato == null)
					{
						ViewBag.Errore = "Errore: Piano selezionato non trovato.";
						return RedirectToAction("Index", "Pricing");
					}

					pianoSelezionato.Confermato = true;
					_context.PianoSelezionato.Update(pianoSelezionato);

					TempData["SuccessMessage"] = "Agenzia registrata con successo! Benvenuto in ASTAMI.";
					return RedirectToAction("Dashboard");
				}
				else
				{
					TempData["Errore"] = "Errore: Agenzia non trovata.";
				}
			}
			else
			{
				TempData["Errore"] = "Errore: Il pagamento non è stato completato.";
			}

			return RedirectToAction("Index", "Pricing");
		}

		public IActionResult PaymentCancel()
		{
			TempData["Errore"] = "Pagamento annullato.";
			return RedirectToAction("Index", "Pricing");
		}

		[HttpGet]
		[Authorize]
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

			ViewBag.Nome = user.Nome + " " + user.Cognome;
			return View(agenzia);
		}

		// Metodo per la selezione piano dalla registrazione agenzia
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> SelectPlanForAgency(Guid abbonamentoId)
		{
			var abbonamento = await _context.Abbonamento.FindAsync(abbonamentoId);
			if (abbonamento == null)
			{
				return Json(new { success = false, message = "Piano non trovato" });
			}

			// Salva la selezione del piano in TempData (funziona sia per utenti autenticati che non)
			TempData["PianoSelezionatoId"] = abbonamentoId;

			return Json(new
			{
				success = true,
				planName = abbonamento.Nome,
				planPrice = abbonamento.Prezzo,
				message = "Piano selezionato con successo"
			});
		}
	}

	// ViewModel aggiornato per la registrazione agenzia
	public class AgencyRegistrationViewModel
	{
		public bool IsUserAuthenticated { get; set; }
		public Abbonamento? PianoSelezionato { get; set; }
		public List<Abbonamento> AvailablePlans { get; set; } = new List<Abbonamento>();

		// Dati utente (per nuovi utenti)
		[Required(ErrorMessage = "Il nome è obbligatorio")]
		[StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
		public string? UserNome { get; set; }

		[Required(ErrorMessage = "Il cognome è obbligatorio")]
		[StringLength(100, ErrorMessage = "Il cognome non può superare i 100 caratteri")]
		public string? UserCognome { get; set; }

		[Required(ErrorMessage = "L'email è obbligatoria")]
		[EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
		[StringLength(200, ErrorMessage = "L'email non può superare i 200 caratteri")]
		public string? UserEmail { get; set; }

		[Required(ErrorMessage = "La password è obbligatoria")]
		[StringLength(100, ErrorMessage = "La password deve essere lunga almeno {2} caratteri.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string? UserPassword { get; set; }

		// Dati agenzia
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

		[Required(ErrorMessage = "Il telefono è obbligatorio")]
		[StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri")]
		[Phone(ErrorMessage = "Inserisci un numero di telefono valido")]
		public string? Telefono { get; set; }

		[Required(ErrorMessage = "L'email è obbligatoria")]
		[StringLength(200, ErrorMessage = "L'email non può superare i 200 caratteri")]
		[EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
		public string? Email { get; set; }

		[StringLength(200, ErrorMessage = "Il sito web non può superare i 200 caratteri")]
		[Url(ErrorMessage = "Inserisci un URL valido")]
		public string? SitoWeb { get; set; }

		[Required(ErrorMessage = "La P.IVA è obbligatoria")]
		[StringLength(20, ErrorMessage = "La P.IVA non può superare i 20 caratteri")]
		[RegularExpression(@"^[0-9]{11}$", ErrorMessage = "La Partita IVA deve essere composta da 11 cifre")]
		public string? PartitaIVA { get; set; }

		[Required(ErrorMessage = "Lo SDI è obbligatorio")]
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