using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PicturesAndPasswords.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesAndPasswords.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images;Integrated Security=true;";

        private readonly IWebHostEnvironment _enviroment;
        public HomeController(IWebHostEnvironment environment)
        {
            _enviroment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile myImage, string password)
        {
            Guid guid = Guid.NewGuid();
            string fileName = $"{guid}-{myImage.FileName}";
            string finalFileName = Path.Combine(_enviroment.WebRootPath, "uploads", fileName);
            using var fileStream = new FileStream(finalFileName, FileMode.CreateNew);
            myImage.CopyTo(fileStream);
            ImagesDb db = new ImagesDb(_connectionString);
            Image image = new Image
            {
                Name = fileName,
                Password = password,
                Views = 0
            };
            db.AddImage(image);
            UploadViewModel vm = new UploadViewModel { Id = image.Id, Password = password };

            return View(vm);
        }
        public IActionResult ViewImage(int Id, string Password)
        {
            ImagesDb db = new ImagesDb(_connectionString);

            var ids = HttpContext.Session.Get<List<int>>("Ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            var InSession = ids.Any(i => i == Id);

            var image = db.GetImage(Id);

            bool CorrectPassword = image.Password == Password;
            bool message = false;
            if (Password != null)
            {
                message = true;
            }
            bool showImage = InSession || CorrectPassword;
            if (showImage)
            {
                db.UpdateImageViews(Id);
            }
            if (!InSession && CorrectPassword)
            {
                ids.Add(Id);
                HttpContext.Session.Set("Ids", ids);
            }
            ViewImageViewModel vm = new ViewImageViewModel
            {
                ShowMessage = message,
                ShowImage = showImage,
                Image = image
            };
            return View(vm);  
        }
    }
}
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        return value == null ? default(T) :
            JsonConvert.DeserializeObject<T>(value);
    }
}

