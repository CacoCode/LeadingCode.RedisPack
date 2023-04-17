using CommunityToolkit.Mvvm.ComponentModel;
using LeadingCode.RedisPack.Apis;
using LeadingCode.RedisPack.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;
using Wpf.Ui.Contracts;
using System.Windows.Documents;

namespace LeadingCode.RedisPack.ViewModels
{
    public partial class RedisReleaseViewModel : ObservableObject,INavigationAware, IScopedDependency
    {
        private bool _isInitialized = false;
        private readonly IGithubRedisApi _githubRedisApi;
        private readonly IDialogService _dialogService;
        public IDialogControl RootDialog;


        [ObservableProperty]
        private IEnumerable<RedisReleaseInfo> _redisReleaseInfos;

        public RedisReleaseViewModel(IGithubRedisApi githubRedisApi, 
            IDialogService dialogService)
        {
            _githubRedisApi = githubRedisApi;
            _dialogService = dialogService;

            
        }


        public void OnNavigatedTo()
        {
            if (!_isInitialized) InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            RootDialog = _dialogService.GetDialogControl();
            RootDialog.DialogHeight = 160;
            RootDialog.Footer = new TextBlock
            {
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.WrapWithOverflow,
                Text = "加载中...",
            };

            RootDialog.Content = new ProgressRing
            {
                IsIndeterminate = true,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _isInitialized = true;
        }

        public void GetRedisList()
        {
            var list = _githubRedisApi.GetAsync().Result;
            RedisReleaseInfos = list.OrderByDescending(a => a.Name).ToList();
        }
    }
}
