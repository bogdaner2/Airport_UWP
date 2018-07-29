using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Aiport_UWP.DTO;
using Aiport_UWP.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Aiport_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeparturePage : Page
    {
        public ObservableCollection<DeparturesDTO> Departures { get; private set; }
        public ObservableCollection<CrewDTO> Crews { get; private set; }
        public ObservableCollection<AircraftDTO> Aircrafts { get; private set; }
        private CrudService<CrewDTO> CrewService { get; set; }
        private CrudService<AircraftDTO> AircraftService { get; set; }
        private CrudService<DeparturesDTO> Service { get; set; }
        private DeparturesDTO _selectedDeparture;
        private bool isCreate;
        private int lastId;

        public DeparturePage()
        {
            this.InitializeComponent();
            Departures = new ObservableCollection<DeparturesDTO>();
            Crews = new ObservableCollection<CrewDTO>();
            Aircrafts = new ObservableCollection<AircraftDTO>();
            AircraftService = new CrudService<AircraftDTO>("aircraft");
            CrewService = new CrudService<CrewDTO>("crew");
            Service = new CrudService<DeparturesDTO>("departure");
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Departures.Add(x));
            lastId = Departures.LastOrDefault().Id;
            LoadCrews();
            LoadAircrafts();
        }

        private async void LoadCrews()
        {
            var result = await CrewService.GetAll();
            result.ForEach(x => Crews.Add(x));
        }

        private async void LoadAircrafts()
        {
            var result = await AircraftService.GetAll();
            result.ForEach(x => Aircrafts.Add(x));
        }

        private DeparturesDTO ReadTextBoxesData()
        {
            var number = InputNumber.Text;
            var departure = InputDeparture.Text;
            var crew = CrewCombo.SelectedItem as CrewDTO;
            var aircraft = AircraftCombo.SelectedItem as AircraftDTO;
            Regex regex = new Regex(@"^([0-2][0-9]|(3)[0-1])(.)(((0)[0-9])|((1)[0-2]))(.)\d{4}$");
            MatchCollection matches = regex.Matches(departure);
            if (matches.Count == 0)
            {
                Info.Text = "Info : format dd.mm.yyyy";
                return null;
            }
            if (crew == null)
            {
                Info.Text = "Info : select crew";
                return null;
            }
            if (aircraft == null)
            {
                Info.Text = "Info : select aircraft";
                return null;
            }
            if (String.IsNullOrEmpty(number))
            {
                Info.Text = "Info : fill number";
                return null;
            }
            if (number.Length < 4)
            {
                Info.Text = "Info : length have to be more than 4";
                return null;
            }
            return new DeparturesDTO { Number = number, DepartureTime = departure , AircraftId  = aircraft.Id, CrewId = crew.Id };
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedDeparture = e.ClickedItem as DeparturesDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Departure Id : " + _selectedDeparture?.Id;
                TbNumber.Text = "Number :" + _selectedDeparture?.Number;
                TbDeparture.Text = "Departure time : " + _selectedDeparture?.DepartureTime;
                TbCrew.Text = "Crew Id :" + _selectedDeparture?.CrewId;
                TbAircraft.Text = "Aircraft Id :" + _selectedDeparture?.AircraftId;
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedDeparture.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            Departures.Remove(_selectedDeparture);
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
                    Departures.Add(pilot);
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
            TbNumber.Text = "Number :";
            TbDeparture.Text = "Departure time : ";
            TbCrew.Text = "Crew Id :";
            TbAircraft.Text = "Aircraft Id :";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var pilotInput = ReadTextBoxesData();
                if (pilotInput != null && _selectedDeparture != null)
                {
                    try
                    {
                        await Service.Update(pilotInput, _selectedDeparture.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = Departures.ToList().FindIndex(x => x.Id == _selectedDeparture.Id);
                    var item = Departures.ToList().ElementAt(itemIndex);
                    Departures.RemoveAt(itemIndex);
                    item = pilotInput;
                    item.Id = _selectedDeparture.Id;
                    Departures.Insert(itemIndex, item);
                    TbId.Text = "Departure Id :" + item.Id;
                    TbNumber.Text = "Number :" + item.Number;
                    TbDeparture.Text = "Departure time : " + item.DepartureTime;
                    TbCrew.Text = "Crew Id :" + item.CrewId;
                    TbAircraft.Text = "Aircraft Id :" + item.AircraftId;
                }
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
