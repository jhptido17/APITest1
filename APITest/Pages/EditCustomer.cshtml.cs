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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace APITest.Pages
{
    public class EditCustomerModel : PageModel
    {
        public Customers json;
        public string errorMsg;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public IFormFile Upload { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        public EditCustomerModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnPostShowCustomer(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("customers/"+id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
	        {
		        var result = response.Result.Content.ReadAsStringAsync().Result;
                json = JsonConvert.DeserializeObject<Customers>(result);
                name = json.Name;
                surname = json.Surname;
	        }
        }

         public void OnPostEditCustomer(int id)
        {
            if (Request.Form["name"] == "" && Request.Form["surname"] == "" && Upload == null)
            {
                errorMsg = "Name, Surname and Image are blank";
                //return RedirectToPage("/Customers");
                OnPostShowCustomer(id);
            }
            else
            {
                var content = UpdateContent();
                var client = new HttpClient();
                client.BaseAddress = new Uri(_configuration.GetSection("APIUri").Value);
                client.DefaultRequestHeaders.Accept.Clear();
                client.MaxResponseContentBufferSize = 256000;
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                MediaTypeFormatter formatter =  new JsonMediaTypeFormatter();
                var response = client.PutAsync<Customers>("customers/"+id, content, formatter);
                response.Wait();
                if (response.Result.IsSuccessStatusCode)
                {
                    if (Upload != null)
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Basic " + HttpContext.Session.GetString("Authentication"));
                        byte[] data;
                        using (var br = new BinaryReader(Upload.OpenReadStream()))
                            data = br.ReadBytes((int)Upload.OpenReadStream().Length);
                        ByteArrayContent bytes = new ByteArrayContent(data);
                        MultipartFormDataContent ImageContent = new MultipartFormDataContent();
                        ImageContent.Add(bytes, "file", Upload.FileName);
                        var Imageresponse = client.PostAsync("customersimage/"+id, ImageContent);
                        if (Imageresponse.Result.IsSuccessStatusCode)
                        {
                            errorMsg = "Image Upload to Customer with Id: " + id;
                            //return RedirectToPage("/Customers");
                            OnPostShowCustomer(id);
                        }
                    }
                    //return RedirectToPage("/Customers");
                    OnPostShowCustomer(id);
                }
                else
                {   
                    errorMsg = "Error: User not added: " + response.Result.Content.ReadAsStringAsync().Result.Replace("\"", "");;
                    //return RedirectToPage("/Customers");
                    OnPostShowCustomer(id);
                }
            }
        }

        public Customers UpdateContent()
        {   if (Request.Form["name"] == "" && Request.Form["surname"] == "")
            {
                return new Customers { };
            }
            if (Request.Form["name"] == "")
            {
                return new Customers { Surname = Request.Form["surname"] };
            }
            if (Request.Form["surname"] == "")
            {
                return new Customers { Name = Request.Form["name"] };
            }
            return new Customers { Name = Request.Form["name"], Surname = Request.Form["surname"] };
        }
    }
}