using PaymentGateway.Api.Models.AppSettings;

namespace PaymentGateway.Api.Installers
{
    public static class BankInstaller
    {
        public static IServiceCollection InstallBank(this IServiceCollection services, IConfiguration configuration)
        {
            var bank = new BankConfiguration();
            configuration.GetSection("AppConfiguration:Bank").Bind(bank);

            services.AddSingleton(bank);
            return services;
        }
    }
}
