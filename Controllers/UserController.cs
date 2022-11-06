using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Monitoring.Application.Services.Books.Commands.AddBook;
using Monitoring.Application.Services.Books.Queries.GetBook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;


namespace Monitoring.Site.Controllers
{
    public class UserController : Controller
    {
        private readonly IGetBookService _getBookService;
        private readonly IAddBookService _addBookService;

        //public HomeController(IGetBookService getBookService, IAddBookService addBookService)
        //{
        //    _getBookService = getBookService;
        //    _addBookService = addBookService;
        //}

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.title = "خانه";
            ViewBag.menuNavigation = "پیشخوان";
            ViewBag.menuItem = "خانه";
            ViewBag.activeNavigation = "navigationDashboard";
            ViewBag.activeItem = "itemHome";


            return View();
        }

        [HttpPost]
        public IActionResult AddBook(string Name, int Number)
        {
            var signupResult = _addBookService.Execute(new RequestAddBookDto
            {
                Name = Name,
                Number = Number,
            });           

            return Json(signupResult);
        }
    }
}
