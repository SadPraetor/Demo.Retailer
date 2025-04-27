using Elastic.Channels;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Serilog.Enrichers.CallerInfo;

namespace Demo.Retailer.Tests.Demo.Retailer.Tests
{
	public class LoggingTests
	{
		[Fact]
		public void LogMessage()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.Enrich.FromLogContext()		
				

				.WriteTo.Elasticsearch(new[] {new Uri("http://localhost:9200") }, opts =>
				{
					
					opts.DataStream = new DataStreamName("logs", "unit-test", "default");
					opts.BootstrapMethod = BootstrapMethod.Failure;
					opts.ConfigureChannel = channelOpts =>
					{
						channelOpts.BufferOptions = new BufferOptions
						{
							ExportMaxConcurrency = 10
						};
					};
					
				} )
				.Enrich.WithProperty("Application", "Demo.Retailer")
				//.Enrich.WithProperty("Application.Scope","unit-test")
				.Enrich.WithEnvironmentName()
				.Enrich.WithProcessName()
				.Enrich.WithCallerInfo(true, "Demo.Retailer." ,filePathDepth:3)
				.CreateLogger();

			Log.Logger.Information("This is unit test {random}", Random.Shared.Next(1, 10));
			//Log.Logger.Warning("There should be a message");
			Log.CloseAndFlush();
		}

	}
}
