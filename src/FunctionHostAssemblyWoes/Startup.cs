[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(FunctionHostAssemblyWoes.Startup))]

namespace FunctionHostAssemblyWoes
{
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup : FunctionsStartup
    {
        /// <inheritdoc/>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(builder.Services);
        }
    }
}
