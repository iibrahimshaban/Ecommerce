using Ecommerce.Api;
using Ecommerce.Application.Common.Interfaces;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiDependancies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
    //app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:password")
        }
    ],
    DashboardTitle = "Survay Basket dashboard"
});

RecurringJob.AddOrUpdate<IProductService>(
    "SendNewProductNotification",
    service => service.SendNewProductNotifications(null,CancellationToken.None),
    Cron.Daily
);

app.UseAuthorization();

app.MapControllers();

app.Run();
