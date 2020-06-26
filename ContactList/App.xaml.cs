using ContactList.Models;
using ContactList.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace ContactList
{
    public partial class App : Application
    {
        public IContactServices contactServices { get; set; }
        public ObservableCollection<Contact> allContacts { get; set; }
        public App(IContactServices contactServices)
        {
            this.contactServices = contactServices;
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage()) { BarBackgroundColor = Color.FromHex("#1A1A1A") };
        }
        async void RequestPermissions()
        {
            try
            {
                var permisosRead = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
                if (permisosRead != PermissionStatus.Granted)
                {
                    permisosRead = await Permissions.RequestAsync<Permissions.ContactsRead>();
                }
                if (permisosRead == PermissionStatus.Granted)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var watch = Stopwatch.StartNew();

                        allContacts = new ObservableCollection<Contact>(await contactServices.RetrieveContactsAsync());

                        watch.Stop();
                        var elapsedMs = watch.Elapsed.TotalSeconds;
                        Debug.WriteLine("Tiempo al recivir los contactos: " + elapsedMs + " seg");
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        protected override void OnStart()
        {
            RequestPermissions();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
