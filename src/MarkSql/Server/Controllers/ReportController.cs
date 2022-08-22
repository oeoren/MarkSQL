using Dapper;

using MarkSql.ServerLib;
using MarkSql.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Data;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarkSql.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IMarkModelBuilder _modelBuilder;

        public ReportController(DapperContext context, IMarkModelBuilder modelBuilder)
        {
            _context = context;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("{procedureName}")]
        public async Task<string> GetAsync(string procedureName)
        {

            var model = await _modelBuilder.GetModel();

            var queryParams = QueryHelpers.ParseQuery(Request.QueryString.Value);
            var items = queryParams.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var parameters = new DynamicParameters();

            //   parameters.Add("lookupCategory", "2", DbType.String, ParameterDirection.Input);

            var proc = model.procs.FirstOrDefault(p => p.name == procedureName);

            foreach (var pi in proc.parameters)
                foreach (var parameter in queryParams)
                    if (parameter.Key == pi.name)
                        switch (pi.sqlType)
                        {
                            case "datetime":
                                var val = DateTime.Parse(parameter.Value.ToString());
                                parameters.Add(parameter.Key, val, DbType.DateTime);
                                break;

                            default:
                                parameters.Add(parameter.Key, parameter.Value.ToString(), DbType.String, ParameterDirection.Input);
                                break;


                        };

            using (var connection = _context.CreateConnection())
            {
                string jStr = ""; ;
                try
                {
                    var ret = await connection.QueryFirstOrDefaultAsync<ProcReturn>
                        (procedureName, parameters, commandType: CommandType.StoredProcedure)
                        .ConfigureAwait(false);
                    jStr = JsonSerializer.Serialize(ret.MarkDown);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing " + procedureName + ":" + ex.ToString());
                    throw;
                }
                return jStr;
            }
        }
    }
}