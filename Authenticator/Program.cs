using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AuthenticationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var rsa = RSA.Create();
            //byte[] privkey = rsa.ExportRSAPrivateKey();
            //byte[] pubkey = rsa.ExportRSAPublicKey();


            //byte[] refreshKey = new byte[256];
            //Random rng = new Random();
            //rng.NextBytes(refreshKey);

            //Console.WriteLine(Convert.ToBase64String(privkey));
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(Convert.ToBase64String(pubkey));
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(Convert.ToBase64String(refreshKey));


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
