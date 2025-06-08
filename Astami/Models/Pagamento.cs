using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Pagamento
	{
		public Guid PagamentoId { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public decimal Importo { get; set; }

		public DateTime DataPagamento { get; set; } = DateTime.UtcNow;

		public TipoPagamento Tipo { get; set; } = TipoPagamento.Abbonamento;

		public StatoPagamento Stato { get; set; } = StatoPagamento.InAttesa;

		[StringLength(200)]
		public string? TransactionId { get; set; } // ID da Stripe

		[StringLength(100)]
		public string? MetodoPagamento { get; set; } // carta, bonifico, paypal, etc.

		[StringLength(1000)]
		public string? Note { get; set; }

		public DateTime? DataScadenza { get; set; }

		// Foreign Keys
		public string ApplicationUserId { get; set; } = string.Empty;
		public Guid? AbbonamentoId { get; set; }
		public Guid? ImmobileId { get; set; } // Per pagamenti relativi a immobili specifici

		// Navigation Properties
		public ApplicationUser ApplicationUser { get; set; } = null!;
		public Abbonamento? Abbonamento { get; set; }
		public Immobile? Immobile { get; set; }
	}
}
