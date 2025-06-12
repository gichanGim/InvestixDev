using System.IO.Compression;
using System.IO;
using System.Net;
using System.Text;
using System.Data;

namespace InvestixDev.Repository.KIS
{
    public class DownloadStockCode
    {
        private string _baseDir = Directory.GetCurrentDirectory();
        private string _url = "";
        private DataTable _dt = new DataTable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns>[0]: Symbol, [1]: Currency, [2]: Name, [3]: Exchange, [4]: Type</returns>
        public DataTable GetStockCode(string code)
        {
            string codeZip = "";
            _dt.Columns.AddRange(new[]
            {
                new DataColumn("Symbol", typeof(string)),
                new DataColumn("Currency", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Exchange", typeof(string)),
                new DataColumn("Type", typeof(string)),
            });

            if (code == "kospi" || code == "kosdaq" || code == "konex") // 국내 주식
            {
                _url = $"https://new.real.download.dws.co.kr/common/master/{code}_code.mst.zip";
                codeZip = $"{code}_code.zip";

                DownloadAndExtractFile(_url, codeZip);
                ProccessMstFIle(code);

                return _dt;
            }
            else if (code == "oversea")                                // 해외 주식
            {
                string[] marketCodes = { "nas", "nys", "ams", "shs", "shi", "szs", "szi", "tse", "hks", "hnx", "hsx" };

                foreach (var cd in marketCodes)
                {
                    _url = $"https://new.real.download.dws.co.kr/common/master/{cd}mst.cod.zip";
                    codeZip = $"{cd}mst.code.zip";

                    DownloadAndExtractFile(_url, codeZip);
                    ProccessMstFIle(cd);
                }
            }
            else
            {
                Console.WriteLine("GetStockCode 입력 파라미터 오류");
                return _dt;
            }

            return _dt;
        }

        private void DownloadAndExtractFile(string url, string zipFile)
        {
            string zipPath = Path.Combine(_baseDir, zipFile);

            using (var client = new WebClient())
            {
                Console.WriteLine($"{zipFile} 다운로드 중...");
                client.DownloadFile(url, zipPath);
            }

            ZipFile.ExtractToDirectory(zipPath, _baseDir, true);
            File.Delete(zipPath);
        }

        private void ProccessMstFIle(string code)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            if (code == "kospi")
            {
                string mstFile = Path.Combine(_baseDir, "kospi_code.mst");
                string[] lines = File.ReadAllLines(mstFile, Encoding.GetEncoding(949));

                foreach (var line in lines)
                {
                    List<string> data = new List<string>();

                    string part = line.Substring(0, line.Length - 228);

                    string symbol = part.Substring(0, 9).Trim();
                    string currency = "KRW";
                    string name = part.Substring(21).Trim();
                    string exchange = "코스피";
                    string type = "Stock";

                    _dt.Rows.Add(symbol, currency, name, exchange, type);
                }

                return;
            }
            else if (code == "kosdaq")
            {
                string mstFile = Path.Combine(_baseDir, "kosdaq_code.mst");
                string[] lines = File.ReadAllLines(mstFile, Encoding.GetEncoding("ks_c_5601-1987")); // CP949

                foreach (var line in lines)
                {
                    List<string> data = new List<string>();

                    string symbol = line.Substring(0, 9).Trim();
                    string currency = "KRW";
                    string name = line.Substring(21, line.Length - 21 - 222).Trim();
                    string exchange = "코스닥";
                    string type = "Stock";

                    _dt.Rows.Add(symbol, currency, name, exchange, type);
                }

                return;
            }
            else if (code == "konex")
            {
                string mstFile = Path.Combine(_baseDir, "konex_code.mst");
                string[] lines = File.ReadAllLines(mstFile, Encoding.GetEncoding("ks_c_5601-1987")); // CP949

                foreach (var line in lines)
                {
                    List<string> data = new List<string>();

                    string symbol = line.Substring(0, 9).Trim();
                    string currency = "KRW";
                    string name = line.Substring(21, line.Length - 184 - 22).Trim();
                    string exchange = "코넥스";
                    string type = "Stock";

                    _dt.Rows.Add(symbol, currency, name, exchange, type);
                }
                return;
            }
            else // 해외 주식
            {
                string mstFile = Path.Combine(_baseDir, $"{code}mst.cod");
                string[] lines = File.ReadAllLines(mstFile, Encoding.GetEncoding("ks_c_5601-1987")); // CP949

                foreach (var line in lines)
                {
                    List<string> data = new List<string>();

                    var values = line.Split('\t');

                    if (values.Length == 24)
                    {
                        string symbol = values[4];
                        string currency = values[9];
                        string name = values[6];
                        string exchange = values[3];

                        string type = values[8] switch
                        {
                            "1" => "Index",
                            "2" => "Stock",
                            "3" => "ETF",
                            "4" => "Warrant",
                            _ => "Unknown"
                        };

                        _dt.Rows.Add(symbol, currency, name, exchange, type);
                    }
                }

                return;
            }
        }
    }
}
