// src/CareRoute.Api/Program.cs   (Api layer · composition root)
var builder = WebApplication.CreateBuilder(args);   // ①

builder.Services.AddControllers();                  // ②
builder.Services.AddOpenApi();                      // ③

var app = builder.Build();                          // ④

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                               // ⑤
}

app.UseHttpsRedirection();                          // ⑥
app.UseAuthorization();                             // ⑦
app.MapControllers();                               // ⑧

app.Run();                                          // ⑨