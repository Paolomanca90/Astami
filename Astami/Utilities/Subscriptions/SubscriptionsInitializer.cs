using Astami.Data;
using Astami.Utilities.Constants;

namespace Astami.Utilities.Subscriptions
{
	public static class SubscriptionsInitializer
	{
		public static async Task InitializeSubscriptions(ApplicationDbContext _context)
		{
			if (!_context.Abbonamento.Any(x => x.AbbonamentoId == SubscriptionsConstants.Standard))
			{
				var abbonamentoStandard = new Models.Abbonamento
				{
					AbbonamentoId = SubscriptionsConstants.Standard,
					Nome = "Abbonamento Standard",
					Descrizione = "Accesso base alle funzionalità.",
					Prezzo = 9.99m,
					Durata = 30
				};
				_context.Abbonamento.Add(abbonamentoStandard);
			}

			if (!_context.Abbonamento.Any(x => x.AbbonamentoId == SubscriptionsConstants.Pro))
			{
				var abbonamentoPremium = new Models.Abbonamento
				{
					AbbonamentoId = SubscriptionsConstants.Pro,
					Nome = "Abbonamento Pro",
					Descrizione = "Accesso completo a tutte le funzionalità.",
					Prezzo = 29.99m,
					Durata = 30
				};
				_context.Abbonamento.Add(abbonamentoPremium);
			}

			if (!_context.Abbonamento.Any(x => x.AbbonamentoId == SubscriptionsConstants.Business))
			{
				var abbonamentoBusiness = new Models.Abbonamento
				{
					AbbonamentoId = SubscriptionsConstants.Business,
					Nome = "Abbonamento Business",
					Descrizione = "Accesso esclusivo con vantaggi premium.",
					Prezzo = 59.99m,
					Durata = 30
				};
				_context.Abbonamento.Add(abbonamentoBusiness);
			}

			await _context.SaveChangesAsync();
		}
	}
}
