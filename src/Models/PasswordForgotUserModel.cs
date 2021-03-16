using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace NoteApp_UserManagement_Api.Models
{
    public class PasswordForgotUserModel
    {
      
        public string Id { get; set; }
        

        public string Password { get; set; }

    }
}
