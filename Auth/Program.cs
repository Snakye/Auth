var builder = WebApplication.CreateBuilder(args);

/*
 * Аутентификация отвечает на вопрос "Кем является пользователь?"
 * Авторизация отвечает на вопрос "Какие права имеет пользователь?"
 */

builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();