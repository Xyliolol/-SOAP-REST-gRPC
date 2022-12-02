using LibraryService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LibraryService.Services.Impl
{
    public class LibraryDatabaseContext : ILibraryDatabaseContextService
    {
        private IList<Book> _LibraryDb;
        public IList<Book> Books => _LibraryDb;

        public LibraryDatabaseContext()
        {
            Initialize();
        }

        private void Initialize()
        {
            _LibraryDb = (List<Book>)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Properties.Resources.books), typeof(List<Book>));
        }
    }
}