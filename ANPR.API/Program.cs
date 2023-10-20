using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Lonsum.Services.ANPR.API.Extensions;
using Microsoft.Lonsum.Services.ANPR.API.Filters;
using Microsoft.Lonsum.Services.ANPR.Application;
using Microsoft.Lonsum.Services.ANPR.Application.Common.Exceptions;
using Microsoft.Lonsum.Services.ANPR.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = builder.Configuration;
    var services = builder.Services;
    services.AddInfrastructure(configuration);
    services.AddApplication();

    services.ConfigureApiBehavior();
    services.ConfigureCorsPolicy();
    services
        .AddHttpClient("ANPRClient", client =>
        {
            string baseURL = configuration.GetSection("ANPR:BaseUrl").Value;
            client.BaseAddress = new Uri(baseURL);
            client.DefaultRequestHeaders.Accept.Clear();
        })
            .ConfigurePrimaryHttpMessageHandler((s) =>
            {
                var username = configuration.GetSection("ANPR:Username").Value;
                var password = configuration.GetSection("ANPR:Password").Value;
                return new HttpClientHandler
                {
                    Credentials = new NetworkCredential(username, password)
                };
            });
    // Add services to the container.

    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        // add JWT Authentication
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter JWT Bearer token **_only_**",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer", // must be lower case
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {securityScheme, new string[] { }}
            }
        );

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Lonsum - ANPR HTTP API",
            Version = "v1",
            Description = "The ANPR Service HTTP API"
        });
        //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        //{
        //    Type = SecuritySchemeType.OAuth2,
        //    Flows = new OpenApiOAuthFlows()
        //    {
        //        Implicit = new OpenApiOAuthFlow()
        //        {
        //            AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
        //            TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
        //            Scopes = new Dictionary<string, string>()
        //            {
        //                { "requests", "ELeave API" }
        //            }
        //        }
        //    }
        //});

        options.OperationFilter<AuthorizeCheckOperationFilter>();
    });
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
