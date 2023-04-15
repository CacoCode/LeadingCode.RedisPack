using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LeadingCode.RedisPack.ViewModels;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Controls.Navigation;

namespace LeadingCode.RedisPack.Views.Pages
{
    /// <summary>
    /// RedisReleasePage.xaml 的交互逻辑
    /// </summary>
    public partial class RedisReleasePage : INavigableView<ViewModels.RedisReleaseViewModel>, IScopedDependency
    {

        public RedisReleaseViewModel ViewModel
        {
            get;
        }

        public RedisReleasePage(ViewModels.RedisReleaseViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        
    }
}
