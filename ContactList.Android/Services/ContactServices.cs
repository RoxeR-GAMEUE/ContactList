
using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using ContactList.Models;
using ContactList.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ContactList.Droid.Services
{
    public class ContactServices : IContactServices
    {
        private bool stopLoad = false;
        private bool _isLoading { get; set; }
        public bool IsLoading => _isLoading;

        public event EventHandler<ContactEventArgs> OnContactLoaded;

        public async Task<IList<Contact>> RetrieveContactsAsync(CancellationToken? token = null)
        {
            stopLoad = false;
            
            if (!token.HasValue)
                token = CancellationToken.None;

            // We create a TaskCompletionSource of decimal
            var taskCompletionSource = new TaskCompletionSource<IList<Contact>>();

            // Registering a lambda into the cancellationToken
            token.Value.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                stopLoad = true;
                taskCompletionSource.TrySetCanceled();
            });

            _isLoading = true;

            var task = LoadContactsAsync();

            // Wait for the first task to finish among the two
            var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);
            _isLoading = false;

            return await completedTask;
        }
        public async Task ContactsAsync(CancellationToken? token = null)
        {
            stopLoad = false;
            if (!token.HasValue)
                token = CancellationToken.None;
            var taskCompletionSource = new TaskCompletionSource<IList<Contact>>();
            token.Value.Register(() =>
            {
                stopLoad = true;
                taskCompletionSource.TrySetCanceled();
            });
            _isLoading = true;
            var task = ReadContactsAsync();
            var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);
            _isLoading = false;
        }

        async Task ReadContactsAsync()
        {
            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.CommonDataKinds.Phone.Number };
            await Task.Run(() =>
            {
                var cursor = Application.Context.ContentResolver.Query(uri, projection, null, null, null);
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var contact = new Contact();
                        contact.Id = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                        contact.Name = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                        var number = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                        number = number.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                        number = Regex.Replace(number, @".{1,}(?=\d{10}$)", string.Empty);
                        contact.PhoneNumber = number;
                        if (number.Length == 10)
                            OnContactLoaded?.Invoke(this, new ContactEventArgs(contact));
                        if (stopLoad)
                            break;
                    } while (cursor.MoveToNext());
                }
            });
        }
        async Task<IList<Contact>> LoadContactsAsync()
        {
            IList<Contact> contacts = new List<Contact>();
            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            string[] projection = { 
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.CommonDataKinds.Phone.Number };
            await Task.Run(() =>
            {
                var cursor = Application.Context.ContentResolver.Query(uri, projection, null, null, null);
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var contact = new Contact();
                        contact.Id = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                        contact.Name = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                        var number = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                        number = number.Replace(" ", "").Replace("(", "").Replace(")","").Replace("-","");
                        number = Regex.Replace(number, @".{1,}(?=\d{10}$)", string.Empty);
                        contact.PhoneNumber = number;
                        if (number.Length == 10)
                            contacts.Add(contact);
                        if (stopLoad)
                            break;
                    } while (cursor.MoveToNext());
                }
            });
            return contacts;
        }
    }
}