using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Lead
	{
		public Guid LeadId { get; set; }

		[Required]
		[StringLength(100)]
		public string Nome { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Cognome { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(200)]
		public string Email { get; set; } = string.Empty;

		[StringLength(20)]
		public string? Telefono { get; set; }

		[StringLength(1000)]
		public string? Messaggio { get; set; }

		public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

		public StatoLead Stato { get; set; } = StatoLead.Nuovo;

		[StringLength(500)]
		public string? NoteInterne { get; set; }

		public DateTime? DataUltimoContatto { get; set; }

		// Foreign Keys
		public Guid? AgenziaId { get; set; }
		public Guid? ImmobileId { get; set; }
		public string? AssegnatoAUserId { get; set; } // Agente assegnato

		// Navigation Properties
		public Agenzia? Agenzia { get; set; }
		public Immobile? Immobile { get; set; }
		public ApplicationUser? AssegnatoAUser { get; set; }
	}
}
