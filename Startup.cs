using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzen;
using System.Collections.Generic;
using TacticView.Data;
using TacticView.Utilitiy;

namespace TacticView
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static string Token { get; set; } = null;

        public static string AccessToken { get; set; }
        public const string GITHUB_CLIENT_HEADER = "timheuer-microsoft-com";
        public static string GITHUB_CLIENT_ID { get; set; } = null;
        public static string GITHUB_CLIENT_SECRET { get; set; } = null;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<GitHubQueryService>();
            services.AddScoped<AppState>();
            services.AddScoped<NotificationService>();
            services.AddSingleton<AppInfo>();
            services.AddControllers();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "af", "ar", "as", "bg", "bn", "bs", "ca", "cs", "cy", "da", "de", "el", "es", "et", "fa", "fi", "fil", "fj", "fr", "fr-ca", "ga", "gu", "he", "hi", "hr", "ht", "hu", "id", "is", "it", "ja", "kk", "kmr", "kn", "ko", "ku", "lt", "lv", "mg", "mi", "ml", "mr", "ms", "mt", "mww", "nb", "nl", "or", "otq", "pa", "pl", "prs", "ps", "pt", "pt-pt", "ro", "ru", "sk", "sl", "sm", "sr-Cyrl", "sr-Latn", "sv", "sw", "ta", "te", "th", "tlh-Latn", "tlh-Piqd", "to", "tr", "ty", "uk", "ur", "vi", "yua", "yue", "zh-Hans", "zh-Hant" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            Token = Configuration["GITHUB_TOKEN"];
            GITHUB_CLIENT_ID = Configuration["GITHUB_CLIENT_ID"];
            GITHUB_CLIENT_SECRET = Configuration["GITHUB_CLIENT_SECRET"];
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
