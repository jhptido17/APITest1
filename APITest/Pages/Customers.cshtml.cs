using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using APITest.Models;
using System.Net.Http.Headers;
using System.IO;

namespace APITest.Pages
{
    public class CustomersModel : PageModel
    {
        public IEnumerable<Customers> json;
        public string errorMsg;

        public void OnGet()
        {

        }

        public void OnGetShowCustomers()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            string username = "user1";
            string password = "1234";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("customers");
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<IEnumerable<Customers>>(result);
                //RedirectToPage("/Users");
	        }
        }

        public async Task<ActionResult> OnPostShowImage(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            string username = "user1";
            string password = "1234";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("customersimage/"+id);
            Console.WriteLine("_________________________________Imagen: " + response.Result.Content);
            
            if (response.Result.IsSuccessStatusCode)
            {
                var contentStream = response.Result.Content.ReadAsStreamAsync().Result;
                //new File(contentStream, "image/jpeg");
                
                //MemoryStream imageData = new MemoryStream(contentStream)
                Console.WriteLine("____________________________Image2: " +  File(contentStream, "image/jpeg"));
                //System.Net.Http.HttpContent content = response.Result.Content;
                //var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
                //return File(contentStream, GetContentType());
            }
            return RedirectToPage("/Customers");
        }

        private string GetContentType(string file)
        {
            var fileExtension = "." +file.Split('.')[file.Split('.').Length - 1];
            Console.WriteLine("image/" + fileExtension.TrimStart('.'));
            return "image/" + fileExtension.TrimStart('.');
        }
    }
}