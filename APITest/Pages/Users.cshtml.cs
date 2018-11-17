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
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace APITest.Pages
{
    public class UsersModel : PageModel
    {
        public IEnumerable<Users> json;
        public string errorMsg;
        private readonly IConfiguration _configuration;

        public UsersModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGetShowUsers()
        {   
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("users");
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<IEnumerable<Users>>(result);
	        }
        }

        public void OnPostUsers()
        {
            //if(Request.Form["username"] == "" && Request.Form["password"] == "" && Request.Form["role"] == "")
            if(Request.Form["username"] == "" || Request.Form["password"] == "" || Request.Form["roleValue"] == "")
            {
                errorMsg = "Username, Password and Role are blank";
                OnGetShowUsers();
                RedirectToPage("/Users");
                return;
            }
            //var content = new Users { Username = Request.Form["username"], Password = Request.Form["password"], Role = Request.Form["role"], Status = null };
            var content = new Users { Username = Request.Form["username"], Password = Request.Form["password"], Role = Request.Form["roleValue"], Status = null };
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            MediaTypeFormatter formatter =  new JsonMediaTypeFormatter();
            var response = client.PostAsync<Users>("users", content, formatter);
            response.Wait();

            if (response.Result.IsSuccessStatusCode)
            {
                OnGetShowUsers();
            }
            else
            {   
                errorMsg = "Error: User not added: " + response.Result.Content.ReadAsStringAsync().Result.Replace("\"", "");;
                OnGetShowUsers();
            }
        }

        public void OnPostDeleteUsers(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            var response = client.DeleteAsync("users/" + id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
            {
                OnGetShowUsers();
            }
            else
            {
                errorMsg = "Error: User not deleted";
                OnGetShowUsers();
            }
            OnGetShowUsers();
        }
    }
}