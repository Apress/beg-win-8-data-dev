using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class Category : INotifyPropertyChanged
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
