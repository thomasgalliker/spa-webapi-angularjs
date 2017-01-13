
using System.Web.Mvc;

namespace HomeCinema.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            return this.View();
        }
    }
}