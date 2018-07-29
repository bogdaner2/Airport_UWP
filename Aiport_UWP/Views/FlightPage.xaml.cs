using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Aiport_UWP.DTO;
using Aiport_UWP.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Aiport_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlightPage : Page
    {
        public ObservableCollection<FlightDTO> Flights { get; private set; }
        public ObservableCollection<TicketDTO> Tickets { get; private set; }
        public ObservableCollection<PilotDTO> Pilots { get; private set; }
        private CrudService<TicketDTO> TicketService { get; set; }
        private CrudService<PilotDTO> PilotService { get; set; }
        private CrudService<FlightDTO> Service { get; set; }
        private FlightDTO _selectedFlight { get; set; }
        private ObservableCollection<int> selectedIds { get; set; }
        private bool isCreate;
        private int lastId;

        public FlightPage()
        {
            this.InitializeComponent();
            Flights = new ObservableCollection<FlightDTO>();
            Tickets = new ObservableCollection<TicketDTO>();
            TicketService = new CrudService<TicketDTO>("ticket");
            Service = new CrudService<FlightDTO>("flight");
            selectedIds = new ObservableCollection<int>();
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Flights.Add(x));
            lastId = Flights.LastOrDefault().Id;
            LoadTicketes();
        }

        private async void LoadTicketes()
        {
            var result = await TicketService.GetAll();
            result.ForEach(x => Tickets.Add(x));
        }


        private FlightDTO ReadTextBoxesData()
        {
            Regex regex = new Regex(@"^([0-2][0-9]|(3)[0-1])(.)(((0)[0-9])|((1)[0-2]))(.)\d{4}$");
            var num = InputNumber.Text;
            var dep = InputDep.Text;
            var depTime = InputDepTime.Text;
            var dest = InputDest.Text;
            var arrTime = InputArrTime.Text;
            MatchCollection matchesDepTime = regex.Matches(depTime);
            MatchCollection matchesArrTime = regex.Matches(arrTime);
            if (matchesDepTime.Count == 0 || matchesArrTime.Count == 0)
            {
                Info.Text = "Info : format dd.mm.yyyy";
                return null;
            }
            if (String.IsNullOrEmpty(dep))
            {
                Info.Text = "Info : fill departure name";
                return null;
            }
            if (dep.Length < 3)
            {
                Info.Text = "Info : length have to be more than 3";
                return null;
            }
            if (String.IsNullOrEmpty(dest))
            {
                Info.Text = "Info : fill destination";
                return null;
            }
            if (dest.Length < 3)
            {
                Info.Text = "Info : length have to be more than 3";
                return null;
            }
            if (String.IsNullOrEmpty(num))
            {
                Info.Text = "Info : fill number";
                return null;
            }
            if (num.Length < 3)
            {
                Info.Text = "Info : length have to be more than 3";
                return null;
            }
            if (selectedIds.Count == 0)
            {
                Info.Text = "Info : select ticket";
                return null;
            }
            return new FlightDTO { Number = num,PointOfDeparture = dep,Destination = dest,DepartureTime = depTime,ArrivelTime = arrTime, TicketsId = selectedIds.ToList() };
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedFlight = e.ClickedItem as FlightDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Flight Id : " + _selectedFlight?.Id;
                TbNumber.Text = "Number : " + _selectedFlight?.Number;
                TbDep.Text = "Point of departure :" + _selectedFlight?.PointOfDeparture;
                TbDepTime.Text = "Departure time :" + _selectedFlight?.DepartureTime; ;
                TbDest.Text = "Destination :" + _selectedFlight?.Destination; ;
                TbArrTime.Text = "Arrival time :" + _selectedFlight?.ArrivelTime;
                TbTicket.Text = "Tickets " + TicketsId(_selectedFlight);
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedFlight.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            Flights.Remove(_selectedFlight);
        }

        private async void BtnCreate_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Collapsed;
            if (isCreate)
            {
                var flight = ReadTextBoxesData();
                if (flight != null)
                {
                    try
                    {
                        await Service.Create(flight);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }
                    lastId++;
                    flight.Id = lastId;
                    Flights.Add(flight);
                    isCreate = false;
                    CreateInfo();
                    Info.Text = "Choose new action!";
                }
            }
            else
            {
                CreateInfo();
                isCreate = true;
                Info.Text = "Info : Input data and press 'Create' ";
            }
        }

        private void CreateInfo()
        {
            TbId.Text = "Input data " ;
            TbNumber.Text = "Number : ";
            TbDep.Text = "Point of departure : ";
            TbDepTime.Text = "Departure time :" ;
            TbDest.Text = "Destination : ";
            TbArrTime.Text = "Arrival time :" ;
            TbTicket.Text = "Tickets : ";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var flight = ReadTextBoxesData();
                if (flight != null && _selectedFlight != null)
                {
                    try
                    {
                        await Service.Update(flight, _selectedFlight.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = Flights.ToList().FindIndex(x => x.Id == _selectedFlight.Id);
                    var item = Flights.ToList().ElementAt(itemIndex);
                    Flights.RemoveAt(itemIndex);
                    item = flight;
                    item.Id = _selectedFlight.Id;
                    Flights.Insert(itemIndex, item);
                    TbNumber.Text = "Number : " + item.Number;
                    TbDep.Text = "Point of departure :" + item.PointOfDeparture;
                    TbDepTime.Text = "Departure time :" + item.DepartureTime; 
                    TbDest.Text = "Destination :" + item.Destination; 
                    TbArrTime.Text = "Arrival time :" + item.ArrivelTime;
                    TbTicket.Text = "Tickets : " + TicketsId(item);
                }
            }
        }

        private string TicketsId(FlightDTO flight)
        {
            var result = "count : " + flight.TicketsId.Count + " (";
            flight.TicketsId.ToList().ForEach(x => result += " Id:" + x.ToString());
            result += " )";
            return result;
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void BtnAddTicket_OnClick(object sender, RoutedEventArgs e)
        {
            if (TicketCombo.SelectedItem == null)
            {
                Info.Text = "Info : select ticket"; ;
            }
            else
            {
                selectedIds.Add((TicketCombo.SelectedItem as TicketDTO).Id);
            }
        }
    }
}
