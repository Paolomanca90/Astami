using Astami.Data;
using Astami.Utilities.Constants;

namespace Astami.Utilities.Subscriptions
{
	public static class SubscriptionsInitializer
	{
		public static async Task InitializeSubscriptions(ApplicationDbContext _context)
		{
			// Basic Plan - €99/mese
			if (!_context.Abbonamento.Any(x => x.AbbonamentoId == SubscriptionsConstants.Basic))
			{
				var abbonamentoBasic = new Models.Abbonamento
				{
					AbbonamentoId = SubscriptionsConstants.Basic,
					Nome = "Basic",
					Descrizione = "Dashboard gestionale privata • Caricamento e gestione immobili • Accesso clienti con richieste digitali • Notifiche automatiche e report base",
					Prezzo = 99.00m,
					Durata = 30 // giorni
				};
				_context.Abbonamento.Add(abbonamentoBasic);
			}

			// PRO Plan - €149/mese  
			if (!_context.Abbonamento.Any(x => x.AbbonamentoId == SubscriptionsConstants.Pro))
			{
				var abbonamentoPro = new Models.Abbonamento
				{
					AbbonamentoId = SubscriptionsConstants.Pro,
					Nome = "PRO",
					Descrizione = "Tutto nel piano Basic • Offerte libere digitalizzate • Personalizzazione tempistiche • Notifiche smart per clienti e agenti • Report avanzati sulle performance",
					Prezzo = 149.00m,
					Durata = 30 // giorni
				};
				_context.Abbonamento.Add(abbonamentoPro);
			}

			// Enterprise Plan - €499/mese
			if (!_context.Abbonamento.Any(x => x.AbbonamentoId == SubscriptionsConstants.Enterprice))
			{
				var abbonamentoEnterprise = new Models.Abbonamento
				{
					AbbonamentoId = SubscriptionsConstants.Enterprice,
					Nome = "Enterprise",
					Descrizione = "Tutto nel piano Pro • Firma digitale e gestione contratti • Pagamenti e transazioni online • Integrazione con CRM e portali immobiliari • Supporto premium dedicato",
					Prezzo = 499.00m,
					Durata = 30 // giorni
				};
				_context.Abbonamento.Add(abbonamentoEnterprise);
			}

			// Salva le modifiche solo se ci sono stati cambiamenti
			if (_context.ChangeTracker.HasChanges())
			{
				await _context.SaveChangesAsync();
			}
		}
	}
}