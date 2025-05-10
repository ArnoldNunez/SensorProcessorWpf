using ReactiveUI;
using SensorProcessorWpf.ViewModels;
using System.Reactive.Disposables;
using System.Windows;


namespace SensorProcessorWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow() 
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                // Uses Wpf global converter for bool to visibility
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsAvailable,
                    view => view.searchResultsListBox.Visibility)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.SearchResults,
                    view => view.searchResultsListBox.ItemsSource)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    viewModel => viewModel.SearchTerm,
                    view => view.searchTextBox.Text)
                    .DisposeWith(disposableRegistration);

                #region PAGE NAVIGATION

                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Router,
                    view => view.RoutedViewHost.Router)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.GoNext,
                    view => view.GoNextButton)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.GoBack,
                    view => view.GoBackButton)
                    .DisposeWith(disposableRegistration);

                #endregion

            });
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("HI");
        }
    }
}