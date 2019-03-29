using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExemploTus.Upload;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace ExemploTus
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var tempPath = Path.Combine(env.ContentRootPath, "temp");

            CreateDirectory(tempPath);
            
            app.UseTus(context => new DefaultTusConfiguration
            {
                //Local onde os arquivos temporários serão salvos
                Store = new TusDiskStore(tempPath),
                // URL onde os uploads devem ser realizados.
                UrlPath = "/upload",
                Events = new Events
                {
                    //O que fazer quando o upload for finalizado
                    OnFileCompleteAsync = async ctx =>
                    {
                        var file = await ((ITusReadableStore)ctx.Store).GetFileAsync(ctx.FileId, ctx.CancellationToken);
                        await ProcessFile.SaveFileAsync(file, env);
                    }
                }
            });

            app.UseStaticFiles();
        }

        private void CreateDirectory(string path)
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
