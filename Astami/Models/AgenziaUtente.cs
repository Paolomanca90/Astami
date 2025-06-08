using Astami.Utilities.Constants;

namespace Astami.Models
{
	public class AgenziaUtente
	{
		public Guid AgenziaUtenteId { get; set; }
		public Guid AgenziaId { get; set; }
		public string ApplicationUserId { get; set; } = string.Empty;
		public string Ruolo { get; set; } = RolesConstants.Agente; // Manager, Agente, Collaboratore
		public DateTime DataAssegnazione { get; set; } = DateTime.UtcNow;
		public bool IsActive { get; set; } = true;

		// Navigation Properties
		public Agenzia Agenzia { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
	}
}
