using System.Collections.ObjectModel;
using System.Linq;
using Aiport_UWP.DTO;
using Aiport_UWP.Services;

namespace Aiport_UWP.ViewModel
{
    public class PilotViewModel
    {
        public ObservableCollection<PilotDTO> Pilots { get; private set; }
        public PilotDTO _selectedPilot;
        private CrudService<PilotDTO> Service { get; set; }
        private int lastId;

        public PilotViewModel()
        {
            Pilots = new ObservableCollection<PilotDTO>();
            Service = new CrudService<PilotDTO>("pilot");
        }

        public async void LoadData()
        {
            var result = await Service.GetAll();
            result.ForEach(x => Pilots.Add(x));
            lastId = Pilots.LastOrDefault().Id;
        }

        public async void Delete()
        {
            await Service.Delete(_selectedPilot.Id);
            Pilots.Remove(_selectedPilot);
        }

        public async void Create(PilotDTO pilot)
        {
            await Service.Create(pilot);
            lastId++;
            pilot.Id = lastId;
            Pilots.Add(pilot);
        }

        //public async void Update()
        //{
        //    await Service.Update(pilot, _selectedPilot.Id);
        //    var itemIndex = Pilots.ToList().FindIndex(x => x.Id == _selectedPilot.Id);
        //    var item = Pilots.ToList().ElementAt(itemIndex);
        //    Pilots.RemoveAt(itemIndex);
        //    item = pilot;
        //    item.Id = _selectedPilot.Id;
        //    Pilots.Insert(itemIndex, item);
        //}
    }
}
