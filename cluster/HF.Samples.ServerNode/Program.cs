using System;
using System.Reflection;
using System.Threading;
using Autofac;
using CommandLine;
using Serilog;
using Hangfire;
using Hangfire.Samples.Framework;
using Hangfire.Samples.Framework.Logging;
using HF.Samples.GoodsService;
using HF.Samples.OrderService;
using HF.Samples.StorageService;


namespace HF.Samples.ServerNode
{
	public class Program
	{
		private static ILog _logger = LogProvider.For<Program>();

		private static ManualResetEvent _exit = null;

		private static BackgroundJobServer _backgroundJobServer = null;

		public static void Main(string[] args)
		{
            // 从Hangfire 1.3.0开始，Hangfire引入了日志组件LibLog,
            // 所以应用不需要做任何改动就可以兼容如下日志组件：Serilog、NLog、Log4Net、EntLib Logging、Loupe、Elmah
            // 配置 serilog如下，LibLog组件会自动发现并使用serilog
            Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.LiterateConsole()
				.WriteTo.RollingFile("logs\\log-{Date}.txt")
				.CreateLogger();

			Parser.Default.ParseArguments<NodeOptions>(args)
				.WithNotParsed(_ => _logger.Info("Arguments configuration is empty, you can view command line by type: --help"))
				.WithParsed(opts =>
				{
					_logger.InfoFormat("Accepted args: {@opts}", opts);

					_exit = new ManualResetEvent(false);

					Console.CancelKeyPress += (sender, e) =>
					{
						if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
						{
							_logger.Error("Control+Break detected, terminating service (not cleanly, use Control+C to exit cleanly)");
							return;
						}

						e.Cancel = true;

						_exit.Set();
					};

					//start hangfire server here
					UseHangfireServer(opts);
				});

			_exit?.WaitOne();

			_backgroundJobServer?.Dispose();
		}

		private static void UseHangfireServer(NodeOptions opts)
		{
			var options = new BackgroundJobServerOptions
			{
				ServerName = opts.Identifier,
				WorkerCount = opts.WorkerCount,
				Queues = opts.Queues.Split(',')
			};

			UseAutofac();

			GlobalConfiguration.Configuration.UseRedisStorage();

			_backgroundJobServer = new BackgroundJobServer(options);

			_logger.InfoFormat("The hangfire server {0} [queues: {1}, workercount: {2}] is now running, press Control+C to exit.", opts.Identifier, opts.Queues, opts.WorkerCount);
		}

		private static void UseAutofac()
		{
			var builder = new ContainerBuilder();

			builder.RegisterModule(new DelegateModule(() => new Assembly[]
			{
				typeof(IProductService).GetTypeInfo().Assembly,
				typeof(IOrderService).GetTypeInfo().Assembly,
				typeof(IInventoryService).GetTypeInfo().Assembly
			}));

			var container = builder.Build();

			GlobalConfiguration.Configuration.UseAutofacActivator(container);
		}
	}
}
