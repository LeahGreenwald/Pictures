using System;

namespace PicturesAndPasswords.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        //public bool InSession { get; set; }
        //public bool CorrectPassword { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowMessage { get; set; }
    }
    public class UploadViewModel
    {
        public int Id { get; set; }
        public string Password { get; set; }
    }
}
