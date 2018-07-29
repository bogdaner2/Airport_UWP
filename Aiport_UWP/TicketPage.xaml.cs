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
using Aiport_UWP.ViewModels;


namespace Aiport_UWP
{
    public sealed partial class TicketPage : Page
    {

        public ObservableCollection<TicketDTO> Tickets { get; private set; }
        private TicketDTO _selectedTicket;
        private CrudService<TicketDTO> Service { get; set; }

        public TicketPage()
        {
            this.InitializeComponent();
            Tickets = new ObservableCollection<TicketDTO>();
            Service = new CrudService<TicketDTO>("ticket");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await Service.GetAll();
            result.ForEach(x => Tickets.Add(x));
        }

        private TicketDTO ReadTextBoxesData()
        {
            var number = InputNumber.Text;
            int.TryParse(InputPrice.Text,out int price);
            return new TicketDTO {Number = number, Price = price};
        }

        private void Lv_OnItemClick(object sender, ItemClickEventArgs e)
        {
            _selectedTicket = e.ClickedItem as TicketDTO;
            canvas.Visibility = Visibility.Collapsed;
            TbId.Text = "Ticket Id : " + _selectedTicket?.Id;
            TbNumber.Text = "Number : " + _selectedTicket?.Number;
            TbPrice.Text = "Price : " + _selectedTicket?.Price +"$";
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            await Service.Delete(_selectedTicket.Id);
            Tickets.Remove(_selectedTicket);
        }

        private async void BtnCreate_OnClick(object sender, RoutedEventArgs e)
        {
            var ticket = ReadTextBoxesData();
            await Service.Create(ticket);
            Tickets.Add(ticket);
        }

        private async void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            var ticketInput = ReadTextBoxesData();
            await Service.UpdateTicket(ticketInput, _selectedTicket.Id);
            var itemIndex = Tickets.ToList().FindIndex(x => x.Id == _selectedTicket.Id);
            var item = Tickets.ToList().ElementAt(itemIndex);
            Tickets.RemoveAt(itemIndex);
            item = ticketInput;
            item.Id = _selectedTicket.Id;
            Tickets.Insert(itemIndex, item);
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
