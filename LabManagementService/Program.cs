using FluentValidation;
using LabManagementService.Database;
using LabManagementService.Entities;
using LabManagementService.Repositories;
using LabManagementService.Settings;
using LabManagementService.Shared;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings"));

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();
var serviceSettings = builder.Configuration.GetSection("ServiceSettings").Get<ServiceSettings>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRepository<LabTest>, LabTestRepository>();
builder.Services.AddScoped<IRepository<LabOrder>, LabOrderRepository>();
builder.Services.AddScoped<IRepository<LabResult>, LabResultRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Add all consumers from the assembly
    x.AddConsumers(Assembly.GetExecutingAssembly());

    // Add individual consumers (alternative approach)
    // x.AddConsumer<OrderCreatedConsumer>();
    // x.AddConsumer<UserRegisteredConsumer>();

    // Using RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings.Host);

        // Configure endpoints with kebab case naming
        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

                // Additional configuration
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.UseInMemoryOutbox();
    });
});

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate(); // This is the key line
                                      // Optional: Seed initial data if needed after migration
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        // Consider logging the error or handling it appropriately
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
