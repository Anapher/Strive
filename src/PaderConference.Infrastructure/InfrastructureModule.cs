using Autofac;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Auth;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Interfaces;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Services.Media;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance();
            builder.RegisterType<JwtHandler>().As<IJwtHandler>().SingleInstance();
            builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
            builder.RegisterType<JwtValidator>().As<IJwtValidator>().SingleInstance();

            builder.RegisterType<ConferenceManager>().As<IConferenceManager>().SingleInstance();
            builder.RegisterType<ConnectionMapping>().As<IConnectionMapping>().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IRepository<>))
                .AsImplementedInterfaces();

            builder.RegisterType<RtcMediaConnectionFactory>().As<IRtcMediaConnectionFactory>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IConferenceServiceManager<>))
                .AsImplementedInterfaces().SingleInstance();
        }
    }
}