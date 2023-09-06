using Grpc.Core;
using NATS.Client;
using NATS.Client.JetStream;
using System.Text;

namespace chat_server_grpc.Services
{
    public class ChatService: Chat.ChatBase
    {
        private readonly ILogger<ChatService> _logger;
        public ChatService(ILogger<ChatService> logger) => _logger = logger;


        public override async Task Chatting(IAsyncStreamReader<ChatRequest> requestStream, IServerStreamWriter<ChatResponse> responseStream, ServerCallContext context)
        {
            try
            {
                CancellationToken contextCancellationToken = context.CancellationToken;
                var httpContext = context.GetHttpContext();
                _logger.LogInformation($"Connection id: {httpContext.Connection.Id}");

                string roomName = "asdf";

                // nats 메시지를 받는 이벤트 핸들러
                EventHandler<MsgHandlerEventArgs> receiveEventHandler = (sender, args) =>
                {
                    Console.WriteLine($"worker received {args.Message}");
                    responseStream.WriteAsync(new ChatResponse
                    {
                        Message = Encoding.UTF8.GetString(args.Message.Data),
                    });
                };



                var asdf = Task.Run(() =>
                {
                    while (true)
                    {
                        Console.WriteLine(context.GetHttpContext());
                        Thread.Sleep(1000);
                    }
                });

                using IAsyncSubscription s = Globals.natsConnector.SubscribeAsync(roomName, receiveEventHandler);

                await foreach (var result in requestStream.ReadAllAsync(cancellationToken: contextCancellationToken).ConfigureAwait(false))
                {

                    Console.WriteLine(result);
                    Globals.natsConnector.Publish(roomName, Encoding.UTF8.GetBytes(result.Message));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }
    }
}
