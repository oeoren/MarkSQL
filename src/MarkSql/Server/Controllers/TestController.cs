using Dapper;
using MarkSql.Server.Context;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarkSql.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DapperContext _context;

        public TestController(DapperContext context)
        {
            _context = context;
        }

        // GET: api/<TestController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<TestController>/5
        [HttpGet("")]
        public async Task<string> GetAsync(int lookupCategory)
        {
            var procedureName = "mq_Products";
            var parameters = new DynamicParameters();
            parameters.Add("lookupCategory", lookupCategory, DbType.Int32, ParameterDirection.Input);
            using (var connection = _context.CreateConnection())
            {

                var ret = await connection.QueryFirstOrDefaultAsync<String>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return ret;
            }
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
