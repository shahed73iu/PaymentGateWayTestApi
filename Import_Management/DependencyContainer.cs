using Domain.Core.Bus;
using MediatR;
using MicroRabbit.Infra.Bus;
using Microsoft.CodeAnalysis.Host;
using Microsoft.Extensions.DependencyInjection;


namespace Import_Management
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //Domain Bus
            services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMQBus(sp.GetService<IMediator>(), scopeFactory);
            });



            //Subscriptions
            //services.AddTransient<EditUserInformationEventHandler>();

            //Domain Events
            //services.AddTransient<IEventHandler<EditPasswordEvent>, EditPasswordEventHandler>();

            //Domain Banking Commands
            //services.AddTransient<IRequestHandler<EditRoleManagerCommand, bool>, EditRoleManagerCommandHandler>();



            //data          

            //services.AddTransient<IVatItemCategory, VatItemCategory>();


        }
    }
}
