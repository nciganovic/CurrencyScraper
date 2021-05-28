using System;
using System.IO;
using System.Net;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://srh.bankofchina.com/search/whpj/searchen.jsp");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            //ReadResponseStream(readStream);

            ReadStramLines(readStream);

            // Releases the resources of the response.
            response.Close();
            // Releases the resources of the Stream.
            readStream.Close();
            Console.WriteLine("Hello World!");
        }

        private static void ReadResponseStream(StreamReader readStream) {
            Console.WriteLine("\r\nResponse stream received.");
            Char[] read = new Char[256];
            // Reads 256 characters at a time.
            int count = readStream.Read(read, 0, 256);
            Console.WriteLine("HTML...\r\n");
            while (count > 0)
            {
                // Dumps the 256 characters on a string and displays the string to the console.
                String str = new String(read, 0, count);
                Console.Write(str);
                count = readStream.Read(read, 0, 256);
            }
            Console.WriteLine("");
        }

        private static void ReadStramLines(StreamReader readStram) {
            bool reading = true;
            while (reading) {
                string line = readStram.ReadLine();
                Console.Write(line + "\n");
                if (line == null) {
                    reading = false;
                }
            }

        }
    }
}
