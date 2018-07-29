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
        public ObservableCollection<TicketDTO> Ticketes { get; private set; }
        public ObservableCollection<PilotDTO> Pilots { get; private set; }
        private CrudService<TicketDTO> TicketService { get; set; }
        private CrudService<PilotDTO> PilotService { get; set; }
        private CrudService<FlightDTO> Service { get; set; }
        private FlightDTO _selectedFlight;
        private List<int> selectedIds { get; set; }
        private bool isCreate;
        private int lastId;

        public FlightPage()
        {
            this.InitializeComponent();
            Flights = new ObservableCollection<FlightDTO>();
            Ticketes = new ObservableCollection<TicketDTO>();
            TicketService = new CrudService<TicketDTO>("ticket");
            Service = new CrudService<FlightDTO>("flight");
            selectedIds = new List<int>();
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
            result.ForEach(x => Ticketes.Add(x));
        }


        private FlightDTO ReadTextBoxesData()
        {
            //// var number = InputNumber.Text;
            ////var departure = InputFlight.Text;
            //var crew = PilotCombo.SelectedItem as FlightDTO;
            ////var aircraft = PilotCombo.SelectedItem as PilotDTO;
            //Regex regex = new Regex(@"^([0-2][0-9]|(3)[0-1])(.)(((0)[0-9])|((1)[0-2]))(.)\d{4}$");
            ////MatchCollection matches = regex.Matches(departure);
            ////if (matches.Count == 0)
            ////{
            //    Info.Text = "Info : format dd.mm.yyyy";
            ////    return null;
            ////}
            //if (crew == null)
            //{
            //    Info.Text = "Info : select crew";
            //    return null;
            //}
            ////if (aircraft == null)
            //{
            //    Info.Text = "Info : select aircraft";
              return null;
            //}

           // return new FlightDTO { PilotId = 1, TicketesId = (new int[] { 1, 2 }).ToList() };
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //if (!isCreate)
            //{
            //    Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
            //    _selectedFlight = e.ClickedItem as FlightDTO;
            //    Canvas.Visibility = Visibility.Collapsed;
            //    TbId.Text = "Flight Id : " + _selectedFlight?.Id;
            //    TbPilot.Text = "Pilot Id : " + _selectedFlight?.PilotId; ;
            //    TbTicket.Text = "Ticketes :" + _selectedFlight?.TicketesId; ;
            //}
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
                var pilot = ReadTextBoxesData();
                if (pilot != null)
                {
                    try
                    {
                        await Service.Create(pilot);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }
                    lastId++;
                    pilot.Id = lastId;
                    Flights.Add(pilot);
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
            TbId.Text = "Input data";
            TbPilot.Text = "Pilot Id : ";
            //TbTicket.Text = "Ticketes :";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var pilotInput = ReadTextBoxesData();
                if (pilotInput != null && _selectedFlight != null)
                {
                    try
                    {
                        await Service.Update(pilotInput, _selectedFlight.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = Flights.ToList().FindIndex(x => x.Id == _selectedFlight.Id);
                    var item = Flights.ToList().ElementAt(itemIndex);
                    Flights.RemoveAt(itemIndex);
                    item = pilotInput;
                    item.Id = _selectedFlight.Id;
                    Flights.Insert(itemIndex, item);
                    TbId.Text = "Flight Id :" + item.Id;
                    //TbPilot.Text = "Pilot Id : " + item.PilotId;
                   // TbTicket.Text = "Ticketes :";
                }
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void BtnAddStew_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
