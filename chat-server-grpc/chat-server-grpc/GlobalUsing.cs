global using chat_server_grpc.Common;

namespace chat_server_grpc
{
    static class Globals
    {
        public static NATS.Client.IConnection natsConnector = new Nats().Connection;
    }
}
