using ContactList.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ContactList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactsPage : ContentPage
    {
        public ObservableCollection<ViewModel> View { get; }
        public string SubTitle { get; }
        public ContactsPage()
        {
            View = new ObservableCollection<ViewModel>((Application.Current as App).allContacts);
            SubTitle = View.Count + " contacts";
            InitializeComponent();
            BindingContext = this;
        }
    }
}