using NuGet.Protocol.Core.Types;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;

namespace SensorProcessorWpf.ViewModels
{
    public class NugetDetailsViewModel : ReactiveObject
    {
        private readonly IPackageSearchMetadata _metadata;
        private readonly Uri _defaultUrl;

        public Uri IconUrl => _metadata.IconUrl ?? _defaultUrl;
        public string Description => _metadata.Description;
        public Uri ProjectUrl => _metadata.ProjectUrl;
        public string Title => _metadata.Title;

        // Constructor
        public NugetDetailsViewModel(IPackageSearchMetadata metadata)
        {
            _metadata = metadata;
            _defaultUrl = new Uri("https://git.io/fA1fh");

            OpenPage = ReactiveCommand.Create(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = ProjectUrl.ToString()
                });
            });
        }

        // Reactie command allows to execute logic without exposing
        // any of hte implementation details with the view.
        public ReactiveCommand<Unit, Unit> OpenPage { get; }
    }
}
