/*
 * JWT-токен состоит из трех частей:
 * Header - объект JSON, который содержит информацию о типе токена и алгоритме его шифрования
 * Payload - объект JSON, который содержит данные, нужные для авторизации пользователя
 * Signature - строка, которая создается с помощью секретного кода, Headera и Payload. Эта строка служит для верификации токена
 * Части JWT токена разделены точками
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder();
 
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true
        };
});
var app = builder.Build();
 
app.UseAuthentication();
app.UseAuthorization();

// Эндпоинт для генерации токена.
app.Map("/login/{username}", (string username) => 
{
    var claims = new List<Claim> {new Claim(ClaimTypes.Name, username) };
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
    return new JwtSecurityTokenHandler().WriteToken(jwt);
});
 
// Вернёт 401, но если к запросу добавить хэдер Authorization, а в значение вставить "Bearer {token}" то норм.
app.Map("/data", [Authorize] () => new { message= "Hello World!" });
 
app.Run();
 
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new (Encoding.UTF8.GetBytes(KEY));
}