using Covid19.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Covid19.Hubs
{
    public class CovidHub : Hub
    {
        private readonly CovidService _covidService;
        public CovidHub(CovidService covidService)
        {
            _covidService = covidService;
        }
        public async Task GetCovidList()
        {
            await Clients.All.SendAsync("ReceiveCovidList",_covidService.GetCovidList()); //pivot sorgumun bulundugu metodu çalıştır ve receivecovidlist metodumu dinleyen clientler için bunu gönder
        }
    }
}
