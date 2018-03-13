using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace HF.Samples.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                // 陈烈
                // http://blog.csdn.net/yzj_xiaoyue/article/details/79157420
                .UseUrls("http://localhost:9000")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
