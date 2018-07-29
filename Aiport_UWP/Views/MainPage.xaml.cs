using Aiport_UWP.Views;
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

        private void Forward_Aircraft_Type_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TicketPage));
        }

        private void Forward_Aircraft_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AircraftPage));
        }

        private void Forward_Departure_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DeparturePage));
        }

        private void Forward_Flight_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FlightPage));
        }

        private void Forward_Crew_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CrewPage));
        }

        private void Forward_Stewardess_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StewardessPage));
        }
    }
}
