

namespace ContactList.Models
{
    public class ViewModel
    {
        public string Top { get; set; }
        public string Bot { get; set; }
        public string Icon { get { return Top.ToCharArray()[0].ToString(); } }
        public ViewModel(string top, string bot)
        {
            Top = top;
            Bot = bot;
        }
    }
}
