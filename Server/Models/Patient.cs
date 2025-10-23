using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Patient
    {
        /// <summary>
        /// De unieke identificatie van het album.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// De titel van het album.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Naam { get; set; } = string.Empty;

        /// <summary>
        /// De titel van het album.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Adres { get; set; } = string.Empty;

        /// <summary>
        /// Het publicatiejaar van het album.
        /// </summary>
        [Range(1900, 2025)]
        public int Geboortejaar { get; set; }
    }
}
