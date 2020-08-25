using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

namespace WriteApi
{
    public class Startup
    {
        private EmployeeRepository repository;

        public Startup()
        {
            var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING");
            repository = new EmployeeRepository(connectionString);
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
                endpoints.MapPut("/employees", CreateEmployee);
                endpoints.MapPost("/employees/{employeeId:int:min(1):required}", UpdateEmployee);
            });
        }

        private async Task UpdateEmployee(HttpContext context)
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

            await repository.Update(employeeId, employee);
            
            context.Response.StatusCode = StatusCodes.Status202Accepted;
            await context.Response.WriteAsync($"Hello {employeeId}!");
        }

        private async Task CreateEmployee(HttpContext context)
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

            var id = await repository.Add(employee);

            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsJsonAsync(new {Id = id});
        }
    }
}