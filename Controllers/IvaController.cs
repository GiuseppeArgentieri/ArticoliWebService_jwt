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
    [Route("api/iva")]
    public class IvaController: Controller
    {
        private readonly IArticoliRepository articoliRepository;

        public IvaController(IArticoliRepository articoliRepository)
        {
            this.articoliRepository = articoliRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IvaDto>))]
        public async Task<ActionResult<List<IvaDto>>> GetIva(){
            ICollection<Iva> iva = await articoliRepository.SelIva();
            return Ok(IvaDtoMap(iva));

        }


        private List<IvaDto> IvaDtoMap(ICollection<Iva> listaIva){
            //Console.WriteLine(articolo.CodArt);
            var ivaDtolist = new List<IvaDto>();

            foreach(var elIva in listaIva){
                ivaDtolist.Add(
                    new IvaDto(elIva.IdIva, elIva.Descrizione, elIva.Aliquota)
                );
            }
            return ivaDtolist; 
        }
    }
}