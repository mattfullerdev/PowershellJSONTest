using Microsoft.AspNetCore.Mvc;
using PowershellJSONTest;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DataService>();

var app = builder.Build();

app.MapGet("api/run", ([FromQuery] string script, DataService dataService) =>
{
    if (!Path.HasExtension(script))
        script += ".ps1";
    
    string scriptPath = $"{Environment.CurrentDirectory}/{script}";
    if (!File.Exists(scriptPath))
        throw new FileNotFoundException($"Script {script} not found");
    
    var output = dataService.ExecuteScript(scriptPath);

    return output.Select(item => item.CreateObject()).ToList();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();