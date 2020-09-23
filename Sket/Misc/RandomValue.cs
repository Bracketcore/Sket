using System;
using System.Text;

namespace Bracketcore.Sket.Misc
{
    public static class RandomValue 
    {
        // Generate a random number between two numbers    
        public static int ToNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size    
        public static string ToString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        // Generate a random password    
        public static string RandomPassword()
        {
            var builder = new StringBuilder();
            builder.Append(ToString(4, true));
            builder.Append(ToNumber(1000, 9999));
            builder.Append(ToString(2, false));
            return builder.ToString();
        }
    }
}