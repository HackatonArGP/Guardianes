using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordFill
{
    public class PasswordChecker
    {
        private static Random m_random = new Random();

        public static bool CheckPassword(String password, String password_prefix, String hashed_password)
        {
            if (hashed_password.Equals("-"))
            {
                return true;
            }

            if (hashed_password.Equals(GenerateHashedPassword(password, password_prefix)))
            {
                return true;
            }
            return false;
        }

        public static string GeneratePrefix()
        {
            const int PREFIX_SIZE = 4;
            const String allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()[]{}<>.,;:/|-+=~";

            StringBuilder result = new StringBuilder();

            int i;
            for (i = 0; i < PREFIX_SIZE; ++i)
            {
                result.Append(allowedChars[m_random.Next(allowedChars.Length)]);
            }
            return result.ToString();
        }

        public static String GenerateHashedPassword(String password, String prefix)
        {

            byte[] input = Encoding.ASCII.GetBytes(prefix + password);
            sbyte[] hashed = new sbyte[input.Length];
            MD5 md = MD5.Create();
            int i;
            for (i = 0; i < input.Length; ++i)
            {
                int b = (input[i] * 107) + 42; // To make use of all bits
                hashed[i] = (sbyte)(b % 256);
            }
            StringBuilder result = new StringBuilder();
            hashed = (sbyte[])(Array)md.ComputeHash((byte[])(Array)hashed);
            for (int x = 0; x < hashed.Length; x++)
            {
                result.Append(Encode(hashed[x]));
            }
            return result.ToString();
        }


        private static String Encode(sbyte b)
        {
            String[] encoding = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
			"A", "B", "C", "D", "E", "F" };

            int i = ((int)b + 256) % 256; // Make sure it is positive
            return encoding[(i / 16)] + encoding[(i % 16)];
        }


    }
}
