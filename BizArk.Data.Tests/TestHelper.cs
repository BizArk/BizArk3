using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BizArk.Data.Tests
{
	public static class TestHelper
	{

		public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
		{
			return new ConfigurationBuilder()
				.SetBasePath(outputPath)
				.AddJsonFile("appsettings.json", optional: true)
				.AddEnvironmentVariables()
				.Build();
		}

		public static BizArkDataConfiguration GetApplicationConfiguration(string outputPath)
		{
			var cfg = new BizArkDataConfiguration();

			var icfg = GetIConfigurationRoot(outputPath);

			icfg
				.GetSection("BizArkData")
				.Bind(cfg);

			return cfg;
		}

	}
}
