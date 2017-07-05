using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UserManager;

namespace UserManager
{
    public class UserManager
    {
        private Dictionary<string, User> Users;
        private const int SaltByteLength = 16;
        private const int DerivedKeyLength = 16;
        private const int HashIterationCount = 2000;
        byte[] SaltBytes, PasswordHash;

        public UserManager()
        {
            Users = new Dictionary<string, User>();
        }

        private byte[] GeneratePasswordHash(string Password, byte[] SaltBytes)
        {
            byte[] HashedPassword;
            Rfc2898DeriveBytes Pbkdf2 = new Rfc2898DeriveBytes(Password, SaltBytes, HashIterationCount);
            HashedPassword = Pbkdf2.GetBytes(DerivedKeyLength);
            return HashedPassword;

        }

        private byte[] GenerateSalt()
        {
            byte[] SaltBytes = new byte[SaltByteLength];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(SaltBytes);
            return SaltBytes;

        }

        public bool CompareBytes(byte[] SavedPassword, byte[] EnteredPassword)
        {
            uint difference = (uint)EnteredPassword.Length ^ (uint)SavedPassword.Length;
            for (var i = 0; i < EnteredPassword.Length && i < SavedPassword.Length; i++)
            {
                difference |= (uint)(EnteredPassword[i] ^ SavedPassword[i]);
            }

            return difference == 0;
        }

        //Register a new user and save his/her hashed password along with the salt generated
        //by the GenerateSalt function. For this example i am saving it in the Users dictionary
        //but in general production scenario it will be saved in a database.
        public User RegisterNewUser(string UserName, string Password)
        {
            //Generate a random number to be used as a salt
            SaltBytes = GenerateSalt();
            //Hash a password using PBKDF2 with the generated salt
            PasswordHash = GeneratePasswordHash(Password, SaltBytes);


            User RegisteredUser = new User
            {
                Name = UserName,
                Salt = Convert.ToBase64String(SaltBytes),
                HashedPassword = Convert.ToBase64String(PasswordHash)
            };

            Users.Add(RegisteredUser.Name, RegisteredUser);

            return RegisteredUser;
        }

        //validate the userid and password combination by generating the hash again 
        //with the saved salt        
        public bool ValidatePassword(string UserName, string Password)
        {

            User Usr = Users[UserName];
            byte[] SavedSaltedHashedPassword = Convert.FromBase64String(Usr.HashedPassword);
            byte[] SavedSalt = Convert.FromBase64String(Usr.Salt);

            var GeneratedPasswordHash = GeneratePasswordHash(Password, SavedSalt);            

            return CompareBytes(SavedSaltedHashedPassword, GeneratedPasswordHash);
        }

        
    }
}
