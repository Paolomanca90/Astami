using System.ComponentModel.DataAnnotations;

namespace Astami.Models
{
	public class ImmobileDocumento
	{
		public Guid ImmobileDocumentoId { get; set; }
		public Guid ImmobileId { get; set; }

		[Required]
		[StringLength(200)]
		public string NomeFile { get; set; } = string.Empty;

		[Required]
		[StringLength(500)]
		public string Url { get; set; } = string.Empty;

		[StringLength(100)]
		public string? TipoDocumento { get; set; } // APE, Planimetria, Contratto, etc.

		[Range(0, long.MaxValue)]
		public long DimensioneFile { get; set; }

		[StringLength(50)]
		public string? MimeType { get; set; }

		public DateTime DataCaricamento { get; set; } = DateTime.UtcNow;

		public string CaricatoDaUserId { get; set; } = string.Empty;

		// Navigation Properties
		public Immobile Immobile { get; set; } = null!;
		public ApplicationUser CaricatoDaUser { get; set; } = null!;
	}
}
