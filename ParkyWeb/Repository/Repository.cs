using Newtonsoft.Json;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _clientFactory;

        public Repository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> CreateAsync(string url, T objToCreate)
        {
            //Create a new post request
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            //If the object to create is not null, convert to object into JSON format
            if(objToCreate != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(objToCreate), Encoding.UTF8, "application/json");
            } else
            {
                return false;
            }

            //Send post request and await for response
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            //If reponse status is 201, return true
            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            
            return false;
        }

        public async Task<bool> DeleteAsync(string url, int Id)
        {
            //Create a new delete request
            var request = new HttpRequestMessage(HttpMethod.Delete, url + Id);

            //Send delete request and await for response
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            //If response status is 204, return true
            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            } 
            
            return false;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            //Create a new get request
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            //Send get request and await for response
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            //If response status is 200, convert JSON back into list of T objects
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            }

            return null;
        }

        public async Task<T> GetAsync(string url, int Id)
        {
            //Create a new get request
            var request = new HttpRequestMessage(HttpMethod.Get, url + Id);

            //Send get request and await for response
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            //If response status is 200, convert JSON back into T object
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }

            return null;
        }

        public async Task<bool> UpdateAsync(string url, T objToUpdate)
        {
            //Create a new post request
            var request = new HttpRequestMessage(HttpMethod.Patch, url);

            //If the object to update is not null, convert to object into JSON format
            if (objToUpdate != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(objToUpdate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }

            //Send post request and await for response
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            //If reponse status is 204, return true
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }
    }
}
