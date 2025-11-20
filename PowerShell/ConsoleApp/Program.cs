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
    }


    internal class Program
    {
        static  async Task Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: ConsoleApp.exe <template> <binary> <newton> <output>");
                return;
            }

            var template = await File.ReadAllTextAsync(args[0]);
            var dll = await File.ReadAllBytesAsync(args[1]);
            var newton = await File.ReadAllBytesAsync(args[2]);

            template = template.Replace("__PAYLOAD_B64__", Convert.ToBase64String(dll));
            template = template.Replace("__DLL_NEWTONSOFT_B64__", Convert.ToBase64String(newton));

            var tokens = GenerateCode.FindTokens(template);
            if (tokens == null) Environment.Exit(1);
            HashSet<string> generated = [];
            foreach (var t in tokens)
            {
                template = template.Replace(t, GenerateCode.GenerateRandomString(generated));
            }

            await File.WriteAllTextAsync(args[3], template);

        }
    }
}
