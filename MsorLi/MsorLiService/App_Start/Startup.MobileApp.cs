using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using MsorLiService.DataObjects;
using MsorLiService.Models;
using Owin;

namespace MsorLiService
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
            List<ItemCategory> Categories = new List<ItemCategory>
            {
                new ItemCategory { Id = Guid.NewGuid().ToString(), Name = "מוצרי חשמל" },
                new ItemCategory { Id = Guid.NewGuid().ToString(), Name = "ריהוט וכלי בית" },
                new ItemCategory { Id = Guid.NewGuid().ToString(), Name = "בגדים ואופנה" },
                new ItemCategory { Id = Guid.NewGuid().ToString(), Name = "תחביבים" },
            };

            foreach (var c in Categories)
            {
                context.Set<ItemCategory>().Add(c);
            }

            List<User> users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), FirstName="שגיא", LastName="קונווי", Email="1", Password="1", Phone="0544512565", Address="הגעש", Permission="user", ImgUrl="https://msorli.blob.core.windows.net/images/1be3a60a-75d3-4eaf-8d7d-5866f04ba616" },
                new User { Id = Guid.NewGuid().ToString(), FirstName="עידן", LastName="מרסיאנו", Email="2", Password="2", Phone="051554787", Address="המצדים", Permission="user", ImgUrl="https://msorli.blob.core.windows.net/images/%D7%A4%D7%A8%D7%95%D7%A4%D7%99%D7%9C.jpg"  },
            };

            foreach (var u in users)
            {
                context.Set<User>().Add(u);
            }

            base.Seed(context);
        }
    }
}