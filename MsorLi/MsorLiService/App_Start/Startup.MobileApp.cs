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

            List<ItemSubCategory> SubCategories = new List<ItemSubCategory>
            {
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=1, Name="מחשבים וציוד נלווה", MainCategory = "מוצרי חשמל" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=2, Name="מכשירי חשמל", MainCategory = "מוצרי חשמל" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=3, Name="סלולר", MainCategory = "מוצרי חשמל" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=4, Name="אחר", MainCategory = "מוצרי חשמל" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=1, Name="רהיטים", MainCategory = "ריהוט וכלי בית" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=2, Name="כלי בית", MainCategory = "ריהוט וכלי בית" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=3, Name="ציוד משרדי", MainCategory = "ריהוט וכלי בית" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=4, Name="אחר", MainCategory = "ריהוט וכלי בית" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=1, Name="בגדי נשים", MainCategory = "בגדים ואופנה" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=2, Name="בגדי גברים", MainCategory = "בגדים ואופנה" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=3, Name="בגדי נוער", MainCategory = "בגדים ואופנה" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=4, Name="אקססוריז", MainCategory = "בגדים ואופנה" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=5, Name="אחר", MainCategory = "בגדים ואופנה" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=1, Name="ציוד ספורט", MainCategory = "תחביבים" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=2, Name="מוזיקה", MainCategory = "תחביבים" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=3, Name="אומנות", MainCategory = "תחביבים" },
                new ItemSubCategory { Id = Guid.NewGuid().ToString(), Order=4, Name="אחר", MainCategory = "תחביבים" },
            };

            foreach (var sc in SubCategories)
            {
                context.Set<ItemSubCategory>().Add(sc);
            }

            List<Location> Locations = new List<Location>
            {
                new Location { Id = Guid.NewGuid().ToString(), Name = "צפון", Order=1 },
                new Location { Id = Guid.NewGuid().ToString(), Name = "השרון", Order=2 },
                new Location { Id = Guid.NewGuid().ToString(), Name = "מרכז", Order=3 },
                new Location { Id = Guid.NewGuid().ToString(), Name = "אזור ירושלים", Order=4 },
                new Location { Id = Guid.NewGuid().ToString(), Name = "השפלה", Order=5 },
                new Location { Id = Guid.NewGuid().ToString(), Name = "דרום", Order=6 },
            };

            foreach (var l in Locations)
            {
                context.Set<Location>().Add(l);
            }

            base.Seed(context);
        }
    }
}