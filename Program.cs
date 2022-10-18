using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using online_store_api.Data;
using online_store_api.Helpers;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "online-store-api", Version = "v1" });


        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,

            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("/auth/token", UriKind.Relative),
                    
                },
              
            },
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };

        options.AddSecurityDefinition("Bearer", scheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { scheme, new[] { "online-store-api" } }
        });
    }
);


// add the ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql("Host=localhost;Database=my_store;Username=postgres;Password=root"));

// Authentication
builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection("Auth")
);

var authOptions =  builder.Configuration.GetSection("Auth").Get<AuthOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer("Bearer", options => {
    // options.Authority = "https://localhost:5001";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidIssuer = authOptions.Issuer,
        ValidAudience = authOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authOptions.Secret))
    };;
});

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
// 

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

 