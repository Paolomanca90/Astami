using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Asta
	{
		public Guid AstaId { get; set; }
		public Guid ImmobileId { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public decimal PrezzoPartenza { get; set; }

		[Range(0, double.MaxValue)]
		public decimal? PrezzoCorrente { get; set; }

		[Range(0, double.MaxValue)]
		public decimal? PrezzoRiserva { get; set; }

		public DateTime DataInizio { get; set; }
		public DateTime DataFine { get; set; }

		public StatoAsta Stato { get; set; } = StatoAsta.Programmata;

		[StringLength(500)]
		public string? Note { get; set; }

		public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

		public string CreadaDaUserId { get; set; } = string.Empty;

		public bool AllowRiduci { get; set; } = false; // Opzione RIDUCI se solo 1 partecipante

		// Navigation Properties
		public Immobile Immobile { get; set; } = null!;
		public ApplicationUser CreadaDaUser { get; set; } = null!;
		public ICollection<Offerta> Offerte { get; set; } = new List<Offerta>();
		public ICollection<AstaPartecipante> Partecipanti { get; set; } = new List<AstaPartecipante>();
	}
}
