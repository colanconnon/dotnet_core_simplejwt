using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnet_core_simple_jwt;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        [Authorization("test")]
        public IActionResult Index()
        {
            return Ok(this.HttpContext.User.Identity.Name);
        }

        public IActionResult About()
        {
            return new JsonResult(new {hello = "world"});
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
