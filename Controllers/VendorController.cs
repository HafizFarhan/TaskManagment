using Microsoft.AspNetCore.Mvc;

namespace Injazat.Presentation.Controllers
{
    public class VendorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
