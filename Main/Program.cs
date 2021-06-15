using Chickadee;
using System;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace Main
{
    class Program
    {

        public static Logger logger = LogManager.GetCurrentClassLogger();
        static async Task Main(string[] args)
        {
            Parser parser = new Parser();
            await parser.ComputeUaChanges();

            Console.WriteLine();
            Console.WriteLine("We're done here.");
            // Console.ReadLine();
        }
    }
}
