using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Dtos;
using ArticoliWebService.Models;
using ArticoliWebService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/articoli")]
    public class ArticoliController : Controller
    {
        private readonly IArticoliRepository articoliRepository;
        public ArticoliController(IArticoliRepository articoliRepository)
        {
            this.articoliRepository = articoliRepository;
        }

        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ArticoliDto>))]
        public async Task<IActionResult> GetArticoliByDesc(string filter){
                var articoliDto = new List<ArticoliDto>();
                var articoli = await articoliRepository.SelArticoliByDescrizione(filter);

                if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

                if(articoli.Count == 0){
                    return NotFound(
                        new ErrMsg(string.Format("Non è stato trovato alcun articolo con il filtro '{0}'", filter), this.HttpContext.Response.StatusCode));
                }

                foreach(var articolo in articoli){
                    articoliDto.Add(
                        GetArticoliDto(articolo)
                    );
                }
                return Ok(articoliDto);
        }


        [HttpGet("cerca/codice/{codArt}")]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> getArticoloByCode(string codArt){
            if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

            if(!await articoliRepository.ArticoloExists(codArt)){
                //return NotFound(string.Format("Non è stato trovato alcun articolo con il codice '{0}'", codArt));
                return NotFound(
                        new ErrMsg(string.Format("Non è stato trovato alcun articolo con il codice '{0}'", codArt), this.HttpContext.Response.StatusCode));
            }

            var articolo = await articoliRepository.SelArticoloByCodice(codArt);
            var barcodeDto = new List<BarcodeDto>();
            var articolodto = GetArticoliDto(articolo);
                        
            return Ok(articolodto);
        }

        [HttpGet("cerca/ean/{barcode}")]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> getArticoloByEan(string barcode){
            if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }
            
            var articolo = await articoliRepository.SelArticoloByEan(barcode);

            if(articolo == null){
                //return NotFound(string.Format("Non è stato possibile trvare alcun articolo con l'ean '{0}'", barcode));
                return NotFound(
                        new ErrMsg(string.Format("Non è stato possibile trvare alcun articolo con l'ean '{0}'", barcode), this.HttpContext.Response.StatusCode));
            }

            var articolodto = GetArticoliDto(articolo);

            return Ok(articolodto);

            }

        private ArticoliDto GetArticoliDto(Articoli articolo){
            //Console.WriteLine(articolo.CodArt);
            var barcodeDto = new List<BarcodeDto>();

            foreach(var ean in articolo.Barcode){
                barcodeDto.Add(
                    new BarcodeDto{
                        Barcode = ean.Barcode,
                        Tipo = ean.IdTipoArt
                    }
                );
            }

            var articolodto = new ArticoliDto{
                            CodArt = articolo.CodArt!,
                            Descrizione = articolo.Descrizione!,
                            Um = (articolo.Um != null) ? articolo.Um.Trim() : "",
                            CodStat = (articolo.CodStat != null) ? articolo.CodStat.Trim() : "" ,
                            PzCart = articolo.PzCart,
                            PesoNetto = articolo.PesoNetto,
                            DataCreazione = articolo.DataCreazione,
                            //IdStatoArt = articolo.IdStatoArt!,
                            IdStatoArt = (articolo.IdStatoArt != null) ? articolo.IdStatoArt.Trim() : "" ,
                            Ean = barcodeDto,
                            Iva = new IvaDto(articolo.iva!.Descrizione!, articolo.iva.Aliquota),
                            Categoria = articolo.famAssort!.Descrizione!
                        };

            return articolodto;
        }
        }
}
