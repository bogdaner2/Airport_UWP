using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Aiport_UWP.DTO;
using Aiport_UWP.Services;


namespace Aiport_UWP
{
    public sealed partial class PilotPage : Page
    {
        public ObservableCollection<PilotDTO> Pilots { get; private set; }
        private PilotDTO _selectedPilot;
        private CrudService<PilotDTO> Service { get; set; }
        private bool isCreate;
        private int lastId; 

        public PilotPage()
        {
            this.InitializeComponent();
            Pilots = new ObservableCollection<PilotDTO>();
            Service = new CrudService<PilotDTO>("pilot");
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Pilots.Add(x));
            lastId = Pilots.LastOrDefault().Id;
        }

        private PilotDTO ReadTextBoxesData()
        {
            var firstName = InputFName.Text;
            var lastName = InputLName.Text;
            int.TryParse(InputExp.Text, out int exp);
            var birth = InputBirth.Text;
            Regex regex = new Regex(@"^([0-2][0-9]|(3)[0-1])(.)(((0)[0-9])|((1)[0-2]))(.)\d{4}$");
            MatchCollection matches = regex.Matches(birth);
            if (matches.Count == 0)
            {
                Info.Text = "Info : format dd.mm.yyyy";
                return null;
            }
            if (String.IsNullOrEmpty(firstName))
            {
                Info.Text = "Info : fill first name";
                return null;
            }
            if (firstName.Length < 4)
            {
                Info.Text = "Info : length have to be more than 4";
                return null;
            }
            if (String.IsNullOrEmpty(lastName))
            {
                Info.Text = "Info : fill last name";
                return null;
            }
            if (lastName.Length < 4)
            {
                Info.Text = "Info : length have to be more than 4";
                return null;
            }
            if (exp == 0 || exp > 50)
            {
                Info.Text = "Info : experience have to be between 1 and 50";
                return null;
            }
            return new PilotDTO { FirstName = firstName, LastName = lastName, DateOfBirth = birth, Experience = exp };
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedPilot = e.ClickedItem as PilotDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Pilot Id : " + _selectedPilot?.Id;
                TbFName.Text = "First name : " + _selectedPilot?.FirstName;
                TbLName.Text = "Last name : " + _selectedPilot?.LastName;
                TbBirth.Text = "Birth : " + _selectedPilot?.DateOfBirth;
                TbExp.Text = "Experience : " + _selectedPilot?.Experience;
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedPilot.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            Pilots.Remove(_selectedPilot);
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
                    Pilots.Add(pilot);
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
            TbFName.Text = "First name : " ;
            TbLName.Text = "Last name : ";
            TbBirth.Text = "Birth : " ;
            TbExp.Text = "Experience : ";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var pilotInput = ReadTextBoxesData();
                if (pilotInput != null && _selectedPilot != null)
                {
                    try
                    {
                        await Service.Update(pilotInput, _selectedPilot.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = Pilots.ToList().FindIndex(x => x.Id == _selectedPilot.Id);
                    var item = Pilots.ToList().ElementAt(itemIndex);
                    Pilots.RemoveAt(itemIndex);
                    item = pilotInput;
                    item.Id = _selectedPilot.Id;
                    Pilots.Insert(itemIndex, item);
                    TbId.Text = "Pilot Id :" + item.Id;
                    TbFName.Text = "First name : " + item.FirstName;
                    TbLName.Text = "Last name : " + item.LastName;
                    TbBirth.Text = "Birth : " + item.DateOfBirth;
                    TbExp.Text = "Experience : " + item.Experience;
                }
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
