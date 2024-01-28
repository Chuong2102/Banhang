using Microsoft.AspNetCore.Mvc;
using SV20T1080026.Web.Models;
using System.Diagnostics;

namespace SV20T1080026.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var data = new HomeIndexModel();

            data.TitleMessage = "List of persons and students";
            data.ListOfPersons = new PersonDAL().List();
            data.ListOfStudents = new StudentDAL().List();

            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult InputEmployee()
        {
            return View();
        }

        public IActionResult GetEmployee(InputEmployee e)
        {
            return Content($"name: {e.Name}, age: {e.Age}");
        }
    }
}