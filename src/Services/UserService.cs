#region snippet_UserServiceClass
using NoteApp_UserManagement_Api.Models;
using NoteApp_UserManagement_Api.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;

namespace NoteApp_UserManagement_Api.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        #region snippet_UserServiceConstructor
        public UserService(IUserDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }
        #endregion
        public UserModel Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _users.Find(user => user.Email==email).SingleOrDefault();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return getUserModelFromUser(user);
        }

        public List<UserModel> Get() =>
            _users.Find(user => true).ToList().Select(user1=>  getUserModelFromUser(user1)).ToList();

        public UserModel Get(string id) =>
            getUserModelFromUser(_users.Find<User>(user => user.Id == id).FirstOrDefault());

        public UserModel Create(RegisterUserModel registerUserModel)
        {
            
            User newUser = getUserFromRegisterUserModel(registerUserModel);
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(registerUserModel.Password, out passwordHash, out passwordSalt);
            newUser.PasswordHash=passwordHash;
            newUser.PasswordSalt=passwordSalt;
            _users.InsertOne(newUser);
            return getUserModelFromUser(newUser);
        }

        public void Update(string id, UpdateUserModel userIn) {

            User editedUser = getUserFromUpdateUserModel(userIn);
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(userIn.Password, out passwordHash, out passwordSalt);
            editedUser.PasswordHash=passwordHash;
            editedUser.PasswordSalt=passwordSalt;

            _users.ReplaceOne(user => user.Id == id, editedUser);
        }
        public void PasswordForgot(string id, PasswordForgotUserModel userIn) {

            
        }
        

        public void Remove(UserModel userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) => 
            _users.DeleteOne(user => user.Id == id);

         private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        private  UserModel getUserModelFromUser(User user ){
            UserModel userModel = new UserModel();
            userModel.Id=user.Id;
            userModel.UserName=user.UserName;
            userModel.Email=user.Email;
            return userModel;

        }
        private  User getUserFromRegisterUserModel(RegisterUserModel registerUserModel ){
            User user = new User();
            
            user.UserName=registerUserModel.UserName;
            user.Email=registerUserModel.Email;
            return user;

        }
        private User getUserFromUpdateUserModel(UpdateUserModel updateUserModel)
        {
            User user = new User();
            user.Id=updateUserModel.Id;
            user.UserName=updateUserModel.UserName;
            user.Email=updateUserModel.Email;
            return user;
        }
    }
}
#endregion
