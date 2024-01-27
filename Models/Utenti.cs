using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Models
{
    public class Utenti
    {
        [Key]
        public string CodFidelity { get; set; }

        [StringLength(50, MinimumLength=5, ErrorMessage="La UserId deve essere compreso fra 5 e 50 caratteri")]
        public string UserId { get; set; }

        [StringLength(50, MinimumLength=10, ErrorMessage="La Password deve essere compresa fra 10 e 50 caratteri")]
        [Required(ErrorMessage="E' necessario inserire la password dell'utente")]
        public string Password { get; set; }
        
        [Required]
        public string Abilitato { get; set; }

        public virtual ICollection<Profili>? Profili { get; set; }

    }
}