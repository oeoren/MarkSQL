using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarkSql.Shared;
using MarkSql.ServerLib;

namespace MarkSql.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IMarkModelBuilder _modelBuilder;

        public ModelController(IMarkModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public async Task<MarkModel> GetModel()
        {
            var model = await _modelBuilder.GetModel();

            return model;

        }

    }
}
