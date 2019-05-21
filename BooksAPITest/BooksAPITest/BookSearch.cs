using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BooksAPITest
{
    class BookSearch
    {
        private readonly string configFile = "C:/Users/owena/git/Books API Test/BooksAPITest/BooksAPITest/config.txt";
        public static BooksService service = new BooksService(new BaseClientService.Initializer
        {
        });

        public async Task CreateServiceObject()
        {
            Dictionary<string, string> config = ReadConfigFile(configFile);

            UserCredential credential;
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = config["client_id"],
                    ClientSecret = config["client_secret"]
                },
                new[] { BooksService.Scope.Books },
                "user",
                CancellationToken.None,
                new FileDataStore("Books.ListMyLibrary"));

            service = new BooksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApiKey = config["api_key"],
                ApplicationName = "AR Books"
            });
        }

        private Dictionary<string, string> ReadConfigFile(string filePath)
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
            {
                string[] words = line.Split(' ');
                words[0] = words[0].Replace(":", string.Empty);
                configDict.Add(words[0], words[1]);
            }
            return configDict;
        }

        public static async Task<Volume> SearchISBN(string isbn)
        {
            Console.WriteLine("Executing a book search request for ISBN: {0} ...", isbn);
            // Call API
            var result = await service.Volumes.List(isbn).ExecuteAsync();

            if (result != null && result.Items != null)
            {
                var item = result.Items.FirstOrDefault();
                return item;
            }
            return null;
        }

        public static async Task<Bookshelf> ListMyFirstShelf(string userId)
        {
            Console.WriteLine("Listing the first bookshelf of user with ID {0}...", userId);
            // Call API
            var result = await service.Mylibrary.Bookshelves.List().ExecuteAsync();
            if (result != null && result.Items != null)
            {
                var item = result.Items.First();
                return item;
            }
            return null;
        }

        public static async Task<Bookshelves> ListAllMyShelves(string userId)
        {
            Console.WriteLine("Listing all bookshelves of user with ID {0}...", userId);
            // Call API
            var result = await service.Mylibrary.Bookshelves.List().ExecuteAsync();
            if (result != null && result.Items != null)
            {
                return result;
            }
            return null;
        }

        public static async Task<Volumes> ListVolumesOnShelf(string shelfId)
        {
            Console.WriteLine("Listing volumes on shelf with ID {0}...", shelfId);
            // Call API
            var result = await service.Mylibrary.Bookshelves.Volumes.List(shelfId).ExecuteAsync();
            if (result != null && result.Items != null)
            {
                return result;
            }
            return null;
        }
    }
}
