
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure_storage_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, sir");

            ProcessAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to finish, sir");
            Console.ReadLine();
        }

        private static async Task UploadPhotoFromPC()
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string sourceFile = null;

            Console.WriteLine("Enter storageConnectionString:");
            string storageConnectionString = Console.ReadLine();
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {

                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    Console.WriteLine("Do you want a new container? Y/N");
                    string input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        cloudBlobContainer = cloudBlobClient.GetContainerReference("quickstartblobs" + Guid.NewGuid().ToString());
                        await cloudBlobContainer.CreateAsync();
                        Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Enter the name of the container.");
                        cloudBlobContainer = cloudBlobClient.GetContainerReference(Console.ReadLine());
                        Console.WriteLine("Got the container. {0}", cloudBlobContainer.Name);
                    }

                    string path = "C:\\Users\\ivana\\OneDrive\\Desktop\\";
                    string file = "duckTest.jpg";

                    path = ChangePath(path);
                    file = ChangeFile(file);

                    sourceFile = Path.Combine(path, file);

                    Console.WriteLine("Temp file = {0}", sourceFile);

                    Console.WriteLine("Do you want to upload the file? Y/N");
                    input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        Console.WriteLine("Uploading to Blob storage as blob '{0}'", file);
                        Console.WriteLine();

                        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file);
                        await cloudBlockBlob.UploadFromFileAsync(sourceFile);
                    }

                    Console.WriteLine("Listing blobs in container.");

                    BlobContinuationToken blobContinuationToken = null;
                    do
                    {
                        var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                        // Get the value of the continuation token returned by the listing call. 
                        blobContinuationToken = results.ContinuationToken;

                        foreach (IListBlobItem item in results.Results)
                        {
                            Console.WriteLine(item.Uri);
                        }
                    } while (blobContinuationToken != null); // Loop while the continuation token is not null.

                    Console.WriteLine();
                }


                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
                finally
                {
                    Console.WriteLine("Do you want to delete the container? Y/N");
                    string input = Console.ReadLine();
                    if (input == "Y" || input == "y" && cloudBlobContainer != null)
                    {
                        Console.WriteLine("Deleting the container and any blobs it contains");
                        await cloudBlobContainer.DeleteIfExistsAsync();
                    }
                }

            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +

                    "Add a environment variable named 'storageconnectionstring' with your storage " +

                    "connection string as a value.");
            }

        }

        private static async Task ProcessAsync()
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string sourceFile = null;

            Console.WriteLine("Enter storageConnectionString:");
            string storageConnectionString = Console.ReadLine();

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    Console.WriteLine("Do you want a new container? Y/N");
                    string input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        cloudBlobContainer = cloudBlobClient.GetContainerReference("quickstartblobs" + Guid.NewGuid().ToString());
                        await cloudBlobContainer.CreateAsync();
                        Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Enter the name of the container.");
                        cloudBlobContainer = cloudBlobClient.GetContainerReference(Console.ReadLine());
                        Console.WriteLine("Got the container. {0}", cloudBlobContainer.Name);
                    }

                    string path = "C:\\Users\\ivana\\OneDrive\\Desktop\\";
                    string file = "AzureTest.txt";

                    path = ChangePath(path);
                    file = ChangeFile(file);

                    sourceFile = Path.Combine(path, file);

                    Console.WriteLine("Temp file = {0}", sourceFile);

                    Console.WriteLine("Do you want to upload the file? Y/N");
                    input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        Console.WriteLine("Uploading to Blob storage as blob '{0}'", file);
                        Console.WriteLine();

                        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file);
                        await cloudBlockBlob.UploadFromFileAsync(sourceFile);
                    }

                    Console.WriteLine("Listing blobs in container.");

                    BlobContinuationToken blobContinuationToken = null;
                    do
                    {
                        var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                        // Get the value of the continuation token returned by the listing call. 
                        blobContinuationToken = results.ContinuationToken;

                        foreach (IListBlobItem item in results.Results)
                        {
                            Console.WriteLine(item.Uri);
                        }
                    } while (blobContinuationToken != null); // Loop while the continuation token is not null.

                    Console.WriteLine();
                }


                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
                finally
                {
                    Console.WriteLine("Do you want to delete the container? Y/N");
                    string input = Console.ReadLine();
                    if (input == "Y" || input == "y" && cloudBlobContainer != null)
                    {
                        Console.WriteLine("Deleting the container and any blobs it contains");
                        await cloudBlobContainer.DeleteIfExistsAsync();
                    }
                }
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +

                    "Add a environment variable named 'storageconnectionstring' with your storage " +

                    "connection string as a value.");
            }
        }

        public static string ChangePath(string path)
        {
            Console.WriteLine("Do you want to change the path from '" + path + "'? Y/N");
            string input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                Console.WriteLine("Enter the path: ");
                return Console.ReadLine();
            }
            else if (input == "N" || input == "n")
            {
                return path;
            }
            else
            {
                Console.WriteLine("One more time, please, I didn't get you");
                return ChangePath(path);
            }
        }

        public static string ChangeFile(string file)
        {
            Console.WriteLine("Do you want to change the file from '" + file + "'? Y/N");
            string input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                Console.WriteLine("Enter the file name: ");
                return Console.ReadLine();
            }
            else if (input == "N" || input == "n")
            {
                return file;
            }
            else
            {
                Console.WriteLine("One more time, please, I didn't get you");
                return ChangeFile(file);
            }
        }
    }
}


