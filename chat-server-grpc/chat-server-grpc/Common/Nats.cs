using NATS.Client;

namespace chat_server_grpc.Common
{
    class Nats
    {
        private IConnection connection;

        public Nats()
        {
            try
            {
                ConnectionFactory cf = new ConnectionFactory();
                Options opts = ConnectionFactory.GetDefaultOptions();

                opts.Url = "nats://localhost:4222";

                connection = cf.CreateConnection(opts);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                throw;
            }
            
        }

        public IConnection Connection
        {
            get { return connection; }
        }
    }
}