var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[] {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () => {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapPost("/luckynumber", (LuckyNumberRange range) => {
        if (range.Count < 1) return Results.BadRequest("Invalid count");

        if (!(range.Min < range.Max)) return Results.BadRequest("Invalid min-max range");

        return Results.Ok(PickLuckyNumbers(range));
    })
    .WithName("LuckyNumber")
    .WithOpenApi();

app.Run();

LuckyNumbers PickLuckyNumbers(LuckyNumberRange range) {
    
    var random = new Random();

    var nums = new List<int>();
    
    for (var i = 0; i < range.Count; i++) nums.Add(random.Next(range.Min, range.Max + 1));

    return new LuckyNumbers(nums);
}

record LuckyNumberRange(int Min, int Max, int Count);

record LuckyNumbers(List<int> Numbers)
{
    public int MagicNumber => CalcMagicNumber();

    private int CalcMagicNumber() {
        var total = 0;
        foreach (var num in Numbers) total += num;

        return total / Numbers.Count;
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


