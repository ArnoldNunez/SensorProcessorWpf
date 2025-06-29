using ReactiveUI;
using SensorProcessorWpf.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : ReactiveUserControl<LoginViewModel>
    {
        public LoginView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.Username, view => view.username.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.Login,
                    view => view.loginBtn)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.LoginErrorMsg,
                    view => view.loginErrorTxt.Content)
                    .DisposeWith(disposables);
            });
        }

        /**
         * Password box doesn't support bindings by default. This callback will handle
         * updating the viewmodel.
         */
        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.Password = password.Password;
            }
        }
    }
}
