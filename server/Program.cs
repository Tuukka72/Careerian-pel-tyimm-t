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
app.UseCors("AllowReact");
app.UseHttpsRedirection();

// Register users
app.MapPost("/login/register", async (
    IConfiguration config,
    LoginRequest request) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest("Username and password required");
        }
        var connString = config.GetConnectionString("DefaultConnection");
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        // Check if username already exists
        var checkCmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM login WHERE name = @name",
            conn);
        checkCmd.Parameters.AddWithValue("name", request.Name);
        var exists = (long)await checkCmd.ExecuteScalarAsync();
        if (exists > 0)
        {
            return Results.BadRequest("Username already exists");
        }

        // Insert user
        var insertCmd = new NpgsqlCommand(
            "INSERT INTO login (name, password) VALUES (@name, @password)",
            conn);
        insertCmd.Parameters.AddWithValue("name", request.Name);
        insertCmd.Parameters.AddWithValue("password", request.Password);
        await insertCmd.ExecuteNonQueryAsync();
        return Results.Ok(new
        {
            message = "User registered"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Register error: {ex.Message}");
    }
});

// Check if user exists
app.MapPost("/login/check", async (
    IConfiguration config,
    LoginRequest request) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(@"
            SELECT id, name
            FROM login
            WHERE name = @name
            AND password = @password
        ", conn);

        cmd.Parameters.AddWithValue("name", request.Name);
        cmd.Parameters.AddWithValue("password", request.Password);
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return Results.Ok(new
            {
                success = true,
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name"))
            });
        }
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Login error: {ex.Message}");
    }
});
// Test if db is online and working
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
// Gets all rides
app.MapPost("/kyydit", async (HttpContext context, IConfiguration config) =>
{
    try
    {
        var data = await context.Request.ReadFromJsonAsync<KyytiDto>();

        if (data == null)
            return Results.BadRequest("Invalid data");

        var connString = config.GetConnectionString("DefaultConnection");

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            INSERT INTO kyydit
            (kuski_id, mista, mihin, lahtoaika, paikkoja, tyyppi, lisatiedot)
            VALUES
            (@kuski_id, @mista, @mihin, @lahtoaika, @paikkoja, @tyyppi, @lisatiedot)
        ", conn);

        cmd.Parameters.AddWithValue("@kuski_id", data.kuski_id);
        cmd.Parameters.AddWithValue("@mista", data.mista);
        cmd.Parameters.AddWithValue("@mihin", data.mihin);
        cmd.Parameters.AddWithValue("@lahtoaika", data.lahtoaika);
        cmd.Parameters.AddWithValue("@paikkoja", data.paikkoja);
        cmd.Parameters.AddWithValue("@tyyppi", data.tyyppi);
        cmd.Parameters.AddWithValue("@lisatiedot",
            (object?)data.lisatiedot ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();

        return Results.Ok(new { message = "Kyyti lisätty" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
// Delete rides by id
app.MapDelete("/kyydit/{id}", async (int id, IConfiguration config) =>
{
    try
    {
        var connString = config.GetConnectionString("DefaultConnection");
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(
            "DELETE FROM kyydit WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("@id", id);

        var rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0)
            return Results.NotFound();

        return Results.Ok(new { message = "Deleted" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
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
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name"))
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

public class LoginRequest
{
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
}

public class KyytiDto
{
    public int kuski_id { get; set; }
    public string mista { get; set; }
    public string mihin { get; set; }
    public DateTime lahtoaika { get; set; }
    public int paikkoja { get; set; }
    public string tyyppi { get; set; }
    public string? lisatiedot { get; set; }
}