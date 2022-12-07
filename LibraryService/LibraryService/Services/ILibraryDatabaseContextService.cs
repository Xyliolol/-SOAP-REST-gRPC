using LibraryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.Services
{
    public interface ILibraryDatabaseContextService
    {
        IList<Book> Books { get; }

    }
}