namespace InvestixDev.Models
{
    public class StockInfoDetail // 현재가상세 요청
    {
        public decimal LastPrice { get; set; } // 현재가
        public decimal BasePrice { get; set; } // 전일종가

        public int MarketCap { get; set; } // 시가총액

        public int Capital {  get; set; } // 자본금

        public decimal PBR { get; set; }
        public decimal PER {  get; set; }
        public decimal EPS { get; set; }
        public decimal BPS { get; set; }

        public decimal ExchangeRate { get; set; } // 당일 거래 환율
    }
}
