using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class Agenzia
	{
		public Guid AgenziaId { get; set; }

		[Required]
		[StringLength(200)]
		public string RagioneSociale { get; set; } = string.Empty;

		[StringLength(1000)]
		public string? Descrizione { get; set; }

		[Required]
		[StringLength(300)]
		public string Indirizzo { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Città { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Provincia { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Regione { get; set; } = string.Empty;

		[Required]
		[StringLength(10)]
		public string CAP { get; set; } = string.Empty;

		[StringLength(20)]
		public string? Telefono { get; set; }

		[EmailAddress]
		[StringLength(200)]
		public string? Email { get; set; }

		[StringLength(200)]
		public string? SitoWeb { get; set; }

		[StringLength(20)]
		public string? PartitaIVA { get; set; }

		[StringLength(20)]
		public string? SDI { get; set; }

		public bool IsPublic { get; set; } = false; // Profilo pubblico o privato

		[StringLength(100)]
		public string? PinAccesso { get; set; } // PIN per accesso se privata

		[StringLength(500)]
		public string? LogoUrl { get; set; }

		[StringLength(500)]
		public string? ImageUrl { get; set; }

		public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;

		public bool IsActive { get; set; } = true;

		// Foreign Keys
		public string ApplicationUserId { get; set; } = string.Empty; // Owner dell'agenzia
		public Guid? AbbonamentoId { get; set; }

		// Navigation Properties
		public ApplicationUser ApplicationUser { get; set; } = null!;
		public Abbonamento? Abbonamento { get; set; }
		public ICollection<AgenziaUtente> AgenziaUtenti { get; set; } = new List<AgenziaUtente>();
		public ICollection<Immobile> Immobili { get; set; } = new List<Immobile>();
		public ICollection<Lead> Leads { get; set; } = new List<Lead>();
	}
}
