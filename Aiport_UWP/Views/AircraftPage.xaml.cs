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
    public sealed partial class AircraftPage : Page
    {
        public ObservableCollection<AircraftDTO> Aircrafts { get; private set; }
        public ObservableCollection<AircraftTypeDTO> Types { get; private set; }
        private CrudService<AircraftTypeDTO> TypesService { get; set; }
        private CrudService<AircraftDTO> Service { get; set; }
        private AircraftDTO _selectedAircraft;
        private bool isCreate;
        private int lastId;

        public AircraftPage()
        {
            this.InitializeComponent();
            Aircrafts = new ObservableCollection<AircraftDTO>();
            Types = new ObservableCollection<AircraftTypeDTO>();
            Service = new CrudService<AircraftDTO>("aircraft");
            TypesService = new CrudService<AircraftTypeDTO>("aircrafttype");
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Aircrafts.Add(x));
            lastId = Aircrafts.LastOrDefault().Id;
            LoadTypes();
        }

        private async void LoadTypes()
        {
            var result = await TypesService.GetAll();
            result.ForEach(x => Types.Add(x));
        }

        private AircraftDTO ReadTextBoxesData()
        {
            var name = InputName.Text;
            var release = InputRelease.Text;
            int.TryParse(InputLifetime.Text,out int lifetime);
            var type = TypesCombo.SelectedItem as AircraftTypeDTO;
            Regex regex = new Regex(@"^([0-2][0-9]|(3)[0-1])(.)(((0)[0-9])|((1)[0-2]))(.)\d{4}$");
            MatchCollection matches = regex.Matches(release);
            if (matches.Count == 0)
            {
                Info.Text = "Info : format dd.mm.yyyy";
                return null;
            }
            if (type == null)
            {
                Info.Text = "Info : select type";
                return null;
            }
            if (String.IsNullOrEmpty(name))
            {
                Info.Text = "Info : fill first name";
                return null;
            }
            if (name.Length < 4)
            {
                Info.Text = "Info : length have to be more than 4";
                return null;
            }
            if (lifetime == 0 || lifetime > 20)
            {
                Info.Text = "Info : lifetime have to be between 1 and 20";
                return null;
            }
            return new AircraftDTO { Name = name, TypeId = type.Id, ReleseDate = release, Lifetime = lifetime.ToString() };
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedAircraft = e.ClickedItem as AircraftDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Aircraft Id : " + _selectedAircraft?.Id;
                TbName.Text = "Name : " + _selectedAircraft?.Name;
                TbType.Text = "Type Id : " + _selectedAircraft?.TypeId;
                TbRelease.Text = "Release date : " + _selectedAircraft?.ReleseDate;
                TbLifetime.Text = "Lifetime : " + _selectedAircraft?.Lifetime;
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedAircraft.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            Aircrafts.Remove(_selectedAircraft);
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
                    Aircrafts.Add(pilot);
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
            TbName.Text = "Name : ";
            TbType.Text = "Type Id : ";
            TbRelease.Text = "Release date : ";
            TbLifetime.Text = "Lifetime : ";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!isCreate)
            {
                var pilotInput = ReadTextBoxesData();
                if (pilotInput != null && _selectedAircraft != null)
                {
                    try
                    {
                        await Service.Update(pilotInput, _selectedAircraft.Id);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }

                    var itemIndex = Aircrafts.ToList().FindIndex(x => x.Id == _selectedAircraft.Id);
                    var item = Aircrafts.ToList().ElementAt(itemIndex);
                    Aircrafts.RemoveAt(itemIndex);
                    item = pilotInput;
                    item.Id = _selectedAircraft.Id;
                    Aircrafts.Insert(itemIndex, item);
                    TbId.Text = "Aircraft Id :" + item.Id;
                    TbName.Text = "Name : " + item.Name;
                    TbType.Text = "Type  Id : " + item.TypeId;
                    TbRelease.Text = "Release date : " + item.ReleseDate;
                    TbLifetime.Text = "Lifetime : " + item.Lifetime;
                }
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
