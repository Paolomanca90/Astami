using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Notifica
	{
		public Guid NotificaId { get; set; }

		[Required]
		[StringLength(200)]
		public string Titolo { get; set; } = string.Empty;

		[Required]
		[StringLength(1000)]
		public string Messaggio { get; set; } = string.Empty;

		public TipoNotifica Tipo { get; set; } = TipoNotifica.Info;

		public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

		public bool IsLetta { get; set; } = false;

		public DateTime? DataLettura { get; set; }

		[StringLength(500)]
		public string? Url { get; set; } // Link per azione

		[StringLength(1000)]
		public string? DatiAggiuntivi { get; set; } // JSON per dati extra

		// Foreign Keys
		public string ApplicationUserId { get; set; } = string.Empty;
		public string? MittenteDaUserId { get; set; }

		// Navigation Properties
		public ApplicationUser ApplicationUser { get; set; } = null!;
		public ApplicationUser? MittenteDaUser { get; set; }
	}
}
