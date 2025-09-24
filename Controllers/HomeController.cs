using Microsoft.AspNetCore.Mvc;

namespace ProjectManagerMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
