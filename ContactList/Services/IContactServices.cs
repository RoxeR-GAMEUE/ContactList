
using ContactList.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContactList.Services
{
    public class ContactEventArgs : EventArgs
    {
        public Contact Contacto { get; }
        public ContactEventArgs(Contact contacto)
        {
            Contacto = contacto;
        }
    }
    public interface IContactServices
    {        
        event EventHandler<ContactEventArgs> OnContactLoaded;
        bool IsLoading { get; }
        Task<IList<Contact>> RetrieveContactsAsync(CancellationToken? token = null);
        Task ContactsAsync(CancellationToken? token = null);
    }
}
