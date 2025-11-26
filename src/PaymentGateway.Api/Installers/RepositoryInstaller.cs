using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.AppSettings;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Installers
{
    public static class RepositoryInstaller
    {
        public static IServiceCollection InstallRepository(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentsRepository, PaymentsRepository>(option =>
            {
                var sanitisePaymentDetails = option.GetRequiredService<ISanitisePaymentDetails>();
                var bankConfiguration = option.GetRequiredService<BankConfiguration>();
                var httpClient = new HttpClient();
                return new PaymentsRepository(sanitisePaymentDetails, bankConfiguration, httpClient);
            });

            services.AddSingleton<ISanitisePaymentDetails, SanitisePaymentDetails>();

            return services;
        }
    }
}
