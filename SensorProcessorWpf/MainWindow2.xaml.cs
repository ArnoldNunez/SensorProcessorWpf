using ReactiveUI;
using SensorProcessorWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SensorProcessorWpf
{
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : ReactiveWindow<MainWindowViewModel2>
    {
        public MainWindow2()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel2();

            this.WhenActivated(disposableRegistration =>
            {
                //this.OneWayBind(this.navigationBar.ViewModel, viewModel => viewModel.);

                #region PAGE NAVIGATION

                this.OneWayBind(this.ViewModel, 
                    viewModel => viewModel.Router,
                    view => view.RoutedViewHost.Router)
                .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.NavigateToLogin,
                    view => view.homeBtn)
                .DisposeWith(disposableRegistration);

                #endregion
            });

            
        }
    }
}
