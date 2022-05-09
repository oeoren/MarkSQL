using MarkSql.ClassLibrary.Domain;
using System.Net.Http;
using System.Net.Http.Json;


namespace MarkSql.Client.Services
{
    public interface ILocalApi
    {
        public Task<string?> ExceMqProc(string procName);
        public Task<string?> ExceMqProc(MasReport masReport);
    }

    public class LocalApi : ILocalApi
     {
        HttpClient _httpClient;
        
        public LocalApi(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        }


        public async Task<string?> ExceMqProc(string procName)
        {
            string? md = await _httpClient.GetFromJsonAsync<string>("api/test?lookupCategory=1");
            return md;
        }

        public async Task<string?> ExceMqProc(MasReport masReport)
        {
            string uri = "api/Report/" + masReport.Procname;
            string sep = "?";
            foreach(var p in masReport.Parameters)
            { 
                uri += sep + p.Name + "=" + p.Value; 
                sep = "&";
            }
            string? md = await _httpClient.GetFromJsonAsync<string>(uri);
            return md;
        }
    }
}
