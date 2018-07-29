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
    public sealed partial class CrewPage : Page
    {
        public ObservableCollection<CrewDTO> Crews { get; private set; }
        public ObservableCollection<StewardessDTO> Stewardesses { get; private set; }
        public ObservableCollection<PilotDTO> Pilots { get; private set; }
        private CrudService<StewardessDTO> StewardessService { get; set; }
        private CrudService<PilotDTO> PilotService { get; set; }
        private CrudService<CrewDTO> Service { get; set; }
        private CrewDTO _selectedCrew;
        private List<int> selectedIds { get; set; }
        private bool isCreate;
        private int lastId;

        public CrewPage()
        {
            this.InitializeComponent();
            Crews = new ObservableCollection<CrewDTO>();
            Stewardesses = new ObservableCollection<StewardessDTO>();
            Pilots = new ObservableCollection<PilotDTO>();
            PilotService = new CrudService<PilotDTO>("pilot");
            StewardessService = new CrudService<StewardessDTO>("stewardess");
            Service = new CrudService<CrewDTO>("crew");
            selectedIds = new List<int>();
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Crews.Add(x));
            lastId = Crews.LastOrDefault().Id;
            LoadStewardesses();
            LoadPilots();
        }

        private async void LoadStewardesses()
        {
            var result = await StewardessService.GetAll();
            result.ForEach(x => Stewardesses.Add(x));
        }

        private async void LoadPilots()
        {
            var result = await PilotService.GetAll();
            result.ForEach(x => Pilots.Add(x));
        }

        private CrewDTO ReadTextBoxesData()
        {
           // var number = InputNumber.Text;
            //var departure = InputCrew.Text;
            var crew = PilotCombo.SelectedItem as CrewDTO;
            //var aircraft = PilotCombo.SelectedItem as PilotDTO;
            Regex regex = new Regex(@"^([0-2][0-9]|(3)[0-1])(.)(((0)[0-9])|((1)[0-2]))(.)\d{4}$");
            //MatchCollection matches = regex.Matches(departure);
            //if (matches.Count == 0)
            //{
            //    Info.Text = "Info : format dd.mm.yyyy";
            //    return null;
            //}
            if (crew == null)
            {
                Info.Text = "Info : select crew";
                return null;
            }
            //if (aircraft == null)
            //{
            //    Info.Text = "Info : select aircraft";
            //    return null;
            //}
            
            return new CrewDTO { PilotId = 1 ,StewardessesId = (new int[] { 1, 2}).ToList()};
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedCrew = e.ClickedItem as CrewDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Crew Id : " + _selectedCrew?.Id;
                TbPilot.Text = "Pilot Id : " + _selectedCrew?.PilotId; ;
                TbStewardess.Text = "Stewardesses :" + _selectedCrew?.StewardessesId; ;
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedCrew.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            Crews.Remove(_selectedCrew);
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
                    Crews.Add(pilot);
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
            TbStewardess.Text = "Stewardesses :";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var pilotInput = ReadTextBoxesData();
                if (pilotInput != null && _selectedCrew != null)
                {
                    try
                    {
                        await Service.Update(pilotInput, _selectedCrew.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = Crews.ToList().FindIndex(x => x.Id == _selectedCrew.Id);
                    var item = Crews.ToList().ElementAt(itemIndex);
                    Crews.RemoveAt(itemIndex);
                    item = pilotInput;
                    item.Id = _selectedCrew.Id;
                    Crews.Insert(itemIndex, item);
                    TbId.Text = "Crew Id :" + item.Id;
                    TbPilot.Text = "Pilot Id : " + item.PilotId;
                    TbStewardess.Text = "Stewardesses :";
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
