using PaymentGateway.Api.Middleware;


namespace PaymentGateway.Api.Installers
{
    public static class MiddlewareInstaller
    {
        public static void InstallMiddleware(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateExpiryMiddleware>();                
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
        }
    }
}
