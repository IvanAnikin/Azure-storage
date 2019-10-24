
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure_storage_1
{
    class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, sir");

            //ProcessAsync().GetAwaiter().GetResult();

            CloudStorageAccount storageAccount = null;
            Console.WriteLine("Enter storageConnectionString:");
            string storageConnectionString = ConfigurationManager.ConnectionStrings["AzureTableConStr"].ConnectionString;

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                Console.WriteLine("Enter Table Name to create");
                string tableName = Console.ReadLine();
                CloudTable cloudTable = tableClient.GetTableReference(tableName);
                await CreateNewTableAsync(cloudTable);

                string time = "time3"; //DateTime.UtcNow.ToLongTimeString()
                string nickname = "shrimp";
                string value = "message"; //Guid.NewGuid().ToString();

                /*
                Message message = new Message();
                message.Time = time;
                message.AuthorNickName = nickname;
                message.Body = value;

                message.AssignPartitionKey();
                message.AssignRowKey();

                TableOperation tableOperation = TableOperation.Insert(message);
                await cloudTable.ExecuteAsync(tableOperation); //ERROR : "bad request" ???
                Console.WriteLine("Record inserted");
                */

                await InsertRecordToTableAsync(cloudTable, time, nickname, value);

                await DisplayTableRecordsAsync(cloudTable);

            }
            else
            {
                Console.WriteLine(
                    "Wrong connection string");
            }
        }
        public static async Task DisplayTableRecordsAsync(CloudTable table)
        {
            TableQuery<Message> tableQuery = new TableQuery<Message>();
            TableContinuationToken token = null;

            foreach (Message message in await table.ExecuteQuerySegmentedAsync(tableQuery,token))
            {
                Console.WriteLine("Time : {0}", message.Time);
                Console.WriteLine("NIckname : {0}", message.AuthorNickName);
                Console.WriteLine("Message : {0}", message.Body);
                Console.WriteLine("******************************");
            }
        }

        public static async Task InsertRecordToTableAsync(CloudTable table, string time, string nickname, string value)
        {
            Message message = new Message();
            message.Time = time;
            message.AuthorNickName = nickname;
            message.Body = value;

            message.AssignPartitionKey();
            message.AssignRowKey();

            Message mess = await RetrieveRecordAsync(table, time, nickname);
            if (mess == null)
            {
                TableOperation tableOperation = TableOperation.Insert(message);
                await table.ExecuteAsync(tableOperation);
                Console.WriteLine("Record inserted");
            }
            else
            {
                Console.WriteLine("Record exists");
            }
        }
        public static async Task<Message> RetrieveRecordAsync(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation tableOperation = TableOperation.Retrieve<Message>(partitionKey, rowKey);
            TableResult tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result as Message;
        }

        public static async Task CreateNewTableAsync(CloudTable table)
        {
            if (!await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Table {0} already exists", table.Name);
                return;
            }
            Console.WriteLine("Table {0} created", table.Name);
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

    class Message : TableEntity
    {
        private string time;
        private string authorNickName;
        private string body;

        public string Time
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }

        public string AuthorNickName
        {
            get
            {
                return authorNickName;
            }

            set
            {
                authorNickName = value;
            }
        }

        public string Body
        {
            get
            {
                return body;
            }

            set
            {
                body = value;
            }
        }


        public void AssignRowKey()
        {
            this.RowKey = authorNickName;
        }
        public void AssignPartitionKey()
        {
            this.PartitionKey = time;
        }
    }

}


