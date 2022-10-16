using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using online_store_api.Data;
using online_store_api.Helpers;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add the ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql("Host=localhost;Database=my_store;Username=postgres;Password=root"));




// builder.Services.Configure<CurrencyScoopOptions>(
//     Configuration.GetSection("CurrencyScoop");
// );

// It must run the Currency ServiceCurrencyScoopOptions At First
builder.Services.Configure<CurrencyScoopOptions>(
    builder.Configuration.GetSection("CurrencyScoop")
);

builder.Services.AddScoped<CurrencyService>();

// builder.Services.AddHttpClient();
builder.Services.AddHttpClient<CurrencyService>((serviceProvider, client) => 
{
    var options = serviceProvider.GetRequiredService<IOptionsSnapshot<CurrencyScoopOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
}
);


// .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]{
//     TimeSpan.FromSeconds(1),
//     TimeSpan.FromSeconds(5),
//     TimeSpan.FromSeconds(10)
// }));


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

 