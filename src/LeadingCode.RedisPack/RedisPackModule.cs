using Autofac.Core;
using LeadingCode.RedisPack.Apis;
using LeadingCode.RedisPack.Views;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Wpf.Ui.Contracts;
using Wpf.Ui.Services;

namespace LeadingCode.RedisPack;

[DependsOn(typeof(AbpAutofacModule))]
public class RedisPackModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<MainWindow>();

        //context.Services.AddSingleton<IPageService, PageService>();

        // Theme manipulation
        context.Services.AddSingleton<IThemeService, ThemeService>();

        // TaskBar manipulation
        context.Services.AddSingleton<ITaskBarService, TaskBarService>();

        // Service containing navigation, same as INavigationWindow... but without window
        context.Services.AddSingleton<INavigationService, NavigationService>();
        context.Services.AddScoped<IDialogService, DialogService>();
        context.Services.AddScoped<ISnackbarService, SnackbarService>();

        // Main window with navigation
        //context.Services.AddScoped<INavigationWindow, MainWindow>();
        //context.Services.AddScoped<ViewModels.MainWindowViewModel>();

        // Views and ViewModels
        //context.Services.AddScoped<Views.Pages.DashboardPage>();
        //context.Services.AddScoped<ViewModels.DashboardViewModel>();
        //context.Services.AddScoped<Views.Pages.DataPage>();
        //context.Services.AddScoped<ViewModels.DataViewModel>();
        //context.Services.AddScoped<Views.Pages.SettingsPage>();
        //context.Services.AddScoped<ViewModels.SettingsViewModel>();
        context.Services.AddHttpApi<IGithubRedisApi>();
        context.Services.AddHttpApi<IGithubRedisFileApi>();
        
    }
}
