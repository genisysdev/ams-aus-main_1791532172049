//using Microsoft.SharePoint.Client;
//using System;
//using System.IO;
//using System.Security;
//using System.Web;

//public class SharePointHelper
//{
//    private readonly string _siteUrl;
//    private readonly string _username;
//    private readonly string _password;

//    // Constructor to initialize SharePoint credentials and site URL
//    public SharePointHelper(string siteUrl, string username, string password)
//    {
//        _siteUrl = siteUrl;
//        //_username = username;
//        _password = password;
//    }

//    // Establish connection to SharePoint site
//    private ClientContext GetClientContext()
//    {
//        var securePassword = new SecureString();
//        foreach (char c in _password)
//            securePassword.AppendChar(c);

//        var credentials = new SharePointOnlineCredentials(_username, securePassword);
//        var context = new ClientContext(_siteUrl)
//        {
//            Credentials = credentials
//        };

//        // Test the context by loading web title (optional)
//        try
//        {
//            context.Load(context.Web, w => w.Title);
//            context.ExecuteQuery();
//        }
//        catch (IdcrlException ex)
//        {
//            Console.WriteLine("Authentication failed: " + ex.Message);
//            throw;
//        }

//        return context;
//    }

//    // Check if the library exists
//    public bool LibraryExists(string libraryName)
//    {
//        try
//        {
//            using (var context = GetClientContext())
//            {
//                var web = context.Web;
//                var list = web.Lists.GetByTitle(libraryName);
//                context.Load(list);
//                context.ExecuteQuery();
//                return true;
//            }
//        }
//        catch (ServerException ex)
//        {
//            if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
//                return false;
//            throw;
//        }
//    }

//    // Create a new SharePoint library
//    public void CreateLibrary(string libraryName)
//    {
//        using (var context = GetClientContext())
//        {
//            var web = context.Web;
//            var creationInfo = new ListCreationInformation
//            {
//                Title = libraryName,
//                TemplateType = (int)ListTemplateType.DocumentLibrary
//            };
//            web.Lists.Add(creationInfo);
//            context.ExecuteQuery();
//        }
//    }

//    // Upload a file to the specified SharePoint library
//    public void UploadFileToLibrary(string libraryName, HttpPostedFileBase file)
//    {
//        using (var context = GetClientContext())
//        {
//            var web = context.Web;
//            var library = web.Lists.GetByTitle(libraryName);
//            context.Load(library.RootFolder);
//            context.ExecuteQuery();

//            string fileName = Path.GetFileName(file.FileName);
//            var fileCreationInfo = new FileCreationInformation
//            {
//                Content = ReadFully(file.InputStream),
//                Url = Path.Combine(library.RootFolder.ServerRelativeUrl, fileName),
//                Overwrite = true
//            };

//            library.RootFolder.Files.Add(fileCreationInfo);
//            context.ExecuteQuery();
//        }
//    }

//    // Helper function to read the input stream fully
//    private static byte[] ReadFully(Stream input)
//    {
//        using (MemoryStream ms = new MemoryStream())
//        {
//            input.CopyTo(ms);
//            return ms.ToArray();
//        }
//    }
//}
