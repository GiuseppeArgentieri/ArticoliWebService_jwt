using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Models
{
    public class Profili
    {
        [Key]
        public int Id { get; set; }
        public string CodFidelity { get; set; }
        public string Tipo { get; set; }

        public virtual Utenti? Utente { get; set; } 
    }
}