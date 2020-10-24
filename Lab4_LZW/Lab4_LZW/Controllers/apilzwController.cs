using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab4_LZW.Controllers
{
    [Route("api/[compress]")]
    [ApiController]
    public class apilzwController : ControllerBase
    {
        //[HttpPost("{name}")]
        //public IActionResult Post1()
        //{
        //    return;
        //}
        [HttpPost]
        public IActionResult Post2()
        {
            ClassLZW lista = new ClassLZW();
            return Ok(lista);
        }
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    return;
        //}
    }
}
