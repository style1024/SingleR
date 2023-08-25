using Microsoft.AspNetCore.Mvc;

namespace CoreMVC_SignalR_Chat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChatRoom(string roomName)
        {
            return View(model: roomName);
        }
    }
}
