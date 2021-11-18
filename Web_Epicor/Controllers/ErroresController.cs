using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Epicor.Data;
using Web_Epicor.Entities;

namespace Web_Epicor.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ErroresController : ControllerBase
    {
       
        private readonly DbContextSystem context;

        public ErroresController(DbContextSystem context)
        {
            this.context = context;
        }

        //GET api/errores/getError
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Error>>> getError()
        {
            return await context.Errors.OrderByDescending(x => x.id).Take(50).ToListAsync();
        }


    }
}
