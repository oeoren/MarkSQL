using System.Net.Http;
using System.Net.Http.Json;


namespace MarkSql.Client.Services
{
    public interface ILocalApi
    {
        public Task<string?> ExceMqProc(string procName);
    }
    
    public class LocalApi : ILocalApi
     {
        HttpClient _httpClient;
        
        public LocalApi(HttpClient httpClient) 
        { 
            _httpClient = httpClient; 
        }


        async Task<string?> ILocalApi.ExceMqProc(string procName)
        {
            string? md = await _httpClient.GetFromJsonAsync<string>("api/test");
            return md;
        }


    }
}
