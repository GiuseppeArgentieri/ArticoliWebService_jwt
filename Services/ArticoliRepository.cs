using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public class ArticoliRepository : IArticoliRepository
    {
        private AlphaShopDbContext alphaShopDbContext;

        public ArticoliRepository(AlphaShopDbContext alphaShopDbContext)
        {
            this.alphaShopDbContext = alphaShopDbContext;
        }
        public bool ArticoloExists(string Code)
        {
            throw new NotImplementedException();
        }

        public bool DelArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }

        public bool InsArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }

        public bool Salva()
        {
            throw new NotImplementedException();
        }

        public ICollection<Articoli> SelArticoliByDescrizione(string Descrizione)
        {
            return alphaShopDbContext.Articoli
                        .Where(q => q.Descrizione.Contains(Descrizione))
                        .OrderBy(q => q.Descrizione)
                        .ToList();
        }

        public Articoli SelArticoloByCodice(string Code)
        {
            return alphaShopDbContext.Articoli
                        .Where(q => q.CodArt.Equals(Code))
                        .FirstOrDefault();
        }

        public Articoli SelArticoloByEan(string Ean)
        {
            return alphaShopDbContext.Barcode
                        .Where(b => b.Barcode.Equals(Ean))
                        .Select(a => a.articolo)
                        .FirstOrDefault();
        }

        public bool UpdArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }
    }
}