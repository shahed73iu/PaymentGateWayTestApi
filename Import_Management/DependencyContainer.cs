using MediatR;
using Microsoft.CodeAnalysis.Host;
using Microsoft.Extensions.DependencyInjection;


namespace Import_Management
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
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
