using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerMobileApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        // GET: api/Screenshot
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(100)
            };
            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5000/screenshot");
            var image = await response.Content.ReadAsStreamAsync();
            return File(image, "image/jpg");
        }
    }
}
