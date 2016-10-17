using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PasswordManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
public sealed partial class MainPage : PasswordManager.Common.LayoutAwarePage
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        LoadAllPasswords();
    }

    private void Refresh_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        LoadAllPasswords();
    }

    private void LoadAllPasswords()
    {           
        gvPasswords.ItemsSource = App.PasswordDB.GetAllPasswords();
    }

    private void Add_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {           
        var rootFrame = new Frame();
        rootFrame.Navigate(typeof(PasswordDetail)); 
        Window.Current.Content = rootFrame;
        Window.Current.Activate();
    }

    private void Edit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        var rootFrame = new Frame();
        rootFrame.Navigate(typeof(PasswordDetail), gvPasswords.SelectedValue);
        Window.Current.Content = rootFrame;
        Window.Current.Activate();
    }
}
}