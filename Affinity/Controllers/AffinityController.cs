using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Affinity.Models;
using Microsoft.AspNetCore.Http;

namespace Affinity.Controllers
{
    public class AffinityController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AffinityController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/login")]
        public async Task<IActionResult> Login()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (authenticated)
            {
                return RedirectToAction("GetGraphs", "Graph");
            }

            return View();
        }
        [Route("/register")]
        public async Task<IActionResult> Register()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (authenticated)
            {
                return RedirectToAction("GetGraphs", "Graph");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
