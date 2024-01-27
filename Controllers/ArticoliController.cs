using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Dtos;
using ArticoliWebService.Models;
using ArticoliWebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/articoli")]
    [Authorize(Roles = "Admin, USER")]
    public class ArticoliController : Controller
    {
        private readonly IArticoliRepository articoliRepository;
        public ArticoliController(IArticoliRepository articoliRepository)
        {
            this.articoliRepository = articoliRepository;
        }

        [HttpGet("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult TestConnex()
        {
            return Ok(new infoMsg(DateTime.Today, "Test Connessione Ok"));
        }

        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ArticoliDto>))]
        //sostituire IActionResult con ActionResult<IEnumerable<ArticoliDto>> a livello di prestazioni è migliore
        public async Task<ActionResult<IEnumerable<ArticoliDto>>> GetArticoliByDesc(string filter, [FromQuery(Name = "cat")] string? idCat){
                var articoliDto = new List<ArticoliDto>();
                var articoli = await articoliRepository.SelArticoliByDescrizione(filter, idCat);

                if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }

                if(articoli.Count == 0){
                    return NotFound(
                        new ErrMsg(string.Format("Non è stato trovato alcun articolo con il filtro '{0}'", filter), 404));
                }

                foreach(var articolo in articoli){
                    //Console.WriteLine(articolo.CodArt);
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
        [AllowAnonymous]
        public async Task<ActionResult<ArticoliDto>> getArticoloByCode(string codArt){
            if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }
            
            bool retVal = await articoliRepository.ArticoloExists(codArt);
            int statusCode = (this.HttpContext != null)? this.HttpContext.Response.StatusCode: 404;

            if(!retVal){
                //return NotFound(string.Format("Non è stato trovato alcun articolo con il codice '{0}'", codArt));
                return NotFound(
                        new ErrMsg(string.Format("Non è stato trovato alcun articolo con il codice '{0}'", codArt), statusCode));
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

        [HttpPost("inserisci")]
        [ProducesResponseType(201, Type = typeof(Articoli))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SaveArticoli([FromBody] Articoli articolo){
            if(articolo == null){
                return BadRequest(new ErrMsg("Dati nuovo articolo assenti", this.HttpContext.Response.StatusCode));
            }

            if(articolo.IdIva == -1){
                return BadRequest(new ErrMsg("Aliquota iva non selezionata", this.HttpContext.Response.StatusCode));
            }

            var isPresent = await articoliRepository.ArticoloExists(articolo.CodArt);

            //controlliamo che l'articolo sia presente
            if(isPresent)
            {
                //ModelState.AddModelError("", $"Articolo {articolo.CodArt} è presente in anagrafica! impossibile utilizzare il metodo POST");
                return StatusCode(422, new ErrMsg($"Articolo {articolo.CodArt} è presente in anagrafica! impossibile utilizzare il metodo POST", this.HttpContext.Response.StatusCode));
            }
            //verifichiamo che i dati siano corretti
            if(!ModelState.IsValid){
                string errVal = "";
                foreach(var modelState in ModelState.Values){
                    foreach(var modelError in modelState.Errors){
                        errVal += modelError.ErrorMessage + " - ";
                    }
                }
                return BadRequest(new ErrMsg(errVal, 400));
            }

            articolo.DataCreazione = DateTime.Today;
            var retVal = await articoliRepository.InsArticoli(articolo);

            if(!retVal){
                //ModelState.AddModelError("", $"Ci sono stati problemi nell'inserimento dell'articolo {articolo.CodArt}");
                return StatusCode(500, new ErrMsg($"Ci sono stati problemi nell'inserimento dell'articolo {articolo.CodArt}", 500));
            }

            return Ok(new infoMsg(DateTime.Today, "Inserimento articolo eseguito con successo"));
        }

        [HttpPut("Modifica")]
        [ProducesResponseType(201, Type = typeof(Articoli))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateArticoli([FromBody] Articoli articolo)
        {
            if(articolo == null){
                return BadRequest(new ErrMsg("Dati articolo assenti", this.HttpContext.Response.StatusCode));
            }

            if(articolo.IdIva == -1){
                return BadRequest(new ErrMsg("Aliquota iva non selezionata", this.HttpContext.Response.StatusCode));
            }
            
            var isPresent = await articoliRepository.ArticoloExists(articolo.CodArt);

            //controlliamo che l'aticolo sia presente
            if(!isPresent){
                //ModelState.AddModelError("", $"Articolo {articolo.CodArt} non è presente in anagrafica! impossibile utilizzare il metodo PUT");
                return StatusCode(422, new ErrMsg($"Articolo {articolo.CodArt} non è presente in anagrafica! impossibile utilizzare il metodo PUT", this.HttpContext.Response.StatusCode));
            }
            //verifichiamo che i dati siano corretti
            if(!ModelState.IsValid){
                string errVal = "";
                int c = 0;
                foreach(var modelState in ModelState.Values){
                    foreach(var modelError in modelState.Errors){
                        //errVal += modelError.ErrorMessage + " - ";
                        var erroreModello = modelError.ErrorMessage;
                        if(erroreModello != null){
                            c+=1;
                        }
                        if(c == 1 && erroreModello != null){
                            errVal += erroreModello;
                        }
                        if(c > 1 && erroreModello!= null){
                            errVal += " - " + erroreModello;
                        }

                    }
                }
                return BadRequest(new ErrMsg(errVal, 400));
            }

            articolo.DataCreazione = DateTime.Today;
            var retVal = await articoliRepository.UpdArticoli(articolo);

            if(!retVal){
                //ModelState.AddModelError("", $"Ci sono stati problemi nella modifica dell'articolo {articolo.CodArt}");
                return StatusCode(500, new ErrMsg($"Ci sono stati problemi nella modifica dell'articolo {articolo.CodArt}", 500));
            }

            return Ok(new infoMsg(DateTime.Today, "Modifica articolo eseguita con successo!"));
        } 

        [HttpDelete("elimina/{codart}")]
        [ProducesResponseType(201, Type = typeof(infoMsg))]
        [ProducesResponseType(400, Type = typeof(ErrMsg))]
        [ProducesResponseType(422, Type = typeof(ErrMsg))]
        [ProducesResponseType(500, Type = typeof(ErrMsg))]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteArticoli(string codart){
            if(codart == ""){
                return BadRequest(new ErrMsg("è necessario inserire il codice dell'articolo", this.HttpContext.Response.StatusCode));
            }
            //controlliamo se l'articolo è presente
            Articoli articolo = await articoliRepository.SelArticoloByCodicePerEliminazione(codart);

            if(articolo == null){
                return StatusCode(422, new ErrMsg($"Articolo {codart} non è presente in anagrafica", this.HttpContext.Response.StatusCode));
            }
            
            var retVal = await articoliRepository.DelArticoli(articolo);
            if(!retVal){
                ModelState.AddModelError("", $"Ci sono stati problemi nella eliminazione dell'articolo {codart}");
                return StatusCode(500, ModelState);
            }

            return Ok(new infoMsg(DateTime.Today, $"eliminazione dell'articolo {codart} eseguita con successo"));
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

            string cat;
            if(articolo.famAssort == null){
                cat  = "";
            }
            else{
                cat = articolo.famAssort.Descrizione;
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
                            //Iva = new IvaDto(articolo.IdIva, articolo.iva!.Descrizione!, articolo.iva.Aliquota),
                            Categoria = cat,
                            IdIva = articolo.IdIva,
                            IdFamAss = articolo.IdFamAss
                        };

            return articolodto;
        }
        }
}
