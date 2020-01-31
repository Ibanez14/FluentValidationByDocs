using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidationWeb.Models;
using FluentValidationWeb.Validators.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluentValidationWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(ops => ops.Filters.Add<ValidationFilter>())
                    .AddFluentValidation(ops =>
                    {
                        // so now ModelState.IsValid will be with FluentValidations
                        ops.RegisterValidatorsFromAssemblyContaining<Startup>();

                        // means that you have to SetValidators when Validate Child Props
                        // in Validators (see UserRregisterRequestValidator example)
                        ops.ImplicitlyValidateChildProperties = false;

                        // so this one disable any potential DataAnnotation validation
                        ops.RunDefaultMvcValidationAfterFluentValidationExecutes = false;

                    });

                    

            // this is unnecessary as we added all validators from assembly above
            //services.AddTransient<IValidator<UserRegisterRequest>, UserRegisterRequestValidator>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
