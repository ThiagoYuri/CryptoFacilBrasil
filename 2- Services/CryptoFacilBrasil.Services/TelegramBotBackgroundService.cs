using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using CryptoFacilBrasil.Domain.Models;
using CryptoFacilBrasil.Domain.Settings;
using CryptoFacilBrasil.Services.AuthBot;
using CryptoFacilBrasil.Services.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace CryptoFacilBrasil.Services
{
    public class TelegramBotBackgroundService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly RequestManagement _requestManagement;
        // Fila de atendimento e dicionário para status de aceitação de termos

        public TelegramBotBackgroundService(ITelegramBotClient botClient, RequestManagement requestManagement)
        {
            _botClient = botClient;
            _requestManagement = requestManagement;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {

                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } // Captura apenas mensagens e callbacks
                    };

                    _botClient.StartReceiving(
                        HandleUpdateAsync,
                        HandleErrorAsync,
                        receiverOptions: receiverOptions,
                        cancellationToken: stoppingToken
                    );

                    Console.WriteLine("Telegram Bot Service started.");
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error encountered: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Delay para tentar reconectar
                }
            }
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine($"Bot error: {errorMessage}");

            // Tenta reiniciar a conexão em caso de falha
            await Task.Delay(5000, cancellationToken);
            _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: cancellationToken);
        }

        // Atualize o método HandleUpdateAsync para lidar com o callback dos botões
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
                {
                    await HandleCallbackQueryAsync(botClient, update, cancellationToken);
                }
                else if (update.Type == UpdateType.Message && update.Message.Text != null)
                {
                    if (update.Message != null)
                    {
                        if (update.Message.Photo != null && update.Message.Photo.Length > 0)
                        {
                            // Se a mensagem contém uma foto
                            var photo = update.Message.Photo[^1]; // Pega a imagem de maior resolução
                            await DownloadFileAsync(photo.FileId, "photo.jpg");
                        }
                        else if (update.Message.Document != null)
                        {
                            // Se a mensagem contém um documento (PDF, DOC, etc.)
                            var document = update.Message.Document;
                            await DownloadFileAsync(document.FileId, document.FileName);
                        }
                    }

                    if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
                    {
                        var message = update.Message;
                        var chatId = message.Chat.Id;
                        // Verifica se o atendimento é manual para o `chatId` atual
                        var chat = await _requestManagement.GetChatByIdAsync(chatId);

                        //if (chat != null && chat.IsManual)
                        //{
                        //    await _requestManagement.CreateChatAsync(new Domain.Models.Chat()
                        //    {
                        //        IdChat = chatId,
                        //        IsManual = false
                        //    });
                        //    // Atendimento manual ativo, o bot não responde
                        //    return;
                        //}

                        //Create chat if not exists
                        if (chat == null)
                        {
                            await _requestManagement.CreateChatAsync(new Domain.Models.Chat()
                            {
                                IdChat = chatId
                            });
                            chat = await _requestManagement.GetChatByIdAsync(chatId);
                        }

                        if (chat != null)
                        {
                            var order = await _requestManagement.GetLastOrderDetailByIdChatAsync(chat.IdChat);
                            if (order == null)
                                if (message.Text.Trim().ToLower().StartsWith("id:"))
                                {
                                    // Pega o texto após "id:" e remove espaços em branco
                                    var orderId = message.Text.Substring(3).Trim();
                                    if (string.IsNullOrEmpty(orderId)) throw new Exception("Id is null");
                                    var orderDetail = await _requestManagement.GetOrderById(orderId);
                                    if (orderDetail == null)
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "🚫 Não foi possível obter os detalhes da ordem. Por favor, verifique o ID e tente novamente.",
                                            cancellationToken: cancellationToken
                                        );
                                        return; // Retorna para não continuar o processamento
                                    }
                                    await SaveAndSendMessageChat(chatId, "👋 Atualizamos os termos de uso do modelo CryptoFacilBrasil. Para prosseguir com a ordem aceite os termos.\n\nLer os termos: https://CryptoFacilBrasil.com/terms");

                                    await SendTermsConfirmationAsync(botClient, chatId, cancellationToken, orderDetail);
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(
                                       chatId: chatId,
                                       text: "Apenas cole o código copiado automaticamente, para iniciar o atendimento.",
                                       cancellationToken: cancellationToken
                                   );
                                }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.CallbackQuery is not null && update.CallbackQuery.Message is not null)
                    if (update.CallbackQuery.Data == "confirm_yes")
                    {
                        var chatId = update.CallbackQuery.Message.Chat.Id;
                        var order = await _requestManagement.GetLastOrderDetailByIdChatAsync(chatId);
                        if (order != null)
                        {
                            order.StatusOrder = Domain.Models.Enums.EnumStatusOrder.Pending;
                            await _requestManagement.UpdateOrderChatAsync(order);

                            await SaveAndSendMessageChat(chatId, "Obrigado por aceitar os termos!");
                            await SaveAndSendMessageChat(chatId, $"ID: {order.Id} \nMetodo de pagamento: {order.MethodPay} \nMoeda: {order.Network} \nRede: {order.TypeTransferNetwork} \nComprar/ou/Vender: {order.SellOrBuy}\nValor: {order.Value}R$");
                            await SaveAndSendMessageChat(chatId, "Logo você será atendido");

                        }
                    }
                    else if (update.CallbackQuery.Data == "confirm_no")
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            text: "Você não aceitou os termos.",
                            cancellationToken: cancellationToken
                        );

                        // Lógica para tratar a não aceitação dos termos, se necessário
                    }

                // Confirma o processamento da CallbackQuery no Telegram
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex}");
            }


        }


        async Task SendTermsConfirmationAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken, OrderDetail orderDetails)
        {

            orderDetails.StatusOrder = Domain.Models.Enums.EnumStatusOrder.PedingTerms;
            orderDetails.ChatId = chatId;
            await _requestManagement.UpdateOrderChatAsync(orderDetails);


            var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Sim", "confirm_yes"),
                    InlineKeyboardButton.WithCallbackData("Não", "confirm_no")
                }
            });
            await botClient.SendTextMessageAsync(
                                     chatId: chatId,
                                     text: "Você aceitou os termos?",
                                     cancellationToken: cancellationToken,
                                     replyMarkup: inlineKeyboard
                                 );

        }

        async Task SaveAndSendMessageChat(long chatId, string text)
        {
            await _requestManagement.SaveMessageAsync(new
            {
                ChatId = chatId,
                Message = text
            });
        }

        async Task DownloadFileAsync(string fileId, string fileName)
        {
            // Obtém o arquivo pelo ID
            var file = await _botClient.GetFileAsync(fileId);

            // Caminho para salvar o arquivo localmente
            var filePath = Path.Combine("downloads", fileName);

            // Baixa o arquivo usando o caminho da URL do Telegram
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await _botClient.DownloadFileAsync(file.FilePath, fileStream);
        }
    }



    // Método para obter um chat pelo ID
    public class RequestManagement
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        // Injeção de dependências no construtor
        public RequestManagement(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;  // Obtém as configurações da API
        }

        

        public async Task<Domain.Models.Chat> GetChatByIdAsync(long chatId)
        {            
            // Use the dynamic base URL
            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/api/chat/{chatId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string to the Chat object
                var chat = System.Text.Json.JsonSerializer.Deserialize<Domain.Models.Chat>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Optional: Makes the property matching case insensitive
                });

                return chat;
            }

            return null; // Or handle the error as needed
        }

        public async Task CreateChatAsync(Domain.Models.Chat chat)
        {

            // Serializa o objeto Chat para JSON
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(chat),
                Encoding.UTF8,
                "application/json"
            );

            // Envia a solicitação POST para criar um novo chat
            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/api/chat", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Desserialize o JSON em um objeto Chat
                var createdChat = System.Text.Json.JsonSerializer.Deserialize<Domain.Models.Chat>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Opcional: torna a correspondência de propriedades insensível a maiúsculas e minúsculas
                });
            }
        }


        public async Task<OrderDetail> GetOrderById(string orderId)
        {
            // Use the dynamic base URL
            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/api/OrderDetails/{orderId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string to the Chat object
                var order = System.Text.Json.JsonSerializer.Deserialize<Domain.Models.OrderDetail>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Optional: Makes the property matching case insensitive
                });

                return order;
            }

            return null; // Or handle the error as needed

        }


        public async Task<OrderDetail> GetLastOrderDetailByIdChatAsync(long IdChat)
        {
            // Use the dynamic base URL
            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/api/chat/{IdChat}/order/inprogress");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string to the Chat object
                var order = System.Text.Json.JsonSerializer.Deserialize<Domain.Models.OrderDetail>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Optional: Makes the property matching case insensitive
                });

                return order;
            }

            return null; // Or handle the error as needed

        }

        public async Task<OrderDetail> UpdateOrderChatAsync(OrderDetail orderDetails)
        {
            var content = JsonContent.Create(orderDetails);

            // Envia a requisição PUT com o corpo
            var response = await _httpClient.PutAsync($"{_apiSettings.BaseUrl}/api/OrderDetails", content);

            // Processa a resposta
            if (response.IsSuccessStatusCode)
            {
                return orderDetails;
            }
            else
            {
                // Trate o erro, caso a resposta não seja bem-sucedida
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao atualizar OrderDetails: {errorResponse}");
            }

            return null; // Or handle the error as needed

        }


        public async Task SaveMessageAsync(object sendTelegramManualMessageRequest)
        {

            // Serializa o objeto Chat para JSON
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(sendTelegramManualMessageRequest),
                Encoding.UTF8,
                "application/json"
            );

            // Envia a solicitação POST para criar um novo chat
            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/api/message/send-telegram-message", jsonContent);

            if (!response.IsSuccessStatusCode)
            {

            }
        }


    }


}
