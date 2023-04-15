using CommunityToolkit.Mvvm.ComponentModel;
using LeadingCode.RedisPack.Apis;
using LeadingCode.RedisPack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;
using CommunityToolkit.Mvvm.Input;

namespace LeadingCode.RedisPack.ViewModels
{
    public partial class RedisReleaseViewModel : ObservableObject, IScopedDependency
    {
        private readonly IGithubRedisApi _githubRedisApi;

        [ObservableProperty]
        private IEnumerable<RedisReleaseInfo> _redisReleaseInfos;

        public RedisReleaseViewModel(IGithubRedisApi githubRedisApi)
        {
            _githubRedisApi = githubRedisApi;
            Task.Run(OnGetRedisTags);
        }

        [RelayCommand]
        private async Task OnGetRedisTags()
        {
            RedisReleaseInfos = await _githubRedisApi.GetAsync();
        }
    }
}
