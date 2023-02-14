using Microsoft.EntityFrameworkCore;

//Conexión a la base de datos
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SmarthomeContext>(options =>
    options.UseMySQL("Server=localhost;Database=smarthome;Uid=root;Password="));

//Conexión a una base de datos en memoria
//builder.Services.AddDbContext<SmarthomeContext>(options =>
//    options.UseInMemoryDatabase("SensorsList"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Mi SmartHomeApi", Version ="V1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Mi SamartHome API");
app.MapGet("/sensores", async (SmarthomeContext db) =>
{
    return await db.Sensors.ToListAsync();
});

app.MapPost("/sensores", async (Sensor s, SmarthomeContext db) =>
{
    s.Date = DateTime.Now;
    db.Sensors.Add(s);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/sensores/{id}", async (int id, Sensor s, SmarthomeContext db) =>
{
    var sensor = await db.Sensors.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    sensor.Name = s.Name;
    sensor.Value = s.Value;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/sensores/{id}", async (int id, SmarthomeContext db) =>
{
    var sensor = await db.Sensors.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    db.Sensors.Remove(sensor);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

class Sensor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public DateTime Date { get; set; }
}

class SmarthomeContext : DbContext
{
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public SmarthomeContext(DbContextOptions<SmarthomeContext> options) : base(options)
    {
    }
}