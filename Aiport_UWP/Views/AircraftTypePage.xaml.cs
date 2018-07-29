using System;
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
    public sealed partial class AircraftTypePage : Page
    {
        public ObservableCollection<AircraftTypeDTO> AircraftTypes { get; private set; }
        private AircraftTypeDTO _selectedAircraftType;
        private CrudService<AircraftTypeDTO> Service { get; set; }
        private bool isCreate;
        private int lastId;

        public AircraftTypePage()
        {
            this.InitializeComponent();
            AircraftTypes = new ObservableCollection<AircraftTypeDTO>();
            Service = new CrudService<AircraftTypeDTO>("aircrafttype");
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => AircraftTypes.Add(x));
            lastId = AircraftTypes.LastOrDefault().Id;
        }

        private AircraftTypeDTO ReadTextBoxesData()
        {
            var model = InputModel.Text;
            int.TryParse(InputSeats.Text,out int countOfSeats);
            int.TryParse(InputCarryingCapacity.Text, out int carryingCapacity);
            if (String.IsNullOrEmpty(model))
            {
                Info.Text = "Info : fill ьщвуд";
                return null;
            }
            if (model.Length < 4)
            {
                Info.Text = "Info : length have to be more than 4";
                return null;
            }
            if (countOfSeats < 10 || countOfSeats > 1000)
            {
                Info.Text = "Info : Count of seats have to be between 10and 1000";
                return null;
            }
            if (carryingCapacity < 1000 || carryingCapacity > 1000000)
            {
                Info.Text = "Info : Capacity have to be between 1000 and 1000000";
                return null;
            }
            return new AircraftTypeDTO { Model = model, CountOfSeats = countOfSeats, CarryingCapacity = carryingCapacity };
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedAircraftType = e.ClickedItem as AircraftTypeDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "AircraftType Id : " + _selectedAircraftType?.Id;
                TbModel.Text = "Model: " + _selectedAircraftType?.Model;
                TbSeats.Text = " Count of seats : " + _selectedAircraftType?.CountOfSeats;
                TbCapacity.Text = "CarryingCapacity : " + _selectedAircraftType?.CarryingCapacity;
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedAircraftType.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            AircraftTypes.Remove(_selectedAircraftType);
        }

        private async void BtnCreate_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Collapsed;
            if (isCreate)
            {
                var stewardess = ReadTextBoxesData();
                if (stewardess != null)
                {
                    try
                    {
                        await Service.Create(stewardess);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }
                    lastId++;
                    stewardess.Id = lastId;
                    AircraftTypes.Add(stewardess);
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
            TbModel.Text = "Model: ";
            TbSeats.Text = " Count of seats : ";
            TbCapacity.Text = "CarryingCapacity : ";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var stewardessInput = ReadTextBoxesData();
                if (stewardessInput != null && _selectedAircraftType != null)
                {
                    try
                    {
                        await Service.Update(stewardessInput, _selectedAircraftType.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = AircraftTypes.ToList().FindIndex(x => x.Id == _selectedAircraftType.Id);
                    var item = AircraftTypes.ToList().ElementAt(itemIndex);
                    AircraftTypes.RemoveAt(itemIndex);
                    item = stewardessInput;
                    item.Id = _selectedAircraftType.Id;
                    AircraftTypes.Insert(itemIndex, item);
                    TbId.Text = "AircraftType Id :" + item.Id;
                    TbModel.Text = "Model : " + item.Model;
                    TbSeats.Text = " Count of seats : " + item.CountOfSeats;
                    TbCapacity.Text = "CarryingCapacity : " + item.CarryingCapacity;
                }
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
