using MongoDB.Driver;
using PatientManagementService.Repositories;
using PatientManagementService.Settings;

var builder = WebApplication.CreateBuilder(args);

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
if (serviceSettings == null || string.IsNullOrEmpty(serviceSettings.ServiceName))
{
    throw new InvalidOperationException("ServiceSettings or ServiceName is not configured properly.");
}

builder.Services.AddSingleton(serviceProvider =>
{
    var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
    if (mongoDbSettings == null || string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
    {
        throw new InvalidOperationException("MongoDbSettings or ConnectionString is not configured properly.");
    }

    var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

    return mongoClient.GetDatabase(serviceSettings.ServiceName);
});

builder.Services.AddSingleton<IPatientRepository, PatientsRepository>();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
