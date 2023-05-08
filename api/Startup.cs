using System;
using Azure.Identity;
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
            string AppConfigConnString = Environment.GetEnvironmentVariable("AzureConnectionString");
            string keyVault = Environment.GetEnvironmentVariable("keyVaultName");
            string keyVaultURI = string.Format("https://{0}.vault.azure.net/", keyVault);

            builder.ConfigurationBuilder
                          .AddAzureAppConfiguration(AppConfigConnString)
                          .AddEnvironmentVariables()
                          .AddAzureKeyVault(new Uri(keyVaultURI), new DefaultAzureCredential())
                          .Build();
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