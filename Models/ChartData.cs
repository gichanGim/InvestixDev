namespace InvestixDev.Models
{
    public class ChartData
    {
        public decimal PriceHigh { get; set; }
        public decimal PriceLow { get; set; }

        public decimal PriceOpen { get; set; }
        public decimal PriceClose {  get; set; }

        public string ChangeSign { get; set; } // 전일 대비 부호
        public int Volume { get; set; } // 거래량
    }
}
