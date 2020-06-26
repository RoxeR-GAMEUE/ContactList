
namespace ContactList.Models
{
    public class Contact : ViewModel
    {
        public string Id { get; set; }
        public string Name { get { return base.Top; } set { base.Top = value; } }
        public string PhoneNumber { get { return base.Bot; } set { base.Bot = value; } }
        public Contact(string name, string phoneNumber) : base(name, phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }
        public Contact() { }
        public bool Equals(Contact contact)
        {
            return this.Id.Equals(contact.Id);
        }
    }
}
