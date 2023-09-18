using gRPChat.Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace gRPChat.Mobile.Views
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