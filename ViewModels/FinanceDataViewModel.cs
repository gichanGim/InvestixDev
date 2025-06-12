using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViewMyAssetDev.ViewModels
{   
    public class FinanceDataViewModel // 종목 상세보기 페이지
    {
        // ----------- 종목 정보 조회 -----------
        [Required]
        public string Symbol { get; set; }

        // FinanaceData에 표시할 이름 (전체 풀네임)
        // search
        [Required]
        public string LongName { get; set; }

        // 주식 거래소 ex) Nasdaq
        // search
        [Required]
        public string ExchDisp { get; set; }

        public string Domain { get; set; }

        // financial-data
        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal CurrentPrice
        { get; set; }

        // 나의 현재 보유 수량
        public int MyAmount { get; set; }

        // 해당 종목 통화 ex) USD
        // financial-data
        [Required]
        public string FinancialCurrency { get; set; }

        // ex) +1.3%
        public string PercentageChange { get; set; }

        // ex) + 3.30
        public string NetChange { get; set; }

        // deltaIndicator
        public string UpOrDown { get; set; }

        // Equity(주식), Futures(선물 옵션), Forex(환율), Indexs(지수), Mutual Fund(뮤추얼 펀드), ETF, Cryptocurrencies(가상 화폐), Combination(복합 옵션)
        public string Type { get; set; }


        // 후에 해당 종목 그래프 입력값, 제무제표, etc.. 추가 예정 


        //----------- 자산 추가 -----------

        // 구매 당시 가격
        [Required(ErrorMessage = "매수 가격을 입력해주세요")]
        public double PurchasedPrice { get; set; }


        // 구매 수량
        [Required(ErrorMessage = "매수 수량을 입력해주세요")]
        public int Amount { get; set; }
    }
}
