using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/test", () => "Hello from backend!");

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
app.MapPost("/kyydit", async (IConfiguration config, Kyyti uusi) =>
{
    var connString = config.GetConnectionString("DefaultConnection");

    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    var cmd = new NpgsqlCommand(
        "INSERT INTO kyydit (kuski_id, mista_paikasta, mihin_paikkaan, lahtoaika, paikkoja_saatavilla) VALUES ($1, $2, $3, $4, $5)",
        conn
    );

    cmd.Parameters.AddWithValue(uusi.kuski_id);
    cmd.Parameters.AddWithValue(uusi.mista);
    cmd.Parameters.AddWithValue(uusi.mihin);
    cmd.Parameters.AddWithValue(uusi.lahtoaika);
    cmd.Parameters.AddWithValue(uusi.paikkoja);

    await cmd.ExecuteNonQueryAsync();

    return Results.Ok();
});

app.Run();

record Kyyti(int kuski_id, string mista, string mihin, DateTime lahtoaika, int paikkoja);
