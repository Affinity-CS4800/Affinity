using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Affinity.Models;
using System.Drawing;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Affinity.Controllers
{
    public class GraphController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GraphController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        [Route("/graph")]
        public async Task<string> Index()
        {
            Random rand = new Random();
            string tempID = "";
            int randomNum;
            while (tempID.Length < 8)
            {
                randomNum = rand.Next(128);
                if ((randomNum >= 49 && randomNum <= 57) || (randomNum >= 97 && randomNum <= 102))
                {
                    tempID += Convert.ToChar(randomNum);
                }
            }

            return tempID;
            /*return RedirectToRoute(new
            {
                Controller = "Graph",
                Action = "GetSpecificGraph",
                id = 0
            });*/   
        }

        [Route("/graph/{id}")]
        public async Task<IActionResult> GetSpecificGraph()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (!authenticated)
            {
                return RedirectToAction("Login","Affinity");
            }

            return Json(new { id = "1", value = "GetSpecificGraph" });
        }

        [Route("/graphs")]
        public async Task<IActionResult> GetGraphs()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (!authenticated)
            {
                return RedirectToAction("Login", "Affinity");
            }

            return Json(new { id = "2", value = "GetGraphs" });
        }
    }
}
