using Application.CryptoFacilBrasil.ModelsRequest;
using CryptoFacilBrasil.Domain.Models;
using CryptoFacilBrasil.Domain.Models.Enums;
using CryptoFacilBrasil.Services.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace Application.CryptoFacilBrasil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService, ITelegramBotClient telegramBotClient)
        {
            _chatService = chatService;
        }

        // POST: api/chat
        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] Chat chat)
        {
            if (chat == null)
            {
                return BadRequest("Chat cannot be null.");
            }

            try
            {
                await _chatService.CreateChat(chat);
                return CreatedAtAction(nameof(GetChatById), new { chatId = chat.IdChat }, chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/chat/{chatId}
        [HttpGet("{chatId:long}")]
        public async Task<IActionResult> GetChatById(long chatId)
        {
            var chat = await _chatService.GetChat(chatId);
            if (chat == null)
            {
                return NotFound($"Chat with ID {chatId} not found.");
            }
            return Ok(chat);
        }

      

        // Get: api/chat/{chatId}/order/inprogress
        [HttpGet("{chatId:long}/order/inprogress")]
        public async Task<IActionResult> LastOrderDetail(long chatId)
        {          
            try
            {
                var order = await _chatService.GetLastOrderDetail(chatId);
                if (order == null) return NotFound();
                if (order.StatusOrder == EnumStatusOrder.PedingTerms || order.StatusOrder == EnumStatusOrder.Pending)
                    return Ok(order);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
      
    }
}
