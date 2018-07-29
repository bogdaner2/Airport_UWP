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
    public sealed partial class StewardessPage : Page
    {
        public ObservableCollection<StewardessDTO> Stewardesses { get; private set; }
        private StewardessDTO _selectedStewardess;
        private CrudService<StewardessDTO> Service { get; set; }
        private bool isCreate;
        private int lastId;

        public StewardessPage()
        {
            this.InitializeComponent();
            Stewardesses = new ObservableCollection<StewardessDTO>();
            Service = new CrudService<StewardessDTO>("stewardess");
            isCreate = false;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Stewardesses.Add(x));
            lastId = Stewardesses.LastOrDefault().Id;
        }

        private StewardessDTO ReadTextBoxesData()
        {
            var firstName = InputFName.Text;
            var lastName = InputLName.Text;
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
            return new StewardessDTO { FirstName = firstName, LastName = lastName, DateOfBirth = birth};
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedStewardess = e.ClickedItem as StewardessDTO;
                Canvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Stewardess Id : " + _selectedStewardess?.Id;
                TbFName.Text = "First name : " + _selectedStewardess?.FirstName;
                TbLName.Text = "Last name : " + _selectedStewardess?.LastName;
                TbBirth.Text = "Birth : " + _selectedStewardess?.DateOfBirth;
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Canvas.Visibility = Visibility.Visible;
            try
            {
                await Service.Delete(_selectedStewardess.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }
            Stewardesses.Remove(_selectedStewardess);
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
                    Stewardesses.Add(stewardess);
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
            TbFName.Text = "First name : ";
            TbLName.Text = "Last name : ";
            TbBirth.Text = "Birth : ";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            var stewardessInput = ReadTextBoxesData();
            if (stewardessInput != null && _selectedStewardess != null)
            {
                try
                {
                    await Service.Update(stewardessInput, _selectedStewardess.Id);
                }
                catch
                {
                    Info.Text = "Server error!";
                }
                var itemIndex = Stewardesses.ToList().FindIndex(x => x.Id == _selectedStewardess.Id);
                var item = Stewardesses.ToList().ElementAt(itemIndex);
                Stewardesses.RemoveAt(itemIndex);
                item = stewardessInput;
                item.Id = _selectedStewardess.Id;
                Stewardesses.Insert(itemIndex, item);
                TbId.Text = "Stewardess Id :" + item.Id;
                TbFName.Text = "First name : " + item.FirstName;
                TbLName.Text = "Last name : " + item.LastName;
                TbBirth.Text = "Birth : " + item.DateOfBirth;
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
