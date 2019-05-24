using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BooksAPILibrary;

namespace BooksAPITest
{
    class Program
    {
        //private readonly string configFile = "C:/Users/owena/git/BooksAPITest/BooksAPITest/BooksAPITest/config.txt";

        private static void Main(string[] args)
        {
            try
            {
                new BookSearch().CreateServiceObject("C:/Users/owena/git/BooksAPITest/BooksAPITest/BooksAPITest/config.txt").Wait();
                new BooksAPIService().CreateServiceObject("C:/Users/owena/git/BooksAPITest/BooksAPITest/BooksAPITest/config.txt").Wait();
                TestApi();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR {0}", e.Message);
                }
            }
        }

        private static void TestApi()
        {
            string isbn = "0071807993";
            string userId = "105479000489381502269";
            string shelfId = "0";

            Console.WriteLine(BooksAPIService.service.ApplicationName);

            Console.WriteLine("\n------------------------------------------------------------------------");

            var output = BookSearch.SearchISBN(isbn);
            var result = output.Result;

            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Book Name: \t" + result.VolumeInfo.Title);
            Console.WriteLine("Author: \t" + result.VolumeInfo.Authors.FirstOrDefault());
            Console.WriteLine("Publisher: \t" + result.VolumeInfo.Publisher);
            Console.WriteLine("------------------------------------------------------------------------\n");

            var shelf = BookSearch.ListMyFirstShelf(userId);
            var rest = shelf.Result;

            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Shelf Name: \t {0}", rest.Title);
            Console.WriteLine("Access: \t {0}", rest.Access);
            Console.WriteLine("------------------------------------------------------------------------\n");

            //var shelves = BookSearch.ListAllMyShelves(userId);
            var shelves = BooksAPIService.ListAllMyShelves(userId);
            var s = shelves.Result.Items;
            Console.WriteLine("------------------------------------------------------------------------");
            foreach (var bookshelf in s)
            {
                Console.WriteLine("Shelf name: \t{0}", bookshelf.Title);
                Console.WriteLine("Access type: \t{0}", bookshelf.Access);
                Console.WriteLine("Shelf ID: \t{0}", bookshelf.Id);
            }
            Console.WriteLine("------------------------------------------------------------------------\n");

            //var booksOnShelf = BookSearch.ListVolumesOnShelf(shelfId);
            var booksOnShelf = BooksAPIService.ListVolumesOnShelf(shelfId);
            var books = booksOnShelf.Result.Items;
            Console.WriteLine("------------------------------------------------------------------------");
            foreach (var book in books)
            {
                Console.WriteLine("Book name: \t{0}", book.VolumeInfo.Title);
                Console.WriteLine("Book ID: \t{0}", book.Id);
                Console.WriteLine("Authors:");
                foreach (var author in book.VolumeInfo.Authors)
                {
                    Console.WriteLine("\tAuthor: \t{0}", author);
                }
                Console.WriteLine("Embeddable: \t{0}", book.AccessInfo.Embeddable);
            }
            Console.WriteLine("------------------------------------------------------------------------");
        }
    }
}
