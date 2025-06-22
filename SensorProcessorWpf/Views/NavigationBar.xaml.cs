using ReactiveUI;
using SensorProcessorWpf.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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

namespace SensorProcessorWpf.Views
{
    /// <summary>
    /// Interaction logic for NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : ReactiveUserControl<NavigationBarViewModel>
    {
        /**
         * Constructor
         */
        public NavigationBar()
        {
            InitializeComponent();
            ViewModel = new NavigationBarViewModel();

            this.WhenActivated(disposableRegistration => {
                //this.OneWayBind(ViewModel,
                //    viewModel => viewModel.NavigateToLogin,
                //    view => view.homeBtn);
            });
        }
    }
}
