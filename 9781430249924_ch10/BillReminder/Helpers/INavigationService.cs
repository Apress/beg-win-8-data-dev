using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillReminder.Helpers
{
    //http://code.msdn.microsoft.com/wpapps/Sharing-CodeAdding-a4c4beb8
    public interface INavigationService
    {         
        bool CanGoBack { get; }
        void GoBack();
        void Navigate<TDestinationViewModel>(object parameter = null);
    }
}
