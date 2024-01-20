using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Dtos
{
    public class ArticoliDto
    {
        public string CodArt {get; set;}
        public string Descrizione {get; set;}
        public string Um {get; set;}
        public string CodStat {get; set;}
        public Int16? PzCart {get; set;}
        public double? PesoNetto {get; set;}
        public DateTime? DataCreazione {get; set;}
        public ICollection<BarcodeDto> Ean {get; set;}
        public IvaDto Iva {get; set;}
        public string? Categoria {get; set;}
        public string IdStatoArt {get; set;}
    }

    public class BarcodeDto
    {
        public string? Barcode {get; set;}
        public string? Tipo {get; set;}
        
    }

    public class IvaDto{
        public IvaDto(string? Descrizione, Int16? Aliquota)
        {
            this.Descrizione = Descrizione;
            this.Aliquota = Aliquota;
        }
        public string? Descrizione {get; set;}
        public Int16? Aliquota {get; set;}
    }
}