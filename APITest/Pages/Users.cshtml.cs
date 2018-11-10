using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using APITest.Models;

namespace APITest.Pages
{
    public class UsersModel : PageModel
    {
        public IEnumerable<Users> json;

        public void OnGet()
        {

        }

        public void OnPostShowUsers()
        {   Console.WriteLine("jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj");
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            string username = "user1";
            string password = "1234";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("users").Result;
            Console.WriteLine("_______________: " + response);

            if (response.IsSuccessStatusCode)
	        {
		        var result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("_______________: " + response);
                json = JsonConvert.DeserializeObject<IEnumerable<Users>>(result);
                Console.WriteLine(json.First().Id);
	        }
        }

        public void OnPost()
        {
            
        }
    }
}