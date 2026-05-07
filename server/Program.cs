using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/db-test", async (IConfiguration config) =>
{
    var connString = config.GetConnectionString("DefaultConnection");

    await using var conn = new NpgsqlConnection(connString);

    await conn.OpenAsync();

    return "Database connected!";
});

app.MapGet("/kyydit", async (IConfiguration config) =>
{
    var connString = config.GetConnectionString("DefaultConnection");

    var kyydit = new List<object>();

    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    var cmd = new NpgsqlCommand("SELECT * FROM kyydit", conn);
    var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        kyydit.Add(new
        {
            id = reader.GetInt32(0),
            kuski_id = reader.GetInt32(1),
            mista = reader.GetString(2),
            mihin = reader.GetString(3),
            lahtoaika = reader.GetDateTime(4),
            paikkoja = reader.GetInt32(5)
        });
    }

    return kyydit;
});

app.Run();