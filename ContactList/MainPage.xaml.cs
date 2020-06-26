using ContactList.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContactList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<ViewModel> View { get; private set; }

        public MainPage()
        {
            View = new ObservableCollection<ViewModel>();
            InitializeComponent();
            BindingContext = this;
            test();
        }
        private void test()
        {
            View.Add(new ViewModel("Marge", "Te quiero"));
            View.Add(new ViewModel("Carlos", "Texto prueba"));
            View.Add(new ViewModel("Jorge", "Texto"));
        }

        private void ClickToPageContacts(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new ContactsPage()) );
        }
    }
}