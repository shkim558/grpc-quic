using System.Net;
using Grpc.Core;
using Grpc.Net.Client;
using gRPC_HTTP3_Client;

var httpHandler = new HttpHandler(new HttpClientHandler());

var channel = GrpcChannel.ForAddress("https://localhost:7107", new GrpcChannelOptions { HttpHandler = httpHandler });
var client = new Chat.ChatClient(channel);
using var streaming = client.Chatting();

var response = Task.Run(async () =>
{
    await foreach (var response in streaming.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"{response.Message}: {response.Message}");

    }
});

var line = Console.ReadLine();

while (!string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase))
{
    await streaming.RequestStream.WriteAsync(new ChatRequest
    {
        Message = line,
    });
    line = Console.ReadLine();
}

await streaming.RequestStream.CompleteAsync();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

public class HttpHandler : DelegatingHandler
{
    public HttpHandler() { }
    public HttpHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cts)
    {
        request.Version = HttpVersion.Version30;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        return base.SendAsync(request, cts);
    }
}