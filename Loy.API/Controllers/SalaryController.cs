using Data.TimeTravel.Shared;
using Logic.TimeTravel;
using Loy.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Loy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        // GET: api/<SalaryController>
        [HttpGet]
        public Salary Get(Guid primaryKey)
        {


            //TimeTravelManager<Salary, InternalSalary, Envelop<InternalSalary>> timeManager = new TimeTravelManager<Salary, InternalSalary, Envelop<InternalSalary>>
            //(
            //    READ_DATABASE<InternalSalary, Envelop<InternalSalary>>.InternalSalaries,
            //    READ_DATABASE<InternalSalary, Envelop<InternalSalary>>.InternalSalaryEnvelops
            //);


            //return timeManager.READ(primaryKey);

            return null;
        }



















        // GET api/<SalaryController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SalaryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SalaryController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SalaryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
