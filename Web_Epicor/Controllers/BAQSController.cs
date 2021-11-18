using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web_Epicor.Data;
using Web_Epicor.Data.AppJson;
using Web_Epicor.Data.BAQS;
using Web_Epicor.Data.Procedures;
using Web_Epicor.Entities;
using Web_Epicor.Models;
using Web_Epicor.Results;
using Web_Epicor.TaskScheduler_BAQS;

namespace Web_Epicor.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]

    public class BAQSController : ControllerBase
    {
        private readonly DbContextSystem context;

        public BAQSController(DbContextSystem context)
        {
            this.context = context;
        }
        
       //GET api/BAQS/getBAQS
       [HttpGet("[action]")]
       public async Task<ActionResult<IEnumerable<BAQ>>> getBAQS()
        {
            return await context.BAQs.ToListAsync();
        }


        //Consumo de BAQS


        //GET api/BAQS/Job
        [HttpGet("[action]/{param?}")]
        public async Task<ActionResult> Job(string param)
        {
            var job = await context.BAQs
                 .Where(x => x.status_baq == true)
                 .FirstOrDefaultAsync(x => x.id == 1);
            if (job != null)
            {
                if(param != null)
                {
                    Jobs.GetJobsParameters(param);
                }
                else
                {
                    //Jobs.GetJobs();
                    Jobs.GetJobsBulk();
                }                                
            }
            else
            {
                Debug.WriteLine("Desactivado --- JOB");
            }

            return Ok();
        }

        //GET api/BAQS/Labor_Time/option
        // 1 = Mes
        // 2 = Semanal
        // 3 = Diaria
        // 4 = Temporizador (X hora)

        [HttpGet("[action]/{option?}")]
        public async Task<ActionResult> Labor_Time(int option)
        
        {
            var laborTime = await context.BAQs
                .Where(x => x.status_baq == true)
                .FirstOrDefaultAsync(x => x.id == 2);

            if (laborTime != null)
            {

                if(option <= 0)
                {
                    LaborTime.GetLaborTime();
                    
                }
                else
                {
                    LaborTime.GetLaborTime(option);
                }
                
            }
            else
            {
                Debug.WriteLine("Desactivado -- JOB LABOR");
            }

            return Ok();
        }

        //GET api/BAQS/Job_Materials
        [HttpGet("[action]")]
        public async Task<ActionResult> Job_Materials()
        {
            var jobMaterials = await context.BAQs
                .Where(x => x.status_baq == true)
                .FirstOrDefaultAsync(x => x.id == 3);

            if (jobMaterials != null)
            {
                JobMaterials.GetJobMAterials();
                Debug.WriteLine("Se ejecutó --- JOB MATERIALS");
            }
            else
            {
                Debug.WriteLine("Desactivado --- JOB MATERIALS");
            }
            return Ok();
        }

        //GET api/BAQS/Part_Cost
        [HttpGet("[action]")]
        public async Task<ActionResult> Part_Cost()
         {
            var partCost = await context.BAQs
                .Where(x => x.status_baq == true)
                .FirstOrDefaultAsync(x => x.id == 4);
            if (partCost != null)
            {
                PartCost.GetPart_cost();
                Debug.WriteLine("Se ejecutó  ---PART COST");
            }
            else
            {
                Debug.WriteLine("Desactivado -- PART COST");
            }

            return Ok();
        }


        //PUT api/BAQS/activate/1
        [HttpPut("[action]/{id}")]
        public async Task<ActionResult> activate([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var baq = await context.BAQs.FirstOrDefaultAsync(x => x.id == id);

            if (baq == null)
            {
                return NotFound();
            }

            baq.status_baq = true;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return Ok();

        }

        //PUT api/BAQS/desactivate/1
        [HttpPut("[action]/{id}")]
        public async Task<ActionResult> desactivate([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }


            var baq = await context.BAQs.FirstOrDefaultAsync(x => x.id == id);

            if (baq == null)
            {
                NotFound();
            }

            baq.status_baq = false;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return Ok();

        }


        //PUT api/BAQS/dailyTask
        [HttpPut("[action]")]
        public async Task<ActionResult>dailyTask([FromBody] DailyViewModel model)
        {
            if(model.id <= 0)
            {
                return BadRequest();
            }

            var baq = await context.BAQs.FirstOrDefaultAsync(x => x.id == model.id);

            if(baq == null)
            {
                return NotFound();
             }
            baq.task_name = model.task_name;
            baq.aTrigger = model.aTrigger;
            baq.description = model.description;
            baq.program_script = model.program_script;
            baq.arguments = model.arguments;
            baq.username = model.username;

            //Modifico/creo tarea 
            bool verify = Scheduler.CreateOrUpdateTaskDaily(model.task_name, model.program_script, model.description, model.username, model.password, model.time, model.arguments);
           
                try
                {

                    if(verify == true)
                    {
                        await context.SaveChangesAsync();
                }
                    else
                    {
                        return BadRequest();
                    }
                  
                }
                catch (Exception ex)
                {
                    ErrorLog.SaveFile("Actualizar tarea", ex);
                    ErrorLog.SendMail("Actualizar tarea", ex);

                    return BadRequest();
                }
            
            
            return Ok();

        }


        //PUT api/BAQS/timerTask
        [HttpPut("[action]")]
        public async Task<ActionResult>timerTask([FromBody] TimerViewModel model)
        {
            if (model.id <= 0)
            {
                return BadRequest();
            }

            var baq = await context.BAQs.FirstOrDefaultAsync(x => x.id == model.id);

            if (baq == null)
            {
                return NotFound();
            }
            baq.task_name = model.task_name;
            baq.aTrigger = model.aTrigger;
            baq.description = model.description;
            baq.program_script = model.program_script;
            baq.arguments = model.arguments;
            baq.username = model.username;

            //Modifico/creo tarea 
            bool verify = Scheduler.CreateOrUpdateTaskTimer(model.task_name, model.program_script, model.description, model.username, model.password, model.dateTime, model.delay, model.time_unit, model.arguments);

            try
            {

                if (verify == true)
                {
                    await context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (SqlException ex)
            {
                ErrorLog.SaveFile("Actualizar tarea", ex);
                ErrorLog.SendMail("Actualizar tarea", ex);

                return BadRequest();
            }


            return Ok();

        }
        //GET api/baqs/prueba
        [HttpGet("[action]")]
        public async Task<ActionResult> prueba()
        {
            int a = 0, b = 0;


            DateTime start = new DateTime(2020, 06, 03);
            DateTime final = new DateTime(2020, 06, 04);
            DeleteByDates.DeleteByDatesLaborTime(start, final);



            //string conexion = LoadJsonData.ConnetionString();
            //string route = LoadJsonData.Route();
            //string joburl = LoadJsonData.JobLabor_url();
            //string jablab = LoadJsonData.JobLabor_url();
            //string jobmat = LoadJsonData.JobMaterials_url();
            //string partcost = LoadJsonData.PartCost_url();
            //DataTable table = new DataTable();
            //table = LoadUsers.Users();
            //UpdateDate.updateJobs(1);
            //string email = LoadCredentials.Email();

            //string pass = LoadCredentials.Password();

            //try
            //{
            //    string email = LoadEmails.Emails();
            //    int div = a / b;



            //}
            //catch (Exception ex)
            //{
            //    ErrorLog.SaveFile("Cargar correos", ex);
            //    ErrorLog.SendMailList("Cargar correos", ex);
            //    //ErrorLog.SendMail("Cargar correos", ex);
            //}



            return Ok();
        }

    }
}
