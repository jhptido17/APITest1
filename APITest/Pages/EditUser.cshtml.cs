﻿using System;
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

namespace APITest.Pages
{
    public class EditUserModel : PageModel
    {
        public Users json;
        public string errorMsg;

        public void OnGet()
        {

        }

        public void OnPostShowUser(int id)
        {
            Console.WriteLine("__________________________: " + id);
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            string username = "user1";
            string password = "1234";
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("users/"+id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<Users>(result);
	        }
        }

        public ActionResult OnPostEditUser(int id)
        {
            if (Request.Form["username"] == "" && Request.Form["password"] == "" && Request.Form["role"] == "")
            {
                errorMsg = "Username, Password and Role are blank";
                return RedirectToPage("/Users");
            }
            /*if(Request.Form["username"] == "" && Request.Form["password"] == "" && Request.Form["role"] == "")
            {
                errorMsg = "Username, Password and Role are blank";
                RedirectToPage("/Users");
                return;
            }*/
            Console.WriteLine("_________________________________________ESTA es la id: " + id);

            var content = UpdateContent();
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
            var response = client.PutAsync<Users>("users/"+id, content, formatter);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
            {
                return RedirectToPage("/Users");
            }
            else
            {   
                errorMsg = "Error: User not added: " + response.Result.Content.ReadAsStringAsync().Result.Replace("\"", "");;
                return RedirectToPage("/Users");
            }
        }

        public Users UpdateContent()
        {   
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
            if (Request.Form["username"] == "" && Request.Form["password"] == "")
            {
                return new Users { Role = Request.Form["role"], Status = null };
            }
            if (Request.Form["username"] == "" && Request.Form["role"] == "")
            {
                return new Users { Password = Request.Form["password"], Status = null };
            }
            if (Request.Form["password"] == "" && Request.Form["role"] == "")
            {
                return new Users { Username = Request.Form["username"], Status = null };
            }
            return new Users { Username = Request.Form["username"], Password = Request.Form["password"], Role = Request.Form["role"], Status = null };
        }
    }
}