using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Serilog;
using SkippingCounter.Models;
using SkippingCounter.Services;
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

        public ObservableCollection<SkippingSession> Sessions { get; } = new ObservableCollection<SkippingSession>();

        void Refresh() => Task.Run(async () =>
        {
            await foreach (var item in _dataStore.GetItemsAsync())
            {
                await MainThread.InvokeOnMainThreadAsync(() => Sessions.Add(item));
            }

            IsRefreshing = false;
        });
    }
}
