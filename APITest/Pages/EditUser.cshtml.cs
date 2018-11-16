using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using APITest.Models;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace APITest.Pages
{
    public class EditUserModel : PageModel
    {
        public Users json;
        public string errorMsg;

        public void OnPostShowUser(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("users/"+id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<Users>(result);
	        }
        }

        public void OnPostEditUser(int id)
        {
            if (Request.Form["username"] == "" && Request.Form["password"] == "" && Request.Form["role"] == "")
            {
                errorMsg = "Username, Password and Role are blank";
                //return RedirectToPage("/Users");
                OnPostShowUser(id);
            }
            else
            {
                var content = UpdateContent();
                //var userName = Request.Form["username"];
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:5001/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                MediaTypeFormatter formatter =  new JsonMediaTypeFormatter();
                var response = client.PutAsync<Users>("users/"+id, content, formatter);
                response.Wait();
                if (response.Result.IsSuccessStatusCode)
                {
                    //return RedirectToPage("/Users");
                    OnPostShowUser(id);
                }
                else
                {   
                    errorMsg = "Error: User not added: " + response.Result.Content.ReadAsStringAsync().Result.Replace("\"", "");;
                    //return RedirectToPage("/Users");
                    OnPostShowUser(id);
                }
            }
        }

        public Users UpdateContent()
        {   
            if (Request.Form["password"] == "" && Request.Form["role"] == "")
            {
                return new Users { Username = Request.Form["username"], Status = null };
            }
            if (Request.Form["username"] == "" && Request.Form["role"] == "")
            {
                return new Users { Password = Request.Form["password"], Status = null };
            }
            if (Request.Form["username"] == "" && Request.Form["password"] == "")
            {
                return new Users { Role = Request.Form["role"], Status = null };
            }
            if (Request.Form["username"] == "")
            {
                return new Users { Password = Request.Form["password"], Role = Request.Form["role"], Status = null };
            }
            if (Request.Form["password"] == "")
            {
                return new Users { Username = Request.Form["username"], Role = Request.Form["role"], Status = null };
            }
            if (Request.Form["role"] == "")
            {
                return new Users { Username = Request.Form["username"], Password = Request.Form["password"], Status = null };
            }
            return new Users { Username = Request.Form["username"], Password = Request.Form["password"], Role = Request.Form["role"], Status = null };
        }
    }
}