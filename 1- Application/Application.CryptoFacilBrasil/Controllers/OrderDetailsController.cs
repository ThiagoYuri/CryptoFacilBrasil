using Application.CryptoFacilBrasil.ModelsRequest;
using CryptoFacilBrasil.Domain.Models;
using CryptoFacilBrasil.Domain.Models.Enums;
using CryptoFacilBrasil.Services.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.CryptoFacilBrasil.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        //Método que retorna os detalhes das ordens
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(string id)
        {
            // Busca a ordem pelo ID
            var order = await _orderDetailService.GetOrderDetail(Guid.Parse(id));
            return Ok(order);
        }



        // Método para adicionar uma nova ordem
        [HttpPost]
        public async Task<ActionResult> AddOrderDetails([FromBody] CreateOrderDetailRequest newOrder)
        {
            if (newOrder == null)
            {
                return BadRequest("Dados da ordem inválidos.");
            }
            var order = new OrderDetail()
            {
                MethodPay = newOrder.MethodPay,
                Network = newOrder.Network,
                TypeTransferNetwork = newOrder.TypeTransferNetwork,
                StatusOrder = EnumStatusOrder.None,
                SellOrBuy = newOrder.SellOrBuy,
                Value = newOrder.Value
            };
            // Adiciona a nova ordem à lista
            await _orderDetailService.CreateOrderDetail(order);

            return CreatedAtAction(nameof(order), new { id = order.Id }, order);           
        }


        [HttpPut]
        public async Task<ActionResult> UpdateOrderDetails([FromBody] UpdateOrderDetailRequest updateOrder)
        {
            if (updateOrder == null)
            {
                return BadRequest("Dados da ordem inválidos.");
            }
            var order = new OrderDetail()
            {
                Id = updateOrder.Id,
                ChatId = updateOrder.ChatId,
                MethodPay = updateOrder.MethodPay,
                Network = updateOrder.Network,
                TypeTransferNetwork = updateOrder.TypeTransferNetwork,
                StatusOrder =  updateOrder.StatusOrder,
                SellOrBuy = updateOrder.SellOrBuy,
                Value = updateOrder.Value
            };
            // Update a ordem
            await _orderDetailService.UpdateOrderDetail(order);

            return Ok();
        }

        // Método para finalizar uma ordem
        //[HttpDelete("{id}")]
        //public ActionResult RemoveOrder(string id)
        //{
        //    // Busca a ordem pela ID
        //    var orderToRemove = StaticValues.orderDetailsList.FirstOrDefault(o => o.Id == id);

        //    if (orderToRemove == null)
        //    {
        //        return NotFound($"Ordem com ID {id} não encontrada.");
        //    }

        //    // Remove a ordem da lista
        //    StaticValues.orderDetailsList.Remove(orderToRemove);
        //    return NoContent(); // Retorna 204 No Content após a remoção
        //}


    }
}
