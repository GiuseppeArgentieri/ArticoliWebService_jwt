using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticoliWebService.Services
{
    public class ArticoliRepository : IArticoliRepository
    {
        private AlphaShopDbContext alphaShopDbContext;

        public ArticoliRepository(AlphaShopDbContext alphaShopDbContext)
        {
            this.alphaShopDbContext = alphaShopDbContext;
        }
        public async Task<bool> ArticoloExists(string Code)
        {
            //var articolo = await SelArticoloByCodice(Code);
            //return articolo != null;
            return await alphaShopDbContext.Articoli.AnyAsync(q => q.CodArt == Code);
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

        public async Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione)
        {
            return await alphaShopDbContext.Articoli
                        .Where(q => q.Descrizione!.Contains(Descrizione))
                        .Include(q => q.Barcode)
                        .Include(q => q.famAssort)
                        .Include(q => q.iva)
                        .OrderBy(q => q.Descrizione)
                        .ToListAsync();
        }

        public async Task<Articoli> SelArticoloByCodice(string Code)
        {
            return await alphaShopDbContext.Articoli
                        .Where(q => q.CodArt!.Equals(Code))
                        .Include(q => q.Barcode)
                        .Include(q => q.famAssort)
                        .Include(q => q.iva)
                        .FirstOrDefaultAsync()!;
        }

        public async Task<Articoli> SelArticoloByEan(string Ean)
        {
            return await alphaShopDbContext.Barcode
                        .Include(q => q.articolo!.Barcode)
                        .Include(q => q.articolo!.famAssort)
                        .Include(q => q.articolo!.iva)
                        .Where(b => b.Barcode!.Equals(Ean))
                        .Select(a => a.articolo)
                        .FirstOrDefaultAsync()!;
        }

        public bool UpdArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }
    }
}