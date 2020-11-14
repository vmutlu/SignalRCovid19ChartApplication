using Covid19.Models;
using Covid19.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsTRController : ControllerBase
    {
        private readonly CovidService _covidService;
        public CovidsTRController(CovidService covidService)
        {
            _covidService = covidService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCovid(CovidClass covidClass)
        {
            await _covidService.SaveCovid(covidClass);
            IQueryable<CovidClass> covid = _covidService.GetList();

            return Ok(covid);
        }

        //rastgele 10 günlük covid degerleri uyduralım

        [HttpGet]
        public IActionResult InitializeCovid()
        {
            Random rn = new Random();
            Enumerable.Range(1, 10).ToList().ForEach(i =>
             {
                 foreach (ECity item in Enum.GetValues(typeof(ECity)))
                 {
                     var newCovid = new CovidClass()
                     {
                         City = item,
                         Count = rn.Next(100, 10000),
                         CovidDate = DateTime.Now.AddDays(i)
                     };

                     _covidService.SaveCovid(newCovid).Wait();
                     System.Threading.Thread.Sleep(1000);
                 }
             });

            return Ok();
        }
    }
}
