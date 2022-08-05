using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DuckDNS_Updtr
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #if (DEBUG)
                Console.Write("Debug Mode - ");
                args = new string[] { "duck-dns-token-goes-here", "subdomainname", "loop", "10" };
            #endif

            if (args is null || args.Length < 2)
            {
                Console.WriteLine("Arguments missing! Please inform required arguments for execution in order: \"token\" \"username\"");
                Console.WriteLine("\nShould the application run indefinitely, after \"username\" inform the argument \"loop\" and");
                Console.WriteLine("\nthe amount of time it should run, like every \"10\" minutes.");
                Environment.Exit(0);
            }

            Console.WriteLine("DuckDNS IP Updater");

            await UpdateDuckDNS(args[0], args[1]);
            
            if(args[2] is not null)
            {
                while(true)
                {
                    Console.WriteLine("\nWaiting for the next cycle in " + args[3] + " minutes...\n");
                    System.Threading.Thread.Sleep(int.Parse(args[3]) * 60 * 1000);
                    await UpdateDuckDNS(args[0], args[1]);
                }
            } else
            {
                Environment.Exit(0);
            }
        }

        static async System.Threading.Tasks.Task UpdateDuckDNS(string token, string domain)
        {
            try
            {
                var httpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() { UseDefaultCredentials = true });

                var ip = await GetIP();
                if(ip is null)
                {
                    throw new ArgumentException("Error: IP ca");
                }

                Uri url = new("https://www.duckdns.org/update?domains=" + domain + "&token=" + token + "&ip=" + ip);

                Console.WriteLine("Sending request to DuckDNS...");

                var response = await httpClient.GetAsync(url);

                if (response.StatusCode.ToString().Contains("OK"))
                {
                    Console.WriteLine("Success! Your IP was updated on DuckDNS!");
                }
                else
                {
                    Console.WriteLine("Error - StatusCode: " + response.StatusCode.ToString() 
                        + "\nError updating your IP!  Try checking your internet connection.");
                }

            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
                Console.WriteLine("\nError updating your IP! Try checking your internet connection.");
            }

        }

        static async System.Threading.Tasks.Task<string> GetIP()
        {
            Console.WriteLine("\nFetching your public IP address...");

            try
            {
                Uri url = new("http://checkip.amazonaws.com");

                var httpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() { UseDefaultCredentials = true });
                var response = await httpClient.GetAsync(url);
                var publicIP = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Your public IP is: " + publicIP);
                return publicIP;
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
                Console.WriteLine("Error. Couldn't fetch your public IP address." +
                    "\nTry checking your internet connection.");
                return null;
            }
        }
    }
}
