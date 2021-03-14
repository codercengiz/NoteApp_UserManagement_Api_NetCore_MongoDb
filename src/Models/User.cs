using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace NoteApp_UserManagement_Api.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonElement("Name")]
        [JsonProperty("UserName")]
        public string UserName { get; set; }



        public string Email { get; set; }

        public string Password { get; set; }

    }
}
