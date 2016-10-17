using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Controls;
using BillReminder.ViewModel;
using Newtonsoft.Json;

namespace BillReminder.Helpers
{
    public class NavigationService : INavigationService
    {        
        private static readonly Dictionary<Type, string> ViewModelRouting = new Dictionary<Type, string> 
        { 
                { 
                    typeof(MainViewModel), "MainPage.xaml" 
                }, 
                { 
                    typeof(BillViewModel), "BillView.xaml" 
                } 
       };
       
        public bool CanGoBack
        {
            get
            {
                return RootFrame.CanGoBack;
            }
        }
    
        private Frame RootFrame
        {
            get { return Application.Current.RootVisual as Frame; }
        }
        
        public static TJson DecodeNavigationParameter<TJson>(NavigationContext context)
        {
            if (context.QueryString.ContainsKey("param"))
            {
                var param = context.QueryString["param"];
                return string.IsNullOrWhiteSpace(param) ? default(TJson) : JsonConvert.DeserializeObject<TJson>(param);
            }

            throw new KeyNotFoundException();
        }
       
        public void GoBack()
        {
            RootFrame.GoBack();
        }
        
        public void Navigate<TDestinationViewModel>(object parameter)
        {
            var navParameter = string.Empty;
            if (parameter != null)
            {
                navParameter = "?param=" + JsonConvert.SerializeObject(parameter);
            }

            if (ViewModelRouting.ContainsKey(typeof(TDestinationViewModel)))
            {
                var page = ViewModelRouting[typeof(TDestinationViewModel)];

                this.RootFrame.Navigate(new Uri("/" + page + navParameter, UriKind.Relative));
            }
        }      
    } 
}
