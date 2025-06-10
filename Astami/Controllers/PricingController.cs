using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Astami.Data;
using Astami.Models;
using Microsoft.AspNetCore.Identity;

namespace Astami.Controllers
{
	public class PricingController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public PricingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var abbonamenti = await _context.Abbonamento.ToListAsync();
			return View(abbonamenti);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SelectPlan(Guid abbonamentoId, string returnUrl = null)
		{
			var abbonamento = await _context.Abbonamento.FindAsync(abbonamentoId);
			if (abbonamento == null)
			{
				return NotFound();
			}

			// Salva la selezione temporanea del piano
			var user = await _userManager.GetUserAsync(User);
			if(user != null)
			{
				var pianoEsistente = await _context.PianoSelezionato
					.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

				if (pianoEsistente != null)
				{
					pianoEsistente.AbbonamentoId = abbonamentoId;
					pianoEsistente.Confermato = false;
					_context.Update(pianoEsistente);
				}
			}
			else
			{
				TempData["PianoSelezionatoId"] = abbonamentoId;
			}

			return RedirectToAction("RegisterAgency", "Agency");
		}
	}
}