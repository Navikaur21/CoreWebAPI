using CoreWebAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Data
{
    public class DummyData
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DocumentContext>();
                context.Database.EnsureCreated();//context.Database.Migrate();
                if (context.DocumentList != null && context.DocumentList.Any())
                    return;
                var doclist =  GetDocuments().ToArray();
                context.DocumentList.AddRange(doclist);
                context.SaveChanges();
            }
           

        }
        public static List<DocumentLog>GetDocuments()
        {
            List<DocumentLog> doclist = new List<DocumentLog>()
            {
                //new DocumentLog{Name="Compliance",FilePath=""},
                //new DocumentLog{Name="Administration",FilePath=""},
                //new DocumentLog{Name="Finance",FilePath=""}
            };
            return doclist;
        }
    }
}
