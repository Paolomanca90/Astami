using Astami.Utilities.Constants;
using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class AppuntamentoPartecipante
	{
		public Guid AppuntamentoPartecipanteId { get; set; }
		public Guid AppuntamentoId { get; set; }
		public string ApplicationUserId { get; set; } = string.Empty;

		public string Ruolo { get; set; } = RolesConstants.Agente; // Agente, Cliente

		public StatoPartecipazione Stato { get; set; } = StatoPartecipazione.Invitato;

		public DateTime? DataRisposta { get; set; }

		[StringLength(500)]
		public string? Note { get; set; }

		// Navigation Properties
		public Appuntamento Appuntamento { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
	}
}
