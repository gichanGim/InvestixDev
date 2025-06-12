using InvestixDev.Models;

namespace InvestixDev.ViewModels
{
    public class StockInfoViewModel
    {
        public UserAPIKey UserAPIKey { get; set; }

        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Currency {  get; set; }
        public string Exchange {  get; set; }
        public string Type { get; set; }
        public List<ChartData> ChartData {  get; set; }
    }
}
