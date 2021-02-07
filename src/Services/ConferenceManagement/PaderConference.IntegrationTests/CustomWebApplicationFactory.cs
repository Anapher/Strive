//using System;
//using System.Collections.Generic;
//using System.Threading;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Mongo2Go;
//using Moq;
//using PaderConference.Core.Interfaces.Gateways.Repositories;
//using PaderConference.Core.Interfaces.Services;
//using PaderConference.Infrastructure.Auth.AuthService;
//using PaderConference.Infrastructure.Data;
//using PaderConference.IntegrationTests._Helpers;

//namespace PaderConference.IntegrationTests
//{
//    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
//    {
//        private MongoDbRunner? _runner;
//        private readonly UserCredentialsOptions _options = new() {Users = new Dictionary<string, OptionsUserData>()};
//        private int _idCounter;

//        protected override void Dispose(bool disposing)
//        {
//            _runner?.Dispose();
//            base.Dispose(disposing);
//        }

//        public UserLogin CreateLogin()
//        {
//            var username = Guid.NewGuid().ToString("N");
//            var password = Guid.NewGuid().ToString("N");
//            var id = Interlocked.Increment(ref _idCounter).ToString();

//            _options.Users.Add(username, new OptionsUserData {DisplayName = username, Id = id, Password = password});

//            return new UserLogin {Id = id, Name = username, Password = password};
//        }

//        protected override void ConfigureWebHost(IWebHostBuilder builder)
//        {
//            var runner = MongoDbRunner.Start();
//            _runner = runner;

//            builder.ConfigureServices(services =>
//            {
//                services.AddSingleton<IAuthService>(new OptionsAuthService(
//                    new OptionsWrapper<UserCredentialsOptions>(_options)));

//                services.AddSingleton<IOptions<MongoDbOptions>>(new OptionsWrapper<MongoDbOptions>(new MongoDbOptions
//                {
//                    DatabaseName = Guid.NewGuid().ToString("N"), ConnectionString = runner.ConnectionString,
//                }));

//                services.AddSingleton(new Mock<IOpenConferenceRepo>().Object);
//            });
//        }
//    }
//}

