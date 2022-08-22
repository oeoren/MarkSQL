using MarkSql.ClientLib;
using MarkSql.Shared;
using System.Net.Http;
using System.Net.Http.Json;


namespace MarkSql.Client.Services
{
    public interface ILocalApi
    {
        public Task<string?> ExceMqProc(string procName);
        public Task<string?> ExceMqProc(MasForm masReport);

        public Task<MasForm?> GetForm(string procName);
        public Task<MarkModel?> GetModel();
    }

    public class LocalApi : ILocalApi
     {
        HttpClient _httpClient;
        MarkModel? _masModel;
        IFormMaker _formMaker;
        public LocalApi(HttpClient httpClient, IFormMaker formMaker)
        {
            _httpClient = httpClient;
            _masModel = null;
            _formMaker = formMaker;
        }


        public async Task<string?> ExceMqProc(string procName)
        {
            string? md = await _httpClient.GetFromJsonAsync<string>("api/test?lookupCategory=1");
            return md;
        }

        public async Task<string?> ExceMqProc(MasForm masForm)
        {
            string uri = "api/Report/" + masForm.Procname;
            string sep = "?";
            foreach(var p in masForm.Fields)
            { 
                uri += sep + p.Name + "=" + p.Value; 
                sep = "&";
            }
            string? md = await _httpClient.GetFromJsonAsync<string>(uri);
            return md;
        }


        public async Task<MasForm?> GetForm(string procName)
        {
            var markModel = await GetModel();
            if (markModel == null)
                return null;
            return _formMaker.BuildForm(markModel,procName);
        }

        public async Task<MarkModel?> GetModel()
        {
            if (_masModel == null)
            { 
                string uri = "api/Model";
                _masModel = await _httpClient.GetFromJsonAsync<MarkModel>(uri);
            }
            return _masModel;
        }
    }
}
