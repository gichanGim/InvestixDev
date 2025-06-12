namespace InvestixDev.Models
{
    public class IncomeStatement
    {
        // 해당 분기
        public string Date { get; set; } // yyyyMM
        
        public decimal? SaleAccount { get; set; } // 매출액
                      
        public decimal? SaleTotalProfit {  get; set; } // 매출 총 이익
                      
        public decimal? OperatingProfit { get; set; } // 영업이익
                      
        public decimal? NetIncome { get; set; } // 당기순이익
                      
        public decimal? Depreciation { get; set; } // 감가상각비
                      
        public decimal? SellAndManagement { get; set; } // 판매 및 관리비
    }
}
