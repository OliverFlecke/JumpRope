using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Serilog;
using SkippingCounter.Models;
using SkippingCounter.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkippingCounter.ViewModels
{
    public class SessionHistoryViewModel : BaseViewModel
    {
        readonly IDataStore<SkippingSession> _dataStore;
        private bool _isRefreshing;

        public SessionHistoryViewModel(
            ILogger logger,
            IDataStore<SkippingSession> dataStore)
            : base(logger)
        {
            _dataStore = dataStore;

            RefreshCmd = new Command(Refresh);
            Refresh();
        }

        public ICommand RefreshCmd { get; }

        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public ObservableRangeCollection<SkippingSession> Sessions { get; } = new ObservableRangeCollection<SkippingSession>();

        void Refresh() => Task.Run(async () =>
        {
            var items = await _dataStore.GetItemsAsync(true).ToListAsync();
            MainThread.BeginInvokeOnMainThread(() => Sessions.ReplaceRange(items));

            IsRefreshing = false;
        });
    }
}
