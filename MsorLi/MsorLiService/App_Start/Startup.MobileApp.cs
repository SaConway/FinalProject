using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using MsorLi.DataObjects;
using MsorLi.Models;
using Owin;

namespace MsorLi
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686 
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MsorLiInitializer());

            // To prevent Entity Framework from modifying your database schema, use a null database initializer
            // Database.SetInitializer<MsorLiContext>(null);

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

    public class MsorLiInitializer : CreateDatabaseIfNotExists<MsorLiContext>
    {
        protected override void Seed(MsorLiContext context)
        {
            List<Item> Items = new List<Item>
            {
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Title ="ארון",
                    ImageUrl ="http://www.doron1949.co.il/images/upload/80-1.jpg",
                    Description = "ארון יפה וחדש",
                    Condition = "חדש",
                    Location = "ngkv",
                    ViewCounter = 0,
                    ContactName = "dsdad",
                    ContactNumber = "Sddad",
                    Date = "dss",
                    Time = "asd",
    }
            };

            foreach (Item item in Items)
            {
                context.Set<Item>().Add(item);
            }

            base.Seed(context);
        }
    }
}

