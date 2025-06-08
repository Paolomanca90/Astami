using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Offerta
	{
		public Guid OffertaId { get; set; }
		public Guid AstaId { get; set; }
		public string ApplicationUserId { get; set; } = string.Empty;

		[Required]
		[Range(0, double.MaxValue)]
		public decimal Importo { get; set; }

		public DateTime DataOfferta { get; set; } = DateTime.UtcNow;

		public TipoOfferta Tipo { get; set; } = TipoOfferta.Rialzo;

		[StringLength(200)]
		public string? Note { get; set; }

		public bool IsWinning { get; set; } = false; // Offerta vincente

		// Navigation Properties
		public Asta Asta { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
	}
}
