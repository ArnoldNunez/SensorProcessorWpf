using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using ReactiveUI;
using SensorProcessorWpf.Views;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessorWpf.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        #region PAGE NAVIGATION

        // The router associated with this Screen.
        // Required by the IScreen interface
        public RoutingState Router { get; }

        // The command that navigates a user to the first view model.
        public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }


        #endregion

        # region NUGET REACTIVEUI EXAMPLE

        // In ReactiveUI, this is the syntax to declare a read-write property
        // that will notify Observers, as well as WPF, that a property has
        // changed. If we declared this as a normal property, we couldn't tell
        // when it has changed!
        private string _searchTerm;

        public string SearchTerm
        {
            get => _searchTerm;
            set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }

        // Here's the interesting part: In ReactiveUI, we can take IObservables
        // and "pipe" them to a Property - whenever the Observable yields a new
        // value, we will notify ReactiveObject that the property has changed.
        //
        // To do this, we have a class called ObservableAsPropertyHelper - this
        // class subscribes to an Observable and stores a copy of the latest value.
        // It also runs an action whenever the property changes, usually calling
        // ReactiveObject's RaisePropertyChanged.
        private readonly ObservableAsPropertyHelper<IEnumerable<NugetDetailsViewModel>> _searchResults;

        public IEnumerable<NugetDetailsViewModel> SearchResults => _searchResults.Value;

        // Here, we want to create a property to represent when the application
        // is performing a search (i.e. when to show the "spinner" control that
        // lets the user know that the app is busy). We also declare this property
        // to be the result of an Observable (i.e. its value is derived from
        // some other property)
        private readonly ObservableAsPropertyHelper<bool> _isAvailable;

        public bool IsAvailable => _isAvailable.Value;

        #endregion

        public MainWindowViewModel()
        {
            #region PAGE NAVIGATION

            // Initialize the router
            Router = new RoutingState();

            // Router uses splat.locator to resolve views for
            // viewmodels, so we need to register our views
            // using Locator.CurrentMutable.Register* methods.
            //
            // Instead of registering views manually, you can
            // use custom IVewLocation implementation,
            // see "View Location" section for details.
            //
            Locator.CurrentMutable.Register(() => new HomeView(), typeof(IViewFor<HomeViewModel>));

            // Manage the routing state. Use the Router.Navigate.Execute
            // command to navigate to different view models.
            //
            // Note, that the Navigate.Execute method accepts an instance
            // of a view model, this allows you to pass parameters to your
            // view models, or to reuse existing view models.
            GoNext = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new HomeViewModel()));

            // You can ask the router to go back. One option is to 
            // execute the default Router.NavigateBack command. Another
            // option is to define your own command iwth custom
            // canExecute condition as such:
            var canGoBack = this
                .WhenAnyValue(x => x.Router.NavigationStack.Count)
                .Select(count => count > 0);
            GoBack = ReactiveCommand.CreateFromObservable(
                () => Router.NavigateBack.Execute(Unit.Default),
                canGoBack);

            #endregion

            #region NUGET REACTIVEUI EXAMPLE

            // Creating our UI declaratively
            //
            // The Properties in this ViewModel are related to each other in different
            // ways - with other frameworks, it is difficult to describe each relation
            // succinctly; the code to implement "The UI spinner spins while the search
            // is live" usually ends up spread out over several event handlers.
            //
            // However, with ReactiveUI, we can describe how properties are related in a
            // very organized clear way. Let's describe the workflow of what the user does
            // in this application, in the order they do it.

            // We're going to take a Property and turn it into an Observable here - this
            // Observable will yield a value every time the Search term changes, which in
            // the XAML, is connected to the TextBox.
            //
            // We're going to use the Throttle operator to ignore changes that happen too
            // quickly, since we don't want to issue a search for each key pressed! We
            // then pull the Value of the change, then filter out changes that are identical,
            // as well as strings that are empty.
            //
            // We then do a SelectMany() which starts the task by converting Task<IEnumerable<T>>
            // into IObservable<IEnumerable<T>>. If subsequent requests are made, the
            // CancellationToken is called. We then ObservableOn the main thread,
            // everything up until this point has been running on a separate thread due
            // to the Throttle().
            //
            // We then use a ObservableAsPropertyHelper and the ToProperty() method to allow
            // us to have the latest results that we can expose through the property to the View.
            _searchResults = this
                .WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .SelectMany(SearchNuGetPackages)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.SearchResults);

            // We subscribe to the "ThrownExceptions" property of our OAPH, where ReactiveUI
            // marshals any exceptions that are thrown in SearchNuGetPackages method.
            // See the "Error Handling" section for more information about this.
            _searchResults.ThrownExceptions.Subscribe(error => { /* Handle errors here */ });

            // A helper method we can use for Visibility or Spinners to show if results are available.
            // We get the latest value of the SearchResults and make sure it's not null.
            _isAvailable = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(searchResults => searchResults != null)
                .ToProperty(this, x => x.IsAvailable);

            #endregion
        }


        #region NUGET REACTIVEUI EXAMPLE

        // Here we search NuGet packages using the NuGet.Client library. Ideally, we should
        // extract such code into a separate service, say, INuGetSearchService, but let's
        // try to avoid overcomplicating things at this time.
        private async Task<IEnumerable<NugetDetailsViewModel>> SearchNuGetPackages(
            string term, CancellationToken token)
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support
            var package = new PackageSource("https://api.nuget.org/v3/index.json");
            var source = new SourceRepository(package, providers);

            var filter = new SearchFilter(false);
            var resource = await source.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
            var metadata = await resource.SearchAsync(term, filter, 0, 10, new NuGet.Common.NullLogger(), token).ConfigureAwait(false);
            return metadata.Select(x => new NugetDetailsViewModel(x));
        }

        #endregion
    }
}
