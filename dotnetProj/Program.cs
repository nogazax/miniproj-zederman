
using dotnetProj.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder();

    // Add services to the container.

    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
		builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders(new string[]  { "Location", "x-Created-Id" });
        }
        );
    });
    builder.Services.AddEntityFrameworkSqlite().AddDbContext<SqlContext>();
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());




var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    using (var client = new SqlContext())
    {
        client.Database.EnsureCreated();
    }

app.Run();
