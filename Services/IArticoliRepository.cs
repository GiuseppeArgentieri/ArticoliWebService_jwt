using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public interface IArticoliRepository
    {
        Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione);
        Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione, string IdCat);
        Task<Articoli> SelArticoloByCodice(string Code);
        Task<Articoli> SelArticoloByCodicePerEliminazione(string Code);
        Task<Articoli> SelArticoloByEan(string Ean);
        Task<bool> InsArticoli(Articoli articolo);
        Task<bool> UpdArticoli(Articoli articolo);
        Task<bool> DelArticoli(Articoli articolo);
        Task<bool> ArticoloExists(string Code);
    }
}