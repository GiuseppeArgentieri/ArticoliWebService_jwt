using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private IArticoliRepository articoliRepository;
        public ArticoliController(IArticoliRepository articoliRepository)
        {
            this.articoliRepository = articoliRepository;
        }

        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Articoli>))]
        public IActionResult GetArticoliByDesc(string filter){
                var articoli = articoliRepository.SelArticoliByDescrizione(filter);
                return Ok(articoli);
        }
    }
}