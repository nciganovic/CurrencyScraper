using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        private static List<string> currencies = new List<string>();

        static void Main(string[] args)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://srh.bankofchina.com/search/whpj/searchen.jsp");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            //ReadResponseStream(readStream);
            FindCurrenciesInStream(readStream);

            foreach (string c in currencies) 
            { 
                
            }

            // Releases the resources of the response.
            response.Close();
            // Releases the resources of the Stream.
            readStream.Close();
            Console.WriteLine("Hello World!");
        }

        private static void FindCurrenciesInStream(StreamReader readStream) {
            
            while (true) 
            {
                string line = readStream.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.Contains("<option value=\"")) {
                    int startIndex = line.IndexOf("\"");
                    string currency = line.Substring(startIndex + 1, 3);
                    currencies.Add(currency);
                }

                Console.Write(line + "\n");
                
            }

            currencies.RemoveAt(0);
        }

    }
}
