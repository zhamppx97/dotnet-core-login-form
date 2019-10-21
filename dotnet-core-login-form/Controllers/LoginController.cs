using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dotnet_core_login_form.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_core_login_form.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Authentication(LoginViewModel loginModel)
        {
            string username = loginModel.Username;
            string password = loginModel.Password;

            if (username == null || password == null)
            {
                ViewBag.Alert = "Please input username and password";
                return View("Index");
            }

            if (HasPermission(username, password))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Alert = "You are not have permission";
                return View("Index");
            }
        }

        private string MD5Hash(string text)
        {
            #pragma warning disable IDE0067 // Dispose objects before losing scope
            MD5 md5 = new MD5CryptoServiceProvider();
            #pragma warning restore IDE0067 // Dispose objects before losing scope
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        private List<UserModel> CreateTempUser()
        {
            List<UserModel> temp = new List<UserModel>
            {
                new UserModel
                {
                    Username = "admin",
                    Password = "81dc9bdb52d04dc20036dbd8313ed055" // 1234
                }
            };
            return temp;
        }

        private bool HasPermission(string username, string password)
        {
            bool isFound = false;
            password = MD5Hash(password);
            List<UserModel> userModel = CreateTempUser();
            try
            {
                var query = from x in userModel
                            where x.Username.Equals(username) && x.Password.Equals(password)
                            select x;
                foreach (var item in query)
                {
                    isFound = true;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return isFound;
        }
    }
}