using Application.CryptoFacilBrasil.ModelsRequest;
using CryptoFacilBrasil.Domain.Models;
using CryptoFacilBrasil.Services;
using CryptoFacilBrasil.Services.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace Application.CryptoFacilBrasil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ITelegramBotClient _telegramBotClient;
        public MessageController(IMessageService messageService, ITelegramBotClient telegramBotClient)
        {
            _messageService = messageService;
            _telegramBotClient = telegramBotClient;
        }

        // POST: api/message/messagesClient
        [HttpPost("messagesClient")]
        public async Task<IActionResult> InsertMessage([FromBody]CreateMessageClient messageClient)
        {
            try
            {
                await _messageService.CreateMessage(new Message()
                {
                    ChatId = messageClient.ChatId,
                    TextValue = messageClient.TextValue,
                    CreateAt = messageClient.CreateAt,
                    IdMessageTelegram = messageClient.IdMessageTelegram,
                    IsBot = false
                }) ;
                return Ok("Message inserted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("send-telegram-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendTelegramManualMessageRequest request)
        {
            if (request == null || request.ChatId <= 0 || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Dados inválidos.");
            }

            try
            {
                // Enviar mensagem para o chat especificado
                var sentMessage = await _telegramBotClient.SendTextMessageAsync(
                    chatId: request.ChatId,
                    text: request.Message
                );

                await _messageService.CreateMessage(new Message()
                {
                    ChatId = request.ChatId,
                    TextValue = request.Message,
                    IdMessageTelegram = sentMessage.MessageId,
                    IsBot = true
                });

                return Ok("Mensagem enviada com sucesso.");
            }
            catch (Exception ex)
            {
                // Log o erro (opcional)
                Console.WriteLine(ex);
                return StatusCode(500, "Erro ao enviar a mensagem.");
            }
        }
    }
}
