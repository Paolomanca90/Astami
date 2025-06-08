using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class ImmobileImmagine
	{
		public Guid ImmobileImmagineId { get; set; }
		public Guid ImmobileId { get; set; }

		[Required]
		[StringLength(500)]
		public string Url { get; set; } = string.Empty;

		[StringLength(200)]
		public string? Descrizione { get; set; }

		public int Ordine { get; set; } = 0;
		public bool IsPrimary { get; set; } = false;
		public DateTime DataCaricamento { get; set; } = DateTime.UtcNow;

		// Navigation Properties
		public Immobile Immobile { get; set; } = null!;
	}
}
