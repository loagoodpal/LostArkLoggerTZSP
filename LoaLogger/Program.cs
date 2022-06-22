using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
public struct Received
{
    public byte[] message;
    public IPEndPoint Sender;
}

namespace LoaLogger
{
    abstract class UdpBase
    {
        protected UdpClient Client;

        protected UdpBase()
        {
            Client = new UdpClient();
        }

        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received()
            {
                message = result.Buffer,
                Sender = result.RemoteEndPoint
            };
        }
    }

    //Server
    class UdpListener : LoaLogger.UdpBase
    {
        private IPEndPoint _listenOn;

        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 37002))
        {
        }

        public UdpListener(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }
      }
    class Program
    {
        static void Main(string[] args)
        {
            Oodle.Init();
            Parser p = new Parser();
            p.init();
            Console.WriteLine("Starting logging on port 37002");
            var server = new UdpListener();
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var recieved = await server.Receive();
                    p.Device_OnPacketArrival_pcap(recieved.message);
                };
            });
            Console.ReadKey();
        }
    }
}
