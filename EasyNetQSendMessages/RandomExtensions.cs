using System;
using System.Linq;

namespace EasyNetQSendMessages
{
    public static class RandomExtensions
    {
        public static string RandomString(this Random random)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";
            var count = random.Next(3, 12);
            var chars = Enumerable.Range(0, count)
                .Select(x => alphabet[random.Next(alphabet.Length)])
                .ToArray();
            chars[0] = Char.ToUpper(chars[0]);
            return new String(chars);
        }
    }
}
