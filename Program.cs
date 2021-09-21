using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Radzen;
using TacticView.Data;
using TacticView.Utilitiy;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRazorPages();
services.AddServerSideBlazor()
    .AddCircuitOptions(options => options.DetailedErrors = true);
services.AddSingleton<GitHubQueryService>();
services.AddScoped<AppState>();
services.AddScoped<NotificationService>();
services.AddSingleton<AppInfo>();
services.AddControllers();
services.AddLocalization(options => options.ResourcesPath = "Resources");
services.AddMvc()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);
services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "af", "ar", "as", "bg", "bn", "bs", "ca", "cs", "cy", "da", "de", "el", "es", "et", "fa", "fi", "fil", "fj", "fr", "fr-ca", "ga", "gu", "he", "hi", "hr", "ht", "hu", "id", "is", "it", "ja", "kk", "kmr", "kn", "ko", "ku", "lt", "lv", "mg", "mi", "ml", "mr", "ms", "mt", "mww", "nb", "nl", "or", "otq", "pa", "pl", "prs", "ps", "pt", "pt-pt", "ro", "ru", "sk", "sl", "sm", "sr-Cyrl", "sr-Latn", "sv", "sw", "ta", "te", "th", "tlh-Latn", "tlh-Piqd", "to", "tr", "ty", "uk", "ur", "vi", "yua", "yue", "zh-Hans", "zh-Hant" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});
services.AddHealthChecks();

var app = builder.Build();

AppInfo.Token = app.Configuration["GITHUB_TOKEN"];
AppInfo.GITHUB_CLIENT_ID = app.Configuration["GITHUB_CLIENT_ID"];
AppInfo.GITHUB_CLIENT_SECRET = app.Configuration["GITHUB_CLIENT_SECRET"];


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
var supportedCultures = new[] { "en", "af", "ar", "as", "bg", "bn", "bs", "ca", "cs", "cy", "da", "de", "el", "es", "et", "fa", "fi", "fil", "fj", "fr", "fr-ca", "ga", "gu", "he", "hi", "hr", "ht", "hu", "id", "is", "it", "ja", "kk", "kmr", "kn", "ko", "ku", "lt", "lv", "mg", "mi", "ml", "mr", "ms", "mt", "mww", "nb", "nl", "or", "otq", "pa", "pl", "prs", "ps", "pt", "pt-pt", "ro", "ru", "sk", "sl", "sm", "sr-Cyrl", "sr-Latn", "sv", "sw", "ta", "te", "th", "tlh-Latn", "tlh-Piqd", "to", "tr", "ty", "uk", "ur", "vi", "yua", "yue", "zh-Hans", "zh-Hant" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles();

app.UseRouting();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapBlazorHub();
app.MapDefaultControllerRoute();
app.MapFallbackToPage("/_Host");

await app.RunAsync();