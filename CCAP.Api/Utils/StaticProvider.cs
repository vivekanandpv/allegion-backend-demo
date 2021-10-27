using System;
using System.Text;

namespace CCAP.Api.Utils {
    public static class StaticProvider {
        public const int MaxWrongAttempts = 5;
        private const string allowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ1234567890";
        
        public static string GetRandomPassword(int length) {
            var random = new Random();
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++) {
                var randomIndex = random.Next(0, allowedCharacters.Length - 1);
                sb.Append(allowedCharacters[randomIndex]);
            }

            return sb.ToString();
        }
    }
}