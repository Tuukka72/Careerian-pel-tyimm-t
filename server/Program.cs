using System.ComponentModel.Design;
using Microsoft.AspNetCore.Mvc.Razor;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Allow React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowReact");

// Redirect HTTP -> HTTPS
app.UseHttpsRedirection();


// TEST DATABASE CONNECTION
app.MapGet("/db-test", async (IConfiguration config) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");

        await using var conn = new NpgsqlConnection(connString);

        await conn.OpenAsync();

        return Results.Ok("Database connected!");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database error: {ex.Message}");
    }
});


// GET ALL RIDES
app.MapGet("/kyydit", async (IConfiguration config) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");

        var kyydit = new List<object>();

        await using var conn = new NpgsqlConnection(connString);

        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
        SELECT 
            k.id,
            k.kuski_id,
            k.mista,
            k.mihin,
            k.lahtoaika,
            k.paikkoja,
            k.tyyppi,
            k.lisatiedot,
            l.name
        FROM kyydit k
        JOIN login l ON k.kuski_id = l.id
        ", conn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            kyydit.Add(new
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                kuski_id = reader.GetInt32(reader.GetOrdinal("kuski_id")),
                mista = reader.GetString(reader.GetOrdinal("mista")),
                mihin = reader.GetString(reader.GetOrdinal("mihin")),
                lahtoaika = reader.GetDateTime(reader.GetOrdinal("lahtoaika")),
                paikkoja = reader.GetInt32(reader.GetOrdinal("paikkoja")),
                tyyppi = reader.GetString(reader.GetOrdinal("tyyppi")),
                lisatiedot = reader.IsDBNull(reader.GetOrdinal("lisatiedot"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("lisatiedot")),
                name = reader.GetString(reader.GetOrdinal("name"))
            });
        }

        return Results.Ok(kyydit);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error loading rides: {ex.Message}");
    }
});
app.MapGet("/login", async (IConfiguration config) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");
        var login = new List<object>();
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("SELECT * FROM login", conn);
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            login.Add(new
            {
                id = reader.GetInt32(reader.GetOrdinal("Id")),
                name = reader.GetString(reader.GetOrdinal("Name")),
                password = reader.GetString(reader.GetOrdinal("Password"))
            });
        }
        return Results.Ok(login);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error loading logins: {ex.Message}");
    }
});

app.Run();