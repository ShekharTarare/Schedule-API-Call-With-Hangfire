using Hangfire;
using Hangfire.SqlServer;
using TestApi;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Step 1 -> Hangfire configuration with SQL Server storage
var connectionString = builder.Configuration.GetConnectionString("TestDatabase"); 
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    });
});

// Step 2 -> Add Hangfire's server.
builder.Services.AddHangfireServer();

// Register httpclient
builder.Services.AddHttpClient<ApiCallService>();

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

//Step 3 -> Added the dashboard url
app.UseHangfireDashboard("/hangfire");

// Schedule the job to call the API endpoint
RecurringJob.AddOrUpdate<ApiCallService>("call-api-endpoint", service => service.CallApiEndpointAsync(), Cron.Daily); // Adjust the schedule as needed


app.Run();
