using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using Aiport_UWP.DTO;
using Aiport_UWP.Services;


namespace Aiport_UWP
{
    public sealed partial class TicketPage : Page
    {

        public ObservableCollection<TicketDTO> Tickets { get; private set; }
        private TicketDTO _selectedTicket;
        private CrudService<TicketDTO> Service { get; set; }
        private bool isCreate;
        private int lastId;

        public TicketPage()
        {
            this.InitializeComponent();
            Tickets = new ObservableCollection<TicketDTO>();
            Service = new CrudService<TicketDTO>("ticket");
            isCreate = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Tickets.Add(x));
            lastId = Tickets.LastOrDefault().Id;
        }

        private TicketDTO ReadTextBoxesData()
        {
            var number = InputNumber.Text;
            if(String.IsNullOrEmpty(number))
            {
                Info.Text = "Info : fill number";
                return null;
            }
            if(number.Length < 4)
            {
                Info.Text = "Info : length have to be more than 4";
                return null;
            }
            int.TryParse(InputPrice.Text,out int price);
            if(price == 0 || price > 10000)
            {
                Info.Text = "Info : price have to be between 1 and 10000$";
                return null;
            }
            return new TicketDTO {Number = number, Price = price};
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!isCreate)
            {
                Info.Text = "Info : Input data and 'Update' for update or 'Delete' for delete";
                _selectedTicket = e.ClickedItem as TicketDTO;
                Сanvas.Visibility = Visibility.Collapsed;
                TbId.Text = "Ticket Id : " + _selectedTicket?.Id;
                TbNumber.Text = "Number : " + _selectedTicket?.Number;
                TbPrice.Text = "Price : " + _selectedTicket?.Price + "$";
            }
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Сanvas.Visibility = Visibility.Visible;
            try
            { 
                await Service.Delete(_selectedTicket.Id);
            }
            catch
            {
                Info.Text = "Server error!";
            }

            Tickets.Remove(_selectedTicket);
        }

        private async void BtnCreate_OnClick(object sender, RoutedEventArgs e)
        {
            Сanvas.Visibility = Visibility.Collapsed;
            if (isCreate)
            {
                var ticket = ReadTextBoxesData();
                if (ticket != null)
                {
                    try
                    { 
                        await Service.Create(ticket);
                    }
                    catch
                    {
                        Info.Text = "Server error!";
                    }
                    lastId++;
                    ticket.Id = lastId;
                    Tickets.Add(ticket);
                    isCreate = false;
                    CreateInfo();
                    Info.Text = "Choose new action!";
                }
            }
            else {
                CreateInfo();
                isCreate = true;
                Info.Text = "Info : Input data and press 'Create' ";
            }
        }

        private void CreateInfo()
        {
            TbId.Text = "Input data";
            TbNumber.Text = "Number :";
            TbPrice.Text = "Price : ";
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            var ticketInput = ReadTextBoxesData();
            if (ticketInput != null && _selectedTicket != null)
            {
                try
                {
                    await Service.Update(ticketInput, _selectedTicket.Id);
                }
                catch
                {
                    Info.Text = "Server error!";
                }
                var itemIndex = Tickets.ToList().FindIndex(x => x.Id == _selectedTicket.Id);
                var item = Tickets.ToList().ElementAt(itemIndex);
                Tickets.RemoveAt(itemIndex);
                item = ticketInput;
                item.Id = _selectedTicket.Id;
                Tickets.Insert(itemIndex, item);
                TbId.Text = "Ticket Id :" + item.Id;
                TbNumber.Text = "Number :" + item.Number;
                TbPrice.Text = "Price : " + item.Price;
            }
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
