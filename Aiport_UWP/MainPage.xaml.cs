using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Aiport_UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private void Forward_Pilot_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PilotPage));
        }

        private void Forward_Ticket_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TicketPage));
        }
    }
}
