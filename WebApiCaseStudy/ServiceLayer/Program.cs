
using DAL.DataAccess;
using DAL.Model;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//To register dependencies
builder.Services.AddScoped<IEventDetailsRepo<EventDetails>, EventDetailsRepo>();
builder.Services.AddScoped<ISessionInfoRepo<SessionInfo>, SessionInfoRepo>();

//To configure swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Event API",
        Description = "Event Application",
        TermsOfService = new Uri("https://www.hexaware.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soyeb Ghachi",
            Email = "shoaib.ghachi@gmail.com",
            Url = new Uri("https://linkedin.com/soyeb")
        },
        License = new OpenApiLicense
        {
            Name = "Hexaware",
            Url = new Uri("https://hexaware.com/license")
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please Enter Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
});

//JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

        };
    });

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors();//middleware for default CORS policy
app.UseCors("AllowGetAndPost");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
});
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();
