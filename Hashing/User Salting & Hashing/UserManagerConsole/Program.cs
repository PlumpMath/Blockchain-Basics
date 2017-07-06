using System;
using UserManager;

namespace UserManagerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string NameToValidate, PasswordToValidate;
            UserManager.UserManager UsrMgr = new UserManager.UserManager();
            Console.WriteLine("Registering new user ... ");
            User usr = UsrMgr.RegisterNewUser(args[0], args[1]);
            Console.WriteLine("New User {0} registered with salt of {1} and salted password of {2} .", usr.Name, usr.Salt, usr.HashedPassword);
            Console.WriteLine("                         ------------Validating user now-----------");
            Console.WriteLine("Enter UserName");
            NameToValidate = Console.ReadLine();
            Console.WriteLine("Enter Password");
            PasswordToValidate = Console.ReadLine();
            if (UsrMgr.ValidatePassword(NameToValidate, PasswordToValidate))
                Console.WriteLine("Valid user -- Logging in");
            else
                Console.WriteLine("Invalid User");

            Console.ReadKey();
            
        }
    }
}