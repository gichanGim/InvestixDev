using InvestixDev.Models;
using InvestixDev.Repository.DB;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InvestixDev.Repository.KIS
{
    public class APIRequest
    {
        private readonly string _apiKey;    
        private readonly string _apiKeySecret;
        private readonly string _token;
        private readonly string _prod = "https://openapi.koreainvestment.com:9443";
        private readonly int _cano; // 실물 계좌번호
        private readonly HttpClient client = new();

        public APIRequest(string apiKey, string apiKeySecret, string token)
        {
            _apiKey = apiKey;
            _apiKeySecret = apiKeySecret;
            _token = token;
        }

         // 손익계산서 정보 호출
        public async Task<List<IncomeStatement>> GetIncomeStatement(string symbol, string exchange)
        {
            if (exchange != "코스피" && exchange != "코스닥" && exchange != "코넥스")       // 해외 주식
            {
                List<IncomeStatement> incomeStatements = await IncomeStatement_OverSea(symbol, exchange);

                return incomeStatements;
            }
            else                                                                            // 국내 주식
            {
                List<IncomeStatement> incomeStatements = await IncomeStatement_KR(symbol);

                return incomeStatements;
            }
        }

        private async Task<List<IncomeStatement>> IncomeStatement_KR(string symbol)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_prod}/uapi/domestic-stock/v1/finance/income-statement?fid_cond_mrkt_div_code=J&fid_input_iscd={symbol}&fid_div_cls_code=1"),
                Headers =
                {
                    { "content-type", "application/json" },
                    { "authorization", $"Bearer {_token}" },
                    { "appkey", _apiKey },
                    { "appsecret", _apiKeySecret },
                    { "tr_id", "FHKST66430200" },
                    { "custtype", "P" }
                }
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var jArray = (JArray)json;

                List<IncomeStatement> list = new List<IncomeStatement>();

                foreach (var j in jArray)
                {
                    list.Add(new IncomeStatement
                    {
                        Date = j["stac_yymm"]?.ToString(),
                        SaleAccount = Convert.ToDecimal(j["sale_account"]),
                        SaleTotalProfit = Convert.ToDecimal(j["sale_totl_prfi"]),
                        OperatingProfit = Convert.ToDecimal(j["bsop_prti"]),
                        NetIncome = Convert.ToDecimal(j["thtr_ntin"]),
                        Depreciation = Convert.ToDecimal(j["depr_cost"]),
                        SellAndManagement = Convert.ToDecimal(j["sell_mang"])
                    });
                }
                return list;
            }
        }

        // 해외주식의 손익계산서는 타 API를 통해 가져옴
        private async Task<List<IncomeStatement>> IncomeStatement_OverSea(string symbol, string currency)
        {
            return null;
        }

        //public async Task<List<ChartData>> GetChartData(string symbol, string exchange)
        //{
        //    if (exchange != "코스피" && exchange != "코스닥" && exchange != "코넥스")       // 해외 주식
        //    {
        //        List<ChartData> chartDatas = await ChartData_OverSea(symbol, exchange);
        //
        //        return chartDatas;
        //    }
        //    else                                                                            // 국내 주식
        //    {
        //        List<ChartData> chartDatas = await ChartData_KR(symbol);
        //
        //        return chartDatas; 
        //    }
        //}
        //
        //private async Task<List<ChartData>> ChartData_KR(string symbol)
        //{
        //    
        //}
        //
        //private async Task<List<ChartData>> ChartData_OverSea(string symbol, string exchange)
        //{
        //    return null;
        //}
        

        public async Task<string> SendOrder(string symbol, string exchange, bool buy, string orderType, int amount, decimal orderPrice)
        {
            JObject jObject = new JObject();

            if (exchange != "코스피" && exchange != "코스닥" && exchange != "코넥스")
                jObject = await Order_OverSea(symbol, exchange, buy, orderType, amount, orderPrice);
            
            else
                jObject = await Order_KR(symbol, buy, orderType, amount, orderPrice);

            string successCode = jObject["rt_cd"]?.ToString();
            string responseMsg = jObject["msg1"]?.ToString();

            if (successCode == "0")
                return "주문 처리 성공";
            else
                return responseMsg;
        }

        private async Task<JObject> Order_KR(string symbol, bool buy, string orderType, int amount, decimal orderPrice = 0)
        {
            string trId = buy ? "TTTC0802U" : "TTTC0801U";

            var content = new Dictionary<string, string>
            {
                ["CANO"] = _cano.ToString(),
                ["ACNT_PRDT_CD"] = "01",
                ["PDNO"] = symbol,
                ["ORD_DVSN"] = orderType,
                ["ORD_QTY"] = amount.ToString(),
                ["ORD_UNPR"] = orderPrice.ToString()
            };

            if (orderType == "22")
                content["CNDT_PRIC"] = orderPrice.ToString();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_prod}/uapi/domestic-stock/v1/trading/order-cash"),
                Headers =
                {
                    { "authorization", $"Bearer {_token}" },
                    { "appkey", _apiKey },
                    { "appsecret", _apiKeySecret },
                    { "tr_id", trId }
                },
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
            };

            using (var response = await client.SendAsync(request))
            {
                var json = await response.Content.ReadAsStringAsync();
                return JObject.Parse(json);
            }
        }

        private async Task<JObject> Order_OverSea(string symbol, string exchange, bool buy, string orderType, int amount, decimal orderPrice = 0)
        {
            string trId = "";
            
            if (buy)
            {
                trId = exchange switch
                {
                    "나스닥" or "뉴욕" or "아멕스" => "TTTT1002U",
                    "도쿄" => "TTTS0308U",
                    "상해" or "상해지수" => "TTTS0202U",
                    "심천" or "심천지수" => "TTTS0305U",
                    "홍콩" => "TTTS1002U",
                    "하노이" or "호치민" => "TTTS0311U",
                    _ => ""
                };
            }
            else
            {
                trId = exchange switch
                {
                    "나스닥" or "뉴욕" or "아멕스" => "TTTT1006U",
                    "도쿄" => "TTTS0307U",
                    "상해" or "상해지수" => "TTTS1005U",
                    "심천" or "심천지수" => "TTTS0304U",
                    "홍콩" => "TTTS1001U",
                    "하노이" or "호치민" => "TTTS0310U",
                    _ => ""
                };
            }

            exchange = exchange switch
            {
                "나스닥" => "NASD",
                "뉴욕" => "NYSE",
                "아멕스" => "AMEX",
                "도쿄" => "TKSE",
                "홍콩" => "SEHK",
                "상해" or "상해지수" => "SHAA",
                "심천" or "심천지수" => "SZAA",
                "하노이" => "HASE",
                "호치민" => "VNSE",
                _ => ""
            };
            
            var content = new Dictionary<string, string>
            {
                ["CANO"] = _cano.ToString(),      // 실전 계좌번호
                ["OVRS_EXCG_CD"] = exchange,
                ["ACNT_PRDT_CD"] = "01",    // 상품 유형 코드( 01 : 국내/해외주식, 03 : 국내선물/옵션, 08 : 해외선물/옵션)
                ["PDNO"] = symbol,        // 종목 코드
                ["ORD_DVSN"] = orderType,        // 주문 유형
                ["ORD_QTY"] = amount.ToString(),         // 주문 수량
                ["OVRS_ORD_UNPR"] = orderPrice.ToString(),     // 주문 단가(0 : 시장가)
                ["ORD_SVR_DVSN_CD"] = "0" // 주문 서버 구분 코드 (0 default)
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_prod}/uapi/overseas-stock/v1/trading/order"),
                Headers =
                {
                    { "authorization", $"Bearer {_token}" },
                    { "appkey", _apiKey },
                    { "appsecret", _apiKeySecret },
                    { "tr_id", trId }
                },
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
            };

            using (var response = await client.SendAsync(request))
            {
                var json = await response.Content.ReadAsStringAsync();
                return JObject.Parse(json);
            }
        }
    }
}
