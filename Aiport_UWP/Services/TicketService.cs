using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Aiport_UWP.DTO;
using Newtonsoft.Json;

namespace Aiport_UWP.Services
{
    public class TicketService
    {
        public async Task<List<TicketDTO>> GetAllTickets()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync("http://localhost:62444/api/ticket"))
            using (HttpContent content = response.Content)
            {
                string responsJson = await content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<List<TicketDTO>>(responsJson);
            }
        }

        public async Task DeleteTicket(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync("http://localhost:62444/api/ticket/" + id);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }
        }

        public async Task AddTicket(TicketDTO ticket)
        {
            using (HttpClient client = new HttpClient())
            {
                var ticketJson = JsonConvert.SerializeObject(ticket);
                HttpResponseMessage response = await client.PostAsync("http://localhost:62444/api/ticket", 
                    new StringContent(ticketJson, UnicodeEncoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }
        }

        public async Task UpdateTicket(TicketDTO ticket, int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var ticketJson = JsonConvert.SerializeObject(ticket);
                HttpResponseMessage response = await client.PutAsync("http://localhost:62444/api/ticket/"+id, 
                    new StringContent(ticketJson, UnicodeEncoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }
        }

        
    }
}
