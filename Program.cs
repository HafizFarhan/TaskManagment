using Injazat.DataAccess.Data;
using Injazat.DataAccess.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;
using EasyRepository.EFCore.Generic;
using Injazat.Presentation.Services.LogDBService;
using Injazat.Presentation.Services.UserService;
using Injazat.Presentation.Services.TaskService;
using Injazat.Presentation.Services.BidService;
using Injazat.Presentation.Services.SubtaskService;
using Injazat.Presentation.Services.TaskActivityService;
using Injazat.Presentation.Services.TaskAssignmentService;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set minimum log level
    .Enrich.FromLogContext()
    .WriteTo.File("Logs\\log-.txt", rollingInterval: RollingInterval.Minute) // Log to file
    .WriteTo.Console()
    .CreateLogger();

// Configure Serilog in the host
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console();
});

// Add services to the container.
// Configure the DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Update the connection string as needed

// Adding Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Configure identity options as needed
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILogDbService, LogDbService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddScoped<ISubTaskService, SubTaskService>();
builder.Services.AddScoped<ITaskActivityService, TaskActivityService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the Swagger generator, defining one or more Swagger documents
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Injazat API", Version = "v1" });

    // Register the custom operation filter
    c.OperationFilter<AddAcceptHeaderFilter>();
});

builder.Services.ApplyEasyRepository<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            // Log the exception
            Log.Error(exception, "An unhandled exception occurred.");

            // You can also return a response, redirect, etc.
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
        });
    });

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve Swagger-UI, specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Injazat API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Custom OperationFilter to add the Accept header
public class AddAcceptHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add Accept header to each operation
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "JsonPlease",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("application/json") // Set default value
            }
        });
    }
}
