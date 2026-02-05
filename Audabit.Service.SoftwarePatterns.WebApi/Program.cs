using Audabit.Common.ApiVersioning.AspNet.Extensions;
using Audabit.Common.ExceptionHandling.AspNet.Extensions;
using Audabit.Common.Swagger.AspNet.Extensions;
using Audabit.Service.SoftwarePatterns.App.Patterns._1_Creational;
using Audabit.Service.SoftwarePatterns.App.Patterns._2_Structural;
using Audabit.Service.SoftwarePatterns.App.Patterns._3_Behavioral;
using Audabit.Service.SoftwarePatterns.App.Patterns._4_Architectural;
using Audabit.Service.SoftwarePatterns.App.Patterns._5_Messaging;
using Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency;
using Audabit.Service.SoftwarePatterns.App.Patterns._7_Reliability;
using Audabit.Service.SoftwarePatterns.App.Patterns._8_AntiPatterns;
using Audabit.Service.SoftwarePatterns.App.Principles._0_Principles;
using Audabit.Service.SoftwarePatterns.App.Testing._9_Testing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddSwaggerDocumentation("SoftwarePatterns", "X-API-Key", typeof(Program));

// ASP.NET Core framework services
builder.Services.AddControllers();

// Principle demonstration services
builder.Services.AddScoped<IPrinciplesService, PrinciplesService>();

// Pattern demonstration services
builder.Services.AddScoped<ICreationalPatternsService, CreationalPatternsService>();
builder.Services.AddScoped<IStructuralPatternsService, StructuralPatternsService>();
builder.Services.AddScoped<IBehavioralPatternsService, BehavioralPatternsService>();
builder.Services.AddScoped<IMessagingPatternsService, MessagingPatternsService>();
builder.Services.AddScoped<IArchitecturalPatternsService, ArchitecturalPatternsService>();
builder.Services.AddScoped<IConcurrencyPatternsService, ConcurrencyPatternsService>();
builder.Services.AddScoped<IReliabilityPatternsService, ReliabilityPatternsService>();
builder.Services.AddScoped<ITestingPatternsService, TestingPatternsService>();
builder.Services.AddScoped<IAntiPatternsService, AntiPatternsService>();

var app = builder.Build();

// Configure the HTTP request pipeline (order matters!)
app.UseExceptionMiddleware("SoftwarePatterns");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithVersioning();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }