using System.Collections.ObjectModel;
using System.Linq;
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
    public sealed partial class CrewPage : Page
    {
        public ObservableCollection<CrewDTO> Crews { get; private set; }
        public ObservableCollection<StewardessDTO> Stewardesses { get; private set; }
        public ObservableCollection<PilotDTO> Pilots { get; private set; }
        private CrudService<StewardessDTO> StewardessService { get; set; }
        private CrudService<PilotDTO> PilotService { get; set; }
        private CrudService<CrewDTO> Service { get; set; }
        private CrewDTO _selectedCrew;
        private ObservableCollection<int> selectedIds { get; set; }
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
            selectedIds = new ObservableCollection<int>();
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
            var pilot = PilotCombo.SelectedItem as PilotDTO;
            if (StewardessCombo.SelectedItem == null)
            {
                Info.Text = "Info : select stewardess";
                return null;
            }
            if (selectedIds.Count == 0)
            {
                Info.Text = "Info : select stewardesses";
                return null;
            }
            if (pilot == null)
            {
                Info.Text = "Info : select pilot";
                return null;
            }
            
            return new CrewDTO { PilotId = pilot.Id ,StewardessesId = selectedIds.ToList()};
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedCrew = e.ClickedItem as CrewDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Crew Id : " + _selectedCrew?.Id;
                TbPilot.Text = "Pilot Id : " + _selectedCrew?.PilotId;
                TbStewardess.Text = "Stewardesses : " + StewardessesId(_selectedCrew);
            }
        }

        private string StewardessesId(CrewDTO crew)
        {
            var result = "count : " + crew.StewardessesId.Count + " (";
            crew.StewardessesId.ToList().ForEach(x => result += " Id:"+ x.ToString());
            result += " )";
            return result;
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
                    TbStewardess.Text = "Stewardesses : " + StewardessesId(item);
                }
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void BtnAddStew_OnClick(object sender, RoutedEventArgs e)
        {
            if (StewardessCombo.SelectedItem == null)
            {
                Info.Text = "Info : select stewardess";;
            }
            else
            {
                selectedIds.Add((StewardessCombo.SelectedItem as StewardessDTO).Id);
            }
        }
    }
}
