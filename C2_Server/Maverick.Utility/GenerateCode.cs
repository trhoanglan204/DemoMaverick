using System.Text.RegularExpressions;

namespace Maverick.Utility
{
    public class GenerateCode
    {
        private static readonly string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random _rand = new();
        private static readonly Regex TokenPattern = new Regex(@"__[^_]+__", RegexOptions.Compiled);
        public static string GenerateRandomString(HashSet<string> generated)
        {
            string result;
            do
            {
                var stringChars = new char[6];
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = _chars[_rand.Next(_chars.Length)];
                }
                result = new string(stringChars);
            } while (generated.Contains(result));
            generated.Add(result);
            return result;
        }

        public static List<string>? FindTokens(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var result = new List<string>();

            var matches = TokenPattern.Matches(input);
            foreach (Match m in matches)
                result.Add(m.Value);
            return result;
        }

        public static string ConvertShellcodeToIntVariable(byte[]? shellcode, int key_to_divide)
        {
            if (shellcode == null) return "";
            int[] output = new int[shellcode.Length];
            for (int i = 0; i < shellcode.Length; i++)
            {
                output[i] = shellcode[i] * key_to_divide;
            }
            string result = string.Join(",", output);
            return result;
        }
    }
}
