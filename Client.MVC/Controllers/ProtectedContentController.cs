using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

namespace Client.MVC.Controllers
{
    [Route("web")]
    [Authorize]
    public class ProtectedContentController : Controller
    {
        [Route("protectedWeb")]
        [HttpGet]
        public IActionResult ProtectedWebPage()
        {
            return View();
        }

        [Route("protectedAPI")]
        [HttpGet]
        public IActionResult ProtectedAPI()
        {
            return View();
        }

        [Route("protectedAPICall")]
        [HttpPost]
        public async Task<IActionResult> ProtectedAPICall()
        {
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetStringAsync("http://localhost:34599/api/protected");
            ViewBag.CallResult = JObject.Parse(response).ToString();

            return View("ProtectedAPI");
        }
    }
}
