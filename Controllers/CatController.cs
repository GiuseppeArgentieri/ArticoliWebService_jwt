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
    [Route("api/cat")]
    public class CatController : Controller
    {
        private readonly IArticoliRepository articoliRepository;

        public CatController(IArticoliRepository articoliRepository)
        {
            this.articoliRepository = articoliRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FamAssort>))]
        public async Task<ActionResult<List<FamAssortDto>>> GetCat(){
            ICollection<FamAssort> categories = await articoliRepository.SelCat();
            return Ok(CatDtoMap(categories));
        }

        private List<FamAssortDto> CatDtoMap(ICollection<FamAssort> categories)
        {
            var CatDtolist = new List<FamAssortDto>();

            foreach(var el in categories){
                CatDtolist.Add(
                    new FamAssortDto{
                        Id = el.Id,
                        Descrizione = el.Descrizione
                    }
                );
            }
            return CatDtolist; 
        }
    }
   
}