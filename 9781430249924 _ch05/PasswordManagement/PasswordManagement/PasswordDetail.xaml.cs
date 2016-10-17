using PasswordManager.Models; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PasswordManager
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PasswordDetail : PasswordManager.Common.LayoutAwarePage
    {
        Password _password = null;
        public PasswordDetail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {


        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            cboCategory.DisplayMemberPath = "CategoryName";
            List<Category> categories = null;
            categories = App.PasswordDB.GetCategories();
            cboCategory.ItemsSource = categories;
            if (e.Parameter != null)
            {
                _password = (Password)e.Parameter;
                if (_password != null)
                {
                    txtTitle.Text = _password.Title ?? "";
                    txtUserName.Text = _password.UserName ?? "";
                    txtPassword.Text = _password.Passcode ?? "";
                    txtKey.Text = _password.Key ?? "";
                    txtNote.Text = _password.Note??"";
                    txtWebSite.Text = _password.WebSite ?? "";
                    cboCategory.SelectedValue= categories.Where(c=>c.CategoryId == _password.CategoryId).First();
                    btnDelete.IsEnabled = true;
                }
            }
        }

        private void Save_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Password pwd;
            pwd = _password == null ? new Password() : _password;
            pwd.PasswordId = _password == null ? Guid.NewGuid() : _password.PasswordId;
            pwd.Title = txtTitle.Text;
            pwd.UserName = txtUserName.Text;
            pwd.Passcode = txtPassword.Text;
            pwd.Key = txtKey.Text;
            pwd.WebSite = txtWebSite.Text;
            pwd.Note = txtNote.Text;
            Category category = (Category)cboCategory.SelectedValue;
            pwd.CategoryId = category.CategoryId;            
            App.PasswordDB.SavePassword(pwd, _password == null ? true : false);            
            NagivateToMainPage();
        }

        private void NagivateToMainPage()
        {
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainPage)); 
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

private void Delete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
{             
    App.PasswordDB.DeletePassword(_password.PasswordId);             
    NagivateToMainPage();
}
    }
}
