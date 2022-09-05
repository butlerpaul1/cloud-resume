using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]

namespace FunctionApp
{
  /// <summary>
  /// Implement the functions startup abstract class
  /// </summary>
  class Startup : FunctionsStartup
  {
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
      string cs = Environment.GetEnvironmentVariable("AzureConnectionString");
      builder.ConfigurationBuilder.AddAzureAppConfiguration(cs);
    }

    /// <summary>
    /// Overwrite the configure app configuration method with the azure app configuration provider
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(IFunctionsHostBuilder builder)
    {
    }
  }
}