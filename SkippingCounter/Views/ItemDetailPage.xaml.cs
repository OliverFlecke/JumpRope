using System.ComponentModel;
using Xamarin.Forms;
using SkippingCounter.ViewModels;

namespace SkippingCounter.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}