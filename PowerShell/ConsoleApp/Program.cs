using System.Text.RegularExpressions;

namespace ConsoleApp
{
    public class GenerateCode
    {
        private static readonly string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static readonly Random _rand = new();
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


    internal class Program
    {
        static  async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: ConsoleApp.exe <template> <binary> <output>");
                return;
            }

            var template = await File.ReadAllTextAsync(args[0]);
            var dll = await File.ReadAllBytesAsync(args[1]);

            var randomKey = GenerateCode._rand.Next(100, 200);
            template = template.Replace("__KEY_PLACEHOLDER__", randomKey.ToString());
            var shellcode = GenerateCode.ConvertShellcodeToIntVariable(dll, randomKey);
            template = template.Replace("__SHELLCODE_PLACEHOLDER__", shellcode);

            var tokens = GenerateCode.FindTokens(template);
            HashSet<string> generated = new();
            foreach (var t in tokens)
            {
                template = template.Replace(t, GenerateCode.GenerateRandomString(generated));
            }

            await File.WriteAllTextAsync(args[2], template);

        }
    }
}
