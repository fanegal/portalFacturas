using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
namespace test
{
    class Program
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";
        static void Main(string[] args)
        {
            ////Create a new XslCompiledTransform and load the compiled transformation.
            //XslCompiledTransform xslt = new XslCompiledTransform();
            //XsltSettings sets = new XsltSettings(true, true);
            //var resolver = new XmlUrlResolver();
            ////   xslt.Load("C:\\Users\\pako_\\Downloads\\cadenaoriginal_3_3.xslt", sets, resolver);
            //xslt.Load("C:\\Users\\pako_\\Downloads\\cadenaoriginal_TFD_1_1.xslt");
            ////xslt.Load("https://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/cadenaoriginal_TFD_1_1.xslt", sets, resolver);
            //// Create a writer for writing the transformed file.


            //// Execute the transformation and output the results to a file.
            //xslt.Transform("c:\\comprobantes\\2021\\2\\10\\CNM980114PI2\\AEAF901009GH6\\timbre.xml", "C:\\Validador\\CadenaOriginal.Txt");

            //Console.WriteLine("La cadena original es :");



            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/drive-dotnet-quickstart.json



            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
         //   listRequest.PageSize = 15;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();

        }
    }
}
