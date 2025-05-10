using ReactiveUI;
using SensorProcessorWpf.ViewModels;
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
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : ReactiveUserControl<HomeViewModel>
    {
        public HomeView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.PlotModel, view => view.LiveGraph.Model)
                    .DisposeWith(disposables);
            });
        }
    }
}
