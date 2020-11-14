using Covid19.Data;
using Covid19.Hubs;
using Covid19.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19.Services
{
    public class CovidService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHubContext<CovidHub> _context;
        public CovidService(AppDbContext appDbContext, IHubContext<CovidHub> context)
        {
            _appDbContext = appDbContext;
            _context = context;
        }

        public IQueryable<CovidClass> GetList()
        {
            return _appDbContext.Covids.AsQueryable();
        }

        public async Task SaveCovid(CovidClass covid)
        {
            await _appDbContext.Covids.AddAsync(covid);
            await _appDbContext.SaveChangesAsync();
            await _context.Clients.All.SendAsync("ReceiveCovidList",GetCovidList());
        }

        //pivot sorgumla beraber chartın istediği şekilde verileri listeye atayayım
        public List<CovidChart> GetCovidList()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();

            using (var command = _appDbContext.Database.GetDbConnection().CreateCommand())
            {
                //pivot table sorgusu. db deki kayıtları charta uygun çekebilmek için 
                command.CommandText = "select tarih,[1],[2],[3],[4],[5] from (select [City],[Count],CAST([CovidDate] as date) as tarih from Covids) as covidT pivot (Sum(Count) for City In ([1],[2],[3],[4],[5])) as ptable order by tarih desc";

                command.CommandType = System.Data.CommandType.Text;

                _appDbContext.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart covid = new CovidChart();
                        covid.CovidDate = reader.GetDateTime(0).ToShortDateString();//db de 0. indexte oldugu için 

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                         {
                             if (System.DBNull.Value.Equals(reader[x])) //veritabanında boş kayıtlar null olarak gelmekte fakat veri tabanındaki null kavramı c# taki null ile eşit anlama gelmemektedir.
                                 covid.Counts.Add(0); //veritabanında null olan sutunlara 0 degeri ile göster yani chart alanımda null olarak değil 0 olarak gözükecek
                             else
                                 covid.Counts.Add(reader.GetInt32(x));
                         });

                        covidCharts.Add(covid);
                    }

                    _appDbContext.Database.CloseConnection();

                    return covidCharts;
                }
            }
        }
    }
}
