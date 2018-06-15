using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusSlave
{
    class Program
    {
        static void Main(string[] args)
        {
            ModbusService server = new ModbusService("127.0.0.1", 502);
            server.Start();
            server.Run();
            Console.Read();
        }
    }
}
