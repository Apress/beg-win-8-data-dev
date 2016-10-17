using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class Password : INotifyPropertyChanged
    {

        public Guid PasswordId { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Passcode { get; set; }
        public string WebSite { get; set; }
        public string Key { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Note { get; set; }
      //  public DateTime LastAccessed { get; set; }
       // public DateTime LastUpdated { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
