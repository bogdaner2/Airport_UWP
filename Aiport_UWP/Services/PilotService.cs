using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiport_UWP.DTO;
using Newtonsoft.Json;

namespace Aiport_UWP.Services
{
    public class PilotService
    {
        public async Task<List<PilotDTO>> getAllPilots()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync("http://localhost:62444/api/pilot"))
            using (HttpContent content = response.Content)
            {
                string responsJson = await content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<List<PilotDTO>>(responsJson);
            }
        }
    }
}
