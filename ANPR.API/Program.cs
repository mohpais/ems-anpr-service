using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Lonsum.Services.ANPR.API.Extensions;
using Microsoft.Lonsum.Services.ANPR.Application;
using Microsoft.Lonsum.Services.ANPR.Application.Common.Exceptions;
using Microsoft.Lonsum.Services.ANPR.Infrastructure;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.ConfigureApiBehavior();
    builder.Services.ConfigureCorsPolicy();
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseExceptionHandler(appError =>
    {
        appError.Run(async context =>
        {
            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature == null) return;

            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = contextFeature.Error switch
            {
                BadRequestException => (int)HttpStatusCode.BadRequest,
                OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                NotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errorResponse = new
            {
                statusCode = context.Response.StatusCode,
                message = contextFeature.Error.GetBaseException().Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        });
    });
    app.UseCors();
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
