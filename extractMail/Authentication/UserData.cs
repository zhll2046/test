﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace Exchange101
{
    public interface IUserData
    {
        ExchangeVersion Version { get; }
        string EmailAddress { get; }
        SecureString Password { get; }
        Uri AutodiscoverUrl { get; set; }
    }

    public class UserDataFromConsole : IUserData
    {
        public static UserDataFromConsole UserData;

        public static IUserData GetUserData()
        {
            if (UserData == null)
            {
                GetUserDataFromConsole();
            }

            return UserData;
        }

        public static IUserData GetUserData(string emailAddress, string password)
        {
            if (UserData == null)
            {
                GetUserDataFromGivenMailAndPWD(emailAddress, password);
            }

            return UserData;
        }

        private static void GetUserDataFromGivenMailAndPWD(string emailAddress, string password)
        {
            UserData = new UserDataFromConsole();

            UserData.EmailAddress = emailAddress;
            UserData.Password = new SecureString();

            foreach (char c in password.ToCharArray())
                UserData.Password.AppendChar(c);

            UserData.Password.MakeReadOnly();

        }

        private static void GetUserDataFromConsole()
        {
            UserData = new UserDataFromConsole();

            Console.Write("Enter email address: ");
            UserData.EmailAddress = Console.ReadLine();

            UserData.Password = new SecureString();

            Console.Write("Enter password: ");

            while (true)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);
                if (userInput.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (userInput.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (userInput.Key == ConsoleKey.Backspace)
                {
                    if (UserData.Password.Length != 0)
                    {
                        UserData.Password.RemoveAt(UserData.Password.Length - 1);
                    }
                }
                else
                {
                    UserData.Password.AppendChar(userInput.KeyChar);
                    Console.Write("*");
                }
            }

            Console.WriteLine();

            UserData.Password.MakeReadOnly();
        }

        public ExchangeVersion Version { get { return ExchangeVersion.Exchange2013; } }

        public string EmailAddress
        {
            get;
            private set;
        }

        public SecureString Password
        {
            get;
            private set;
        }

        public Uri AutodiscoverUrl
        {
            get;
            set;
        }
    }
}
