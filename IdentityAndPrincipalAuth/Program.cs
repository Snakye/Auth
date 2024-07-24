using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
 
var builder = WebApplication.CreateBuilder();
 
// аутентификация с помощью куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
 
var app = builder.Build();
 
app.UseAuthentication();
 
app.MapGet("/login", async (HttpContext context) =>
{
    // По сути это документ
    var claimsIdentity = new ClaimsIdentity("Undefined");
    
    // Набор документов пользователя, их может быть много
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    // установка аутентификационных куки
    await context.SignInAsync(claimsPrincipal);
    return Results.Redirect("/");
});
 
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});
// Вместо HttpContext можно сразу получить ClaimsPrincipal, что аналогично context.User
app.Map("/", (HttpContext context) =>
{
    // Получаем документы пользователя, определённые выше
    var user = context.User.Identity;
    if (user is not null && user.IsAuthenticated)
    {
        return $"Пользователь аутентифицирован. Тип аутентификации: {user.AuthenticationType}";
    }
    else
    {
        return "Пользователь НЕ аутентифицирован";
    }
});
 
app.Run();