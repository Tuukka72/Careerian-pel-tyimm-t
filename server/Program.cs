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
            return Results.BadRequest(
                "Username and password required"
            );
        }

        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        await using var conn =
            new NpgsqlConnection(connString);

        await conn.OpenAsync();

        // Check if username already exists
        var checkCmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM login WHERE name = @name",
            conn
        );

        checkCmd.Parameters.AddWithValue(
            "name",
            request.Name
        );

        var exists =
            (long)await checkCmd.ExecuteScalarAsync();

        if (exists > 0)
        {
            return Results.BadRequest(
                "Username already exists"
            );
        }

        // Insert user
        var insertCmd = new NpgsqlCommand(
            "INSERT INTO login (name, password) VALUES (@name, @password)",
            conn
        );

        insertCmd.Parameters.AddWithValue(
            "name",
            request.Name
        );

        insertCmd.Parameters.AddWithValue(
            "password",
            request.Password
        );

        await insertCmd.ExecuteNonQueryAsync();

        return Results.Ok(new
        {
            message = "User registered"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Register error: {ex.Message}"
        );
    }
});

// Check if user exists
app.MapPost("/login/check", async (
    IConfiguration config,
    LoginRequest request) =>
{
    try
    {
        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        await using var conn =
            new NpgsqlConnection(connString);

        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            SELECT id, name
            FROM login
            WHERE name = @name
            AND password = @password
        ", conn);

        cmd.Parameters.AddWithValue(
            "name",
            request.Name
        );

        cmd.Parameters.AddWithValue(
            "password",
            request.Password
        );

        var reader =
            await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return Results.Ok(new
            {
                success = true,
                id = reader.GetInt32(
                    reader.GetOrdinal("id")
                ),
                name = reader.GetString(
                    reader.GetOrdinal("name")
                )
            });
        }

        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Login error: {ex.Message}"
        );
    }
});

// Add new ride
app.MapPost("/kyydit/add", async (
    IConfiguration config,
    KyytiRequest request) =>
{
    try
    {
        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        await using var conn =
            new NpgsqlConnection(connString);

        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            INSERT INTO kyydit
            (
                kuski_id,
                mista,
                mihin,
                lahtoaika,
                paikkoja,
                tyyppi,
                lisatiedot
            )
            VALUES
            (
                @kuski_id,
                @mista,
                @mihin,
                @lahtoaika,
                @paikkoja,
                @tyyppi,
                @lisatiedot
            )
        ", conn);

        cmd.Parameters.AddWithValue(
            "kuski_id",
            request.Kuski_Id
        );

        cmd.Parameters.AddWithValue(
            "mista",
            request.Mista
        );

        cmd.Parameters.AddWithValue(
            "mihin",
            request.Mihin
        );

        cmd.Parameters.AddWithValue(
            "lahtoaika",
            request.Lahtoaika
        );

        cmd.Parameters.AddWithValue(
            "paikkoja",
            request.Paikkoja
        );

        cmd.Parameters.AddWithValue(
            "tyyppi",
            request.Tyyppi
        );

        cmd.Parameters.AddWithValue(
            "lisatiedot",
            (object?)request.Lisatiedot
                ?? DBNull.Value
        );

        await cmd.ExecuteNonQueryAsync();

        return Results.Ok(new
        {
            success = true,
            message = "Ride added"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Error adding ride: {ex.Message}"
        );
    }
});

// Test if db is online and working
app.MapGet("/db-test", async (
    IConfiguration config) =>
{
    try
    {
        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        await using var conn =
            new NpgsqlConnection(connString);

        await conn.OpenAsync();

        return Results.Ok(
            "Database connected!"
        );
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Database error: {ex.Message}"
        );
    }
});

// Gets all rides
app.MapGet("/kyydit", async (
    IConfiguration config) =>
{
    try
    {
        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        var kyydit = new List<object>();

        await using var conn =
            new NpgsqlConnection(connString);

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
            JOIN login l
            ON k.kuski_id = l.id
        ", conn);

        var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            kyydit.Add(new
            {
                id = reader.GetInt32(
                    reader.GetOrdinal("id")
                ),

                kuski_id = reader.GetInt32(
                    reader.GetOrdinal("kuski_id")
                ),

                mista = reader.GetString(
                    reader.GetOrdinal("mista")
                ),

                mihin = reader.GetString(
                    reader.GetOrdinal("mihin")
                ),

                lahtoaika = reader.GetDateTime(
                    reader.GetOrdinal("lahtoaika")
                ),

                paikkoja = reader.GetInt32(
                    reader.GetOrdinal("paikkoja")
                ),

                tyyppi = reader.GetString(
                    reader.GetOrdinal("tyyppi")
                ),

                lisatiedot =
                    reader.IsDBNull(
                        reader.GetOrdinal(
                            "lisatiedot"
                        )
                    )
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal(
                            "lisatiedot"
                        )
                    ),

                name = reader.GetString(
                    reader.GetOrdinal("name")
                )
            });
        }

        return Results.Ok(kyydit);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Error loading rides: {ex.Message}"
        );
    }
});

// Get all users
app.MapGet("/login", async (
    IConfiguration config) =>
{
    try
    {
        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        var login = new List<object>();

        await using var conn =
            new NpgsqlConnection(connString);

        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT * FROM login",
            conn
        );

        var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            login.Add(new
            {
                id = reader.GetInt32(
                    reader.GetOrdinal("id")
                ),

                name = reader.GetString(
                    reader.GetOrdinal("name")
                )
            });
        }

        return Results.Ok(login);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Error loading logins: {ex.Message}"
        );
    }
});

// Delete ride
app.MapDelete("/kyydit/delete/{id}", async (
    IConfiguration config,
    int id) =>
{
    try
    {
        var connString =
            config.GetConnectionString(
                "DefaultConnection"
            );

        await using var conn =
            new NpgsqlConnection(connString);

        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            DELETE FROM kyydit
            WHERE id = @id
        ", conn);

        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected =
            await cmd.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            return Results.NotFound(
                "Ride not found"
            );
        }

        return Results.Ok(new
        {
            success = true,
            message = "Ride deleted"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            $"Error deleting ride: {ex.Message}"
        );
    }
});

app.Run();

public class LoginRequest
{
    public string Name { get; set; } = "";

    public string Password { get; set; } = "";
}

public class KyytiRequest
{
    public int Kuski_Id { get; set; }

    public string Mista { get; set; } = "";

    public string Mihin { get; set; } = "";

    public DateTime Lahtoaika { get; set; }

    public int Paikkoja { get; set; }

    public string Tyyppi { get; set; } = "";

    public string? Lisatiedot { get; set; }
}