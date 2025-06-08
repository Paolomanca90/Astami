using Astami.Utilities.Enum;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Immobile
	{
		public Guid ImmobileId { get; set; }

		[Required]
		[StringLength(300)]
		public string Titolo { get; set; } = string.Empty;

		[StringLength(2000)]
		public string? Descrizione { get; set; }

		[Required]
		[StringLength(300)]
		public string Indirizzo { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Città { get; set; } = string.Empty;

		[StringLength(100)]
		public string? Provincia { get; set; }

		[Required]
		[StringLength(10)]
		public string CAP { get; set; } = string.Empty;

		[Required]
		public TipoContratto TipoContratto { get; set; } = TipoContratto.Affitto;

		[Required]
		public TipoImmobile TipoImmobile { get; set; } = TipoImmobile.Appartamento;

		[Required]
		[Range(0, double.MaxValue)]
		public decimal PrezzoBase { get; set; }

		[Range(0, 1000)]
		public int? Superficie { get; set; }

		[Range(0, 20)]
		public int? NumeroLocali { get; set; }

		[Range(0, 10)]
		public int? NumeroBagni { get; set; }

		[Range(0, 50)]
		public int? Piano { get; set; }

		public bool HasAscensore { get; set; } = false;
		public bool HasBalcone { get; set; } = false;
		public bool HasTerrazza { get; set; } = false;
		public bool HasGiardino { get; set; } = false;
		public bool HasParcheggio { get; set; } = false;
		public bool HasCantina { get; set; } = false;

		[StringLength(50)]
		public string? ClasseEnergetica { get; set; }

		[Range(0, double.MaxValue)]
		public decimal? SpeseCondominiali { get; set; }

		[StringLength(500)]
		public string? Note { get; set; }

		public DateTime DataCreazione { get; set; } = DateTime.UtcNow;
		public DateTime? DataModifica { get; set; }

		public StatoImmobile Stato { get; set; } = StatoImmobile.Disponibile;

		public bool IsPublished { get; set; } = false;

		// Foreign Keys
		public Guid AgenziaId { get; set; }
		public string? ProprietarioId { get; set; } // ApplicationUser proprietario

		// Navigation Properties
		public Agenzia Agenzia { get; set; } = null!;
		public ApplicationUser? Proprietario { get; set; }
		public ICollection<ImmobileImmagine> Immagini { get; set; } = new List<ImmobileImmagine>();
		public ICollection<ImmobileDocumento> Documenti { get; set; } = new List<ImmobileDocumento>();
		public ICollection<Asta> Aste { get; set; } = new List<Asta>();
		public ICollection<Lead> Leads { get; set; } = new List<Lead>();
		public ICollection<Appuntamento> Appuntamenti { get; set; } = new List<Appuntamento>();
	}
}
