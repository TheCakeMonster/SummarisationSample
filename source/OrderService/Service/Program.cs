using SummarisationSample.OrderService.Library;
using SummarisationSample.OrderService.Messaging;
using SummarisationSample.OrderService.Repositories.InMemoryRepository;
using SummarisationSample.OrderService.Service.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddModelServices();
builder.Services.AddInMemoryRepositories();
builder.Services.AddKafkaMessaging();

builder.Services.AddSingleton(typeof(IMessageQueue<,>), typeof(MessageQueue<,>));
builder.Services.AddHostedService<MessagePublishingService>();

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
