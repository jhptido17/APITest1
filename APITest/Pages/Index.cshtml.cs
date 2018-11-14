using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APITest.Pages
{
    public class IndexModel : PageModel
    {
        public string errorMsg;
        public bool sessionExist = false;

        public void OnGet()
        {

        }

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
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            Console.WriteLine("_________Usuario_______: " + username + " _______________password____________: " + password);
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            HttpContext.Session.SetString("Authentication",encoded);
            HttpContext.Session.SetString("user",username);
        }
    }
}