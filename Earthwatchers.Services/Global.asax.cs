using System;
using System.Web.Routing;
using Earthwatchers.Data;
using Earthwatchers.Services.Formatters;
using Earthwatchers.Services.Resources;
using Ninject;
using Microsoft.AspNet.SignalR;
using System.Web;
using NLog;

namespace Earthwatchers.Services
{
    public class Global : System.Web.HttpApplication
    {
        private readonly string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        void Application_Start(object sender, EventArgs e)
        {
            var config=new ApiConfiguration()
            {
                CreateInstance = (serviceType, context, request) => GetKernel().Get(serviceType),
                ErrorHandlers = (handlers, endpoint, description) => handlers.Add(new GlobalErrorHandler())
            };
            RouteTable.Routes.SetDefaultHttpConfiguration(config);

            config.Formatters.Add(new WktFormatter());
            //RouteTable.Routes.MapServiceRoute<ElmahResource>("api/elmah");
            RouteTable.Routes.MapServiceRoute<StatisticsQueryResource>("api/statisticsquery");
            RouteTable.Routes.MapServiceRoute<ContestResource>("api/contest");
            RouteTable.Routes.MapServiceRoute<PopupMessageResource>("api/popupmessages");
            RouteTable.Routes.MapServiceRoute<JaguarResource>("api/jaguarpositions");
            RouteTable.Routes.MapServiceRoute<EarthwatcherResource>("api/earthwatchers");
            RouteTable.Routes.MapServiceRoute<SatelliteImageResource>("api/satelliteimages");
            RouteTable.Routes.MapServiceRoute<LandResource>("api/land");
            RouteTable.Routes.MapServiceRoute<CommentResource>("api/comments");
            RouteTable.Routes.MapServiceRoute<FlagResource>("api/flags");
            RouteTable.Routes.MapServiceRoute<NewsResource>("api/news");
            RouteTable.Routes.MapServiceRoute<AuthenticatorResource>("api/authenticate");
            RouteTable.Routes.MapServiceRoute<PasswordResource>("api/password");
            RouteTable.Routes.MapServiceRoute<ScoresResource>("api/scores");
            RouteTable.Routes.MapServiceRoute<CollectionsResource>("api/upload");
            RouteTable.Routes.MapServiceRoute<BasecampResource>("api/basecamp");
            RouteTable.Routes.MapServiceRoute<LayersResource>("api/layers");
            RouteTable.Routes.MapServiceRoute<CollectionsResource>("api/collections");
            RouteTable.Routes.MapServiceRoute<HomeResource>("api");

            HubConfiguration hubConfiguration = new HubConfiguration()
            {
                EnableCrossDomain = true,
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true
            };

            RouteTable.Routes.MapHubs(hubConfiguration);
        }

        private IKernel GetKernel()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<ICommentRepository>().To<CommentRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IFlagRepository>().To<FlagRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<ISatelliteImageRepository>().To<SatelliteImageRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<INewsRepository>().To<NewsRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<ILandRepository>().To<LandRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IEarthwatcherRepository>().To<EarthwatcherRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<ILayerRepository>().To<LayerRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<ISettingsRepository>().To<SettingsRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IScoreRepository>().To<ScoreRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IBasecampRepository>().To<BasecampRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<ICollectionsRepository>().To<CollectionsRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IJaguarRepository>().To<JaguarRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IPopupMessageRepository>().To<PopupMessageRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IContestRepository>().To<ContestRepository>().WithConstructorArgument("connectionString", connectionstring);
            kernel.Bind<IStatisticsQueryRepository>().To<StatisticsQueryRepository>().WithConstructorArgument("connectionString", connectionstring);
            return kernel;
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError().GetBaseException();


            logger.Error("Unhandled Exception. \nMessage: {0} \nStackStrace:{1}", exc.Message, exc.StackTrace);

            // Clear the error from the server
            Server.ClearError();
        }
    }
}
