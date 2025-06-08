using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class AstaPartecipante
	{
		public Guid AstaPartecipanteId { get; set; }
		public Guid AstaId { get; set; }
		public string ApplicationUserId { get; set; } = string.Empty;

		public DateTime DataIscrizione { get; set; } = DateTime.UtcNow;
		public bool IsApprovato { get; set; } = false; // L'agenzia può filtrare i partecipanti
		public bool IsActive { get; set; } = true;

		[StringLength(500)]
		public string? NoteAgenzia { get; set; }

		// Navigation Properties
		public Asta Asta { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
	}
}
