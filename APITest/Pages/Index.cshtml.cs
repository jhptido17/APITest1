using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APITest.Models;
using System.Net.Http.Formatting;

namespace APITest.Pages
{
    public class IndexModel : PageModel
    {
        public string errorMsg;
        public bool sessionExist = false;
        public Users json;

        public void OnPostLogOut()
        {
            HttpContext.Session.Remove("Authentication");
            HttpContext.Session.Remove("user");
            HttpContext.Session.Clear();
        }

        public void OnPostLogIn()
        {
            if(Request.Form["username"] == "" && Request.Form["password"] == "")
            {
                errorMsg = "Username, Password and Role are blank";
            }
            var content = new Users { Username = Request.Form["username"], Password = Request.Form["password"] };
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            MediaTypeFormatter formatter =  new JsonMediaTypeFormatter();
            var response = client.PostAsync<Users>("login", content, formatter);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        errorMsg = response.Result.Content.ReadAsStringAsync().Result;
                string username = Request.Form["username"];
                string password = Request.Form["password"];
                string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                HttpContext.Session.SetString("Authentication",encoded);
                HttpContext.Session.SetString("user",username);
	        }
            else
            {
                errorMsg = response.Result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}