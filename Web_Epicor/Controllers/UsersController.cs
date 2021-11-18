using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Epicor.Data;
using Web_Epicor.Models;

namespace Web_Epicor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DbContextSystem context;

        public UsersController(DbContextSystem context)
        {
            this.context = context;
        }

        //POST api/users/
        [HttpPost("[action]")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await context.Users
                .Where(x => x.status_user == true)
                .Where(x => x.pass == model.pass)
                .FirstOrDefaultAsync(x => x.email == model.email);

            if(user == null)
            {
                return NotFound();

            }
            return Ok();

        }


    }
}
