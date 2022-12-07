using LibraryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.Services
{
    public interface ILibraryRepositoryService
    {
        IList<Book> GetByTitle(string title);
        IList<Book> GetByAuthor(string authorName);
        IList<Book> GetByCategory(string category);
    }
}