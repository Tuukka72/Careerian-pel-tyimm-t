using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowReact");
app.UseHttpsRedirection();

// TEST DB
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
        return Results.Problem(ex.Message);
    }
});

// KYYDIT - GET
app.MapGet("/kyydit", async (IConfiguration config) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");
        var list = new List<object>();

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand("SELECT * FROM kyydit", conn);
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                mista = reader.GetString(reader.GetOrdinal("mista")),
                mihin = reader.GetString(reader.GetOrdinal("mihin")),
                tyyppi = reader.GetString(reader.GetOrdinal("tyyppi")),
                lisatiedot = reader.IsDBNull(reader.GetOrdinal("lisatiedot"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("lisatiedot"))
            });
        }

        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// KYYDIT - POST
app.MapPost("/kyydit", async (IConfiguration config, Kyyti kyyti) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "INSERT INTO kyydit (mista, mihin, tyyppi, lisatiedot) VALUES (@mista, @mihin, @tyyppi, @lisatiedot)",
            conn
        );

        cmd.Parameters.AddWithValue("mista", kyyti.Mista);
        cmd.Parameters.AddWithValue("mihin", kyyti.Mihin);
        cmd.Parameters.AddWithValue("tyyppi", kyyti.Tyyppi);
        cmd.Parameters.AddWithValue("lisatiedot", kyyti.Lisatiedot ?? (object)DBNull.Value);

        await cmd.ExecuteNonQueryAsync();

        return Results.Ok("Ride added");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// USERS - GET
app.MapGet("/login", async (IConfiguration config) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");
        var list = new List<object>();

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand("SELECT * FROM login", conn);
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                password = reader.GetString(reader.GetOrdinal("password"))
            });
        }

        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// REGISTER USER - POST
app.MapPost("/login/register", async (IConfiguration config, User user) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "INSERT INTO login (name, password) VALUES (@name, @password)",
            conn
        );

        cmd.Parameters.AddWithValue("name", user.Name);
        cmd.Parameters.AddWithValue("password", user.Password);

        await cmd.ExecuteNonQueryAsync();

        return Results.Ok("User registered");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// LOGIN USER - POST
app.MapPost("/login/login", async (IConfiguration config, User user) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT * FROM login WHERE name = @name AND password = @password",
            conn
        );

        cmd.Parameters.AddWithValue("name", user.Name);
        cmd.Parameters.AddWithValue("password", user.Password);

        var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return Results.Ok("Login successful");

        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

public class User
{
    public string Name { get; set; }
    public string Password { get; set; }
}

public class Kyyti
{
    public string Mista { get; set; }
    public string Mihin { get; set; }
    public string Tyyppi { get; set; }
    public string? Lisatiedot { get; set; }
}