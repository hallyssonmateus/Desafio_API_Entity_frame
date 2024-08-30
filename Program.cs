using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OrganizadorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao"),
    sqlServerOptions => sqlServerOptions.CommandTimeout(180)));

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Teste de conexão com o banco de dados
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrganizadorContext>();
    var connection = context.Database.GetDbConnection();

    try
    {
        connection.Open();
        Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso.");

        // Teste: Listar os nomes das tabelas no banco de dados
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("Tabelas no banco de dados:");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
    }
    finally
    {
        connection.Close();
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
