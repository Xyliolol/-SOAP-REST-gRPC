using LibraryServiceReference;
using LibreryService.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibreryService.Web.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILogger<LibraryController> _logger;

        public LibraryController(ILogger<LibraryController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(SearchType searchType, string searchString)
        {
            try
            {
                LibraryWebServiceSoapClient client = new LibraryWebServiceSoapClient(LibraryWebServiceSoapClient.EndpointConfiguration.LibraryWebServiceSoap);

                if (!string.IsNullOrEmpty(searchString) && searchString.Length >= 3)
                {
                    switch (searchType)
                    {
                        case SearchType.Title:
                            return View(new BookCategoryViewModel
                            {
                                Books = client.GetBooksByTitle(searchString)
                            });
                        case SearchType.Author:
                            return View(new BookCategoryViewModel
                            {
                                Books = client.GetBooksByAuthor(searchString)
                            });
                        case SearchType.Category:
                            return View(new BookCategoryViewModel
                            {
                                Books = client.GetBooksByCategory(searchString)
                            });
                    }
                }
            }
            catch (Exception)
            {


            }
            return View(new BookCategoryViewModel
            {
                Books = new Book[] { }
            });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}