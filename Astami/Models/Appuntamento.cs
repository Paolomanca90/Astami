using Astami.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Appuntamento
	{
		public Guid AppuntamentoId { get; set; }

		[Required]
		[StringLength(200)]
		public string Titolo { get; set; } = string.Empty;

		[StringLength(1000)]
		public string? Descrizione { get; set; }

		public DateTime DataInizio { get; set; }
		public DateTime DataFine { get; set; }

		public TipoAppuntamento Tipo { get; set; } = TipoAppuntamento.Visita;

		public StatoAppuntamento Stato { get; set; } = StatoAppuntamento.Programmato;

		[StringLength(300)]
		public string? Luogo { get; set; }

		[StringLength(500)]
		public string? LinkVideoCall { get; set; }

		[StringLength(500)]
		public string? Note { get; set; }

		public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

		public bool InviatoGoogleCalendar { get; set; } = false;

		[StringLength(200)]
		public string? GoogleCalendarEventId { get; set; }

		// Foreign Keys
		public Guid? ImmobileId { get; set; }
		public Guid? AgenziaId { get; set; }
		public string CreadoDaUserId { get; set; } = string.Empty;
		public string? ClienteUserId { get; set; }

		// Navigation Properties
		public Immobile? Immobile { get; set; }
		public Agenzia? Agenzia { get; set; }
		public ApplicationUser CreadoDaUser { get; set; } = null!;
		public ApplicationUser? ClienteUser { get; set; }
		public ICollection<AppuntamentoPartecipante> Partecipanti { get; set; } = new List<AppuntamentoPartecipante>();
	}
}
