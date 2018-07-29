using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiport_UWP.DTO;
using Aiport_UWP.Services;

namespace Aiport_UWP.ViewModels
{
    public class PilotViewModel : BaseViewModel
    {
        public PilotService service { get; set; }
        public PilotViewModel()
        {
            service = new PilotService();
            Pilots = new ObservableCollection<PilotDTO>();
        }

        public ObservableCollection<PilotDTO> Pilots { get; set; }

        private async void LoadData()
        {
            var result = await service.getAllPilots();
            result.ForEach(x => Pilots.Add(x));
        }

    }
}
