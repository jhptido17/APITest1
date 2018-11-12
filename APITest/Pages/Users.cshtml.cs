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

namespace APITest.Pages
{
    public class UsersModel : PageModel
    {
        public IEnumerable<Users> json;
        public string errorMsg;

        public void OnGet()
        {

        }

        public void OnGetShowUsers()
        {   
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            string username = "user1";
            string password = "1234";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("users");
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<IEnumerable<Users>>(result);
                RedirectToPage("/Users");
	        }
        }

        public void OnPostUsers()
        {
            if(Request.Form["username"] == "" && Request.Form["password"] == "" && Request.Form["role"] == "")
            {
                errorMsg = "Username, Password and Role are blank";
                OnGetShowUsers();
                RedirectToPage("/Users");
                return;
            }
            var content = new Users { Username = Request.Form["username"], Password = Request.Form["password"], Role = Request.Form["role"], Status = null };
            //var userName = Request.Form["username"];
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            string username = "user1";
            string password = "1234";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            MediaTypeFormatter formatter =  new JsonMediaTypeFormatter();
            var response = client.PostAsync<Users>("users", content, formatter);
            response.Wait();

            if (response.Result.IsSuccessStatusCode)
            {
                OnGetShowUsers();
                RedirectToPage("/Users");
            }
            else
            {
                errorMsg = "Error: User not added";
                OnGetShowUsers();
                RedirectToPage("/Users");
            }
        }

        public void OnPostDeleteUsers()
        {
            Console.WriteLine("prueba de borrado");
            OnGetShowUsers();
            RedirectToPage("/Users");
        }
    }
}