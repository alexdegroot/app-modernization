using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Hosting;

namespace WriteApi
{
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
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
                endpoints.MapPut("/employees", x => CreateEmployee(x, repo));
                endpoints.MapPost("/employees/{employeeId:int:min(1):required}", x => UpdateEmployee(x, repo));
            });
        }

        private async Task UpdateEmployee(HttpContext context, EmployeeRepository employeeRepository)
        {
            if (!context.Request.HasJsonContentType())
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var employee = await context.Request.ReadFromJsonAsync<Employee>();

            if (employee == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            
            var employeeId = Convert.ToInt32(context.Request.RouteValues["employeeId"]);

            await employeeRepository.Update(employeeId, employee);
            
            context.Response.StatusCode = StatusCodes.Status202Accepted;
            await context.Response.WriteAsync($"Hello {employeeId}!");
        }

        private async Task CreateEmployee(HttpContext context, EmployeeRepository employeeRepository)
        {
            if (!context.Request.HasJsonContentType())
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var employee = await context.Request.ReadFromJsonAsync<Employee>();

            if (employee == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            await employeeRepository.Add(employee);

            context.Response.StatusCode = StatusCodes.Status201Created;
        }
    }
}