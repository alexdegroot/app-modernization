using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReadApi
{
    public class Startup
    {
        private const string TenantIdHeaderName = "Raet-Identity-Tenant-Id";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                var repo = endpoints.ServiceProvider.GetRequiredService<EmployeeRepository>();
                endpoints.MapGet("/employees/{employeeId:int:min(1):required}",
                    async context =>
                    {
                        context.Response.ContentType = "application/json";
                        await GetEmployee(context, repo);
                    });
                endpoints.MapGet("/companies/{companyId:int:min(1):required}/employees",
                    async context =>
                    {
                        context.Response.ContentType = "application/json";
                        await GetEmployees(context, repo);
                    });
            });
        }

        private static async Task GetEmployee(HttpContext context, EmployeeRepository employeeRepository)
        {
            string tenantHeader = context.Request.Headers[TenantIdHeaderName];
            if (string.IsNullOrEmpty(tenantHeader))
            {
                throw new ArgumentException("Tenant ID header value is missing.");
            }
            var employeeId = Convert.ToInt32(context.Request.RouteValues["employeeId"]);
            var employee = await employeeRepository.GetEmployee(tenantHeader, employeeId).ConfigureAwait(false);
            context.Response.StatusCode = employee != null
                ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
            var json = JsonSerializer.Serialize(employee, new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
            await context.Response.WriteAsync(json);
        }

        private static async Task GetEmployees(HttpContext context, EmployeeRepository employeeRepository)
        {
            string tenantHeader = context.Request.Headers[TenantIdHeaderName];
            if (string.IsNullOrEmpty(tenantHeader))
            {
                throw new ArgumentException("Tenant ID header value is missing.");
            }
            var companyId = Convert.ToInt32(context.Request.RouteValues["companyId"]);
            var employees = await employeeRepository.GetEmployees(tenantHeader, companyId).ConfigureAwait(false);
            context.Response.StatusCode = StatusCodes.Status200OK;
            var json = JsonSerializer.Serialize(employees, new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
            await context.Response.WriteAsync(json);
        }
    }

}