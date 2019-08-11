using System;
using System.IO;
using System.Net;

namespace DuckDNS_Updtr
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = "YOUR_DUCK_DNS_TOKEN"; //Your DuckDNS token, present on your profile
            string domain = "YOURDOMAIN"; //Your domain name, eg "myhome" instead of "myhome.duckdns.org"

            Console.WriteLine("DuckDNS IP Updater");

            updateDuckDNS(domain, token);
            Environment.Exit(0);
        }

        static void updateDuckDNS(string domain, string token)
        {
            try
            {
                Uri url = new Uri("https://www.duckdns.org/update?domains=" + domain + "&token=" + token + "&ip=" + getIP());

                Console.WriteLine("Sending request to DuckDNS...");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode.ToString().Contains("OK"))
                {
                    Console.WriteLine("Success! Your IP was updated on DuckDNS!");
                }
                else
                {
                    Console.WriteLine("Error - StatusCode: " + response.StatusCode.ToString() + "\r\n" +
                                      "Try running the application again or checking your internet connection.");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error! Try running the application again or checking your internet connection.");
                Environment.Exit(1);
            }

        }

        static string getIP()
        {

            string publicIP;

            Console.WriteLine("\r\nFetching your public IP address...");

            try
            {
                Uri url = new Uri("http://checkip.amazonaws.com");
                
                using (WebClient client = new WebClient())
                {
                    publicIP = client.DownloadString(url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                publicIP = "Error. Couldn't fetch your public IP address. \r\n" +
                           "Try running the application again or checking your internet connection.";
            }

            if (publicIP.Contains("Error"))
            {
                Console.WriteLine(publicIP);
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Your public IP is: " + publicIP);
            }

            return publicIP;
        }
    }
}
