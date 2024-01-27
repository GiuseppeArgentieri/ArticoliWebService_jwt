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

        public async Task<bool> InsArticoli(Articoli articolo)
        {
            await alphaShopDbContext.AddAsync(articolo);
            return await Salva();
        }

        public async Task<bool> UpdArticoli(Articoli articolo)
        {
            alphaShopDbContext.Update(articolo);
            return await Salva();
        }

        public async Task<bool> DelArticoli(Articoli articolo)
        {
            alphaShopDbContext.Remove(articolo);
            return await Salva();
        }

        private async Task<bool> Salva()
        {
            var saved = await alphaShopDbContext.SaveChangesAsync();
            return saved >= 0 ? true : false;
        }

        public async Task<Articoli> SelArticoloByCodicePerEliminazione(string Code)
        {
            return await alphaShopDbContext.Articoli
                    .Where(q => q.CodArt.Equals(Code))
                    .FirstOrDefaultAsync();
        }

        public async Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione, string? IdCat)
        {
            bool isNumeric = int.TryParse(IdCat, out int n);

            if(string.IsNullOrWhiteSpace(IdCat) || !isNumeric)
            {
                return await SelArticoliByDescrizione(Descrizione);
            }

            return await alphaShopDbContext.Articoli
                        .Where(q => q.Descrizione!.Contains(Descrizione))
                        .Where(q => q.IdFamAss == int.Parse(IdCat))
                        .Include(q => q.Barcode)
                        .Include(q => q.famAssort)
                        .Include(q => q.iva)
                        .OrderBy(q => q.Descrizione)
                        .ToListAsync();
        }

        public async Task<ICollection<Iva>> SelIva()
        {
            return await alphaShopDbContext.Iva
                    .OrderBy(q => q.Aliquota)
                    .ToListAsync();
        }

        public async Task<ICollection<FamAssort>> SelCat()
        {
            return await alphaShopDbContext.FamAssort
                    .OrderBy(q => q.Id)
                    .ToListAsync();
        }
    }
}