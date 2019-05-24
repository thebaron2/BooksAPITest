using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace BooksAPILibrary
{
    public class BooksAPIService
    {
        public static BooksService service = new BooksService(new BaseClientService.Initializer
        {
        });

        /// <summary>
        /// Creates a new <c>BooksService</c> object in place of the empty one created above.
        /// This service object is used in authenticating requests that access private 
        /// data in the Google Books API.
        /// </summary>
        /// <returns>
        /// A <c>Task</c> object, which isn't used but is required.
        /// </returns>
        public async Task CreateServiceObject(string filePath)
        {

            Dictionary<string, string> config = ReadConfigFile(filePath);

            // Create a credential object by calling the GoogleWebAuthorizationBroker
            // and put the client id and client secret in it.
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

            // Create the new BooksService in place of the old empty one.
            service = new BooksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApiKey = config["api_key"],
                ApplicationName = "AR Books"
            });
        }

        /// <summary>
        /// Reads the config file and stores the client id, 
        /// secret and api key in a dictionary
        /// </summary>
        /// <param name="filePath">String that points to the location of the file.</param>
        /// <returns>
        /// <c>Dictionary</c> with the client ID, secret and API key.
        /// </returns>
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

        /// <summary>
        /// Retrieves all <c>Bookshelves</c> in <c>Mylibrary</c>.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A <c>Task</c> object of type <c>Bookshelves</c>.
        /// </returns>
        public static async Task<Bookshelves> ListAllMyShelves(string userId)
        {
            Console.WriteLine("Listing all bookshelves of user with ID {0}...", userId);
            // Call API to retrieve bookshelves from Mylibrary.
            var result = await service.Mylibrary.Bookshelves.List().ExecuteAsync();
            // Check if result is not null.
            if (result != null && result.Items != null)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Retrieves all <c>Volumes</c> on a specific bookshelf 
        /// in <c>Mylibrary</c>.
        /// </summary>
        /// <param name="shelfId">
        /// A string representing the shelfId
        /// from which the books are to be returned.
        /// </param>
        /// <returns>
        /// A <c>Task</c> object of type <c>Volumes</c>
        /// </returns>
        public static async Task<Volumes> ListVolumesOnShelf(string shelfId)
        {
            Console.WriteLine("Listing volumes on shelf with ID {0}...", shelfId);
            // Call API to retrieve Volumes on the specific bookshelf.
            var result = await service.Mylibrary.Bookshelves.Volumes.List(shelfId).ExecuteAsync();
            // Check if result is not null.
            if (result != null && result.Items != null)
            {
                return result;
            }
            return null;
        }
    }
}
