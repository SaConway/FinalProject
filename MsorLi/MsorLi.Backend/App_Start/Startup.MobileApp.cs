using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using MsorLi.DataObjects;
using MsorLi.Backend.Models;
using Owin;


namespace MsorLi.Backend
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new Initializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }
            app.UseWebApi(config);
        }
    }

    public class Initializer : CreateDatabaseIfNotExists<MasterDetailContext>
    {
        protected override void Seed(MasterDetailContext context)
        {
            List<Item> todoItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Title = "כיסא", ImageUrl = "https://soholtd.co.il/wp-content/uploads/2016/10/%D7%AA%D7%9E%D7%95%D7%A0%D7%95%D7%AA-10004.png" },
            };

            foreach (Item todoItem in todoItems)
            {
                context.Set<Item>().Add(todoItem);
            }

            base.Seed(context);
        }
    }


}
