using PowershellJSONTest;
using PowershellJSONTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IDataService, DataService>();

var app = builder.Build();

app.MapGet("api/DataFromCsv", (IDataService dataService) =>
{
    string scriptPath = $"{Environment.CurrentDirectory}/{FileNames.ScriptFileName}";
    dataService.CreateCsv(scriptPath);

    string csvPath = $"{Environment.CurrentDirectory}/{FileNames.CsvFileName}";
    string jsonString = dataService.GetJsonFromCsv<TestPerson>(csvPath);

    return jsonString;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
