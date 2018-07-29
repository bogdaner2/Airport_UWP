using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aiport_UWP.Services
{
    public class CrudService<T>
    {
        private string _path;
        public CrudService(string type)
        {
            _path = "http://localhost:62444/api/" + type;
        }

        public async Task<List<T>> GetAll()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(_path))
            using (HttpContent content = response.Content)
            {
                string responsJson = await content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<List<T>>(responsJson);
            }
        }

        public async Task Delete(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(_path + "/" + id);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }
        }

        public async Task Create(T item)
        {
            using (HttpClient client = new HttpClient())
            {
                var itemJson = JsonConvert.SerializeObject(item);
                HttpResponseMessage response = await client.PostAsync(_path,
                    new StringContent(itemJson, UnicodeEncoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }
        }

        public async Task Update(T item, int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var itemJson = JsonConvert.SerializeObject(item);
                HttpResponseMessage response = await client.PutAsync(_path + "/" + id,
                    new StringContent(itemJson, UnicodeEncoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }
        }
    }
}
