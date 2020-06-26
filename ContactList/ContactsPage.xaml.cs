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
        public string SubTitle { get; set; }
        public string SearchText { get; set; }
        private bool IsSwipeSearch { get; set; }
        public ObservableCollection<ViewModel> FilteredView { get { return string.IsNullOrEmpty(SearchText) ? View : new ObservableCollection<ViewModel>(View?.Where(s => !string.IsNullOrEmpty(s.Top) && s.Top.ToLower().Contains(SearchText.ToLower()))); } }
        public ContactsPage()
        {
            View = new ObservableCollection<ViewModel>((Application.Current as App).allContacts);
            SubTitle = View.Count + " contacts";
            InitializeComponent();
            BindingContext = this;
        }
        private void FilterSearch(object sender, TextChangedEventArgs e)
        {
            collectView.ItemsSource = FilteredView;
            SubTitle = FilteredView.Count + " contacts";
            labelSubTitle.Text = SubTitle;
        }

        private async void ClickSwipeSearch(object sender, EventArgs e)
        {
            List<Task> tasks = new List<Task>();
            if (!IsSwipeSearch)
            {
                tasks.Add(searchBar.TranslateTo(0, 0));
                tasks.Add(collectView.TranslateTo(0, 50, 100, Easing.BounceOut));
                IsSwipeSearch = true;
                searchBar.Focus();
            }
            else
            {
                tasks.Add(searchBar.TranslateTo(0, -50, 100, Easing.BounceOut));
                tasks.Add(collectView.TranslateTo(0, 0));
                IsSwipeSearch = false;
                searchBar.Text = "";
            }
            await Task.WhenAll(tasks);
        }
    }
}