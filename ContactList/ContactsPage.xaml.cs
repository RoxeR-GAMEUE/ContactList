using ContactList.Models;
using ContactList.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private void OnContactLoaded(object sender, ContactEventArgs e)
        {
            View.Add(e.Contacto);
            //SubTitle = View.Count + " contacts";
            //labelSubTitle.Text = SubTitle;
        }
        async Task LoadContacts()
        {
            var watch = Stopwatch.StartNew();
            IContactServices contactServices = (Application.Current as App).contactServices;
            contactServices.OnContactLoaded += OnContactLoaded;

            try { await contactServices.ContactsAsync(); }
            catch (TaskCanceledException) { Console.WriteLine("Task was cancelled"); }

            contactServices.OnContactLoaded -= OnContactLoaded;
            watch.Stop();
            var elapsedMs = watch.Elapsed.TotalSeconds;
            Debug.WriteLine("Tiempo al leer los contactos: " + elapsedMs + " seg");
        }
        private void ClickReloadContacts(object sender, EventArgs e)
        {
            View.Clear();
            Device.BeginInvokeOnMainThread(async () => await LoadContacts() );
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