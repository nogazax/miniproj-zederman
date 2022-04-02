
using dotnetProj.Models;

var builder = WebApplication.CreateBuilder();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEntityFrameworkSqlite().AddDbContext<MyDatabaseContext>();


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
    using (var client = new MyDatabaseContext())
    {
        client.Database.EnsureCreated();
    }

app.Run();
