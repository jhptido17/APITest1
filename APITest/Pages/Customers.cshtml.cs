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
using Microsoft.AspNetCore.Http;
using System.Net.Http.Formatting;
using Microsoft.Extensions.Configuration;

namespace APITest.Pages
{
    public class CustomersModel : PageModel
    {
        public IEnumerable<Customers> json;
        public string errorMsg;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public IFormFile Upload { get; set; }

        public CustomersModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGetShowCustomers()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("customers");
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<IEnumerable<Customers>>(result);
	        }
            else
            {
                errorMsg = response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        public void OnPostCustomers()
        {
            if(Request.Form["name"] == "" || Request.Form["surname"] == "")
            {
                errorMsg = "name and surname are required";
                OnGetShowCustomers();
            }
            else
            {
                var content = new Customers { Name = Request.Form["name"], Surname = Request.Form["surname"] };
                var client = new HttpClient();
                client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                MediaTypeFormatter formatter =  new JsonMediaTypeFormatter();
                var response = client.PostAsync<Customers>("customers", content, formatter);
                response.Wait();

                if (response.Result.IsSuccessStatusCode)
                {
                    OnGetShowCustomers();
                }
                else
                {   
                    errorMsg = "Error: User not added: " + response.Result.Content.ReadAsStringAsync().Result.Replace("\"", "");;
                    OnGetShowCustomers();
                }
            }
        }

        public void OnPostDeleteCustomer(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            var response = client.DeleteAsync("customers/" + id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
            {
                OnGetShowCustomers();
            }
            else
            {
                errorMsg = "Error: Customer not deleted";
                OnGetShowCustomers();               
            }
            OnGetShowCustomers();           
        }

        public void OnPostDeleteImage(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            var response = client.DeleteAsync("customersimage/"+id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
            {
                errorMsg = response.Result.Content.ReadAsStringAsync().Result;
                OnGetShowCustomers();   
            }
            else
            {
                errorMsg = "Error: Photo not deleted";
                OnGetShowCustomers();
            }
        }

        /*public async Task<IActionResult> OnPostShowImage(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("customersimage/"+id);
            
            if (response.Result.IsSuccessStatusCode)
            {
                var contentString = response.Result.Content.ReadAsStringAsync().Result.Replace("\\\\","/").Replace("\"","");
                //OnGetShowCustomers();
                return Redirect(contentString);
            }
            errorMsg = "Error image: " + response.Result.Content.ReadAsStringAsync().Result;
            //OnGetShowCustomers();
            return RedirectToPage("/Customers");
        }*/

        public void OnPostAddImage(int id)
        {
            if (Upload != null){
                var client = new HttpClient();
                client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
                client.DefaultRequestHeaders.Accept.Clear();
                client.MaxResponseContentBufferSize = 256000;
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
                byte[] data;
                using (var br = new BinaryReader(Upload.OpenReadStream()))
                    data = br.ReadBytes((int)Upload.OpenReadStream().Length);
                ByteArrayContent bytes = new ByteArrayContent(data);
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(bytes, "file", Upload.FileName);
                var response = client.PostAsync("customersimage/"+id, content);
                if (response.Result.IsSuccessStatusCode)
                {
                    errorMsg = "Image Upload to Customer with Id: " + id;
                    OnGetShowCustomers();
                }
                errorMsg = "Error image: " + response.Result.Content.ReadAsStringAsync().Result;
                OnGetShowCustomers();
            }
            OnGetShowCustomers();
        }
    }
}