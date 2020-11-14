using System.Collections.Generic;

namespace Covid19.Models
{
    public class CovidChart
    {
        public CovidChart()
        {
            Counts = new List<int>();
        }
        public string CovidDate { get; set; }
        public List<int> Counts { get; set; }
    }
}
