using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using InvestixDev.Repository.KIS;

namespace InvestixDev.Repository
{
    public class DB_Update
    {
        private readonly string _connectionString;

        public DB_Update(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task UpdateAllStocks()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync("delete from [Stocks]");

                await Task.WhenAll(
                    UpdateStockDataAsync("kospi"),
                    UpdateStockDataAsync("kosdaq"),
                    UpdateStockDataAsync("konex"),
                    UpdateStockDataAsync("oversea")
                );
            }
        }

        private async Task UpdateStockDataAsync(string marketCode)
        {
            Console.WriteLine($"{marketCode.ToUpper()} 정보 업데이트 중...");

            var inst = new DownloadStockCode();
            var dataTable = inst.GetStockCode(marketCode); // 반환형: DataTable

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = "Stocks"
            };

            await bulkCopy.WriteToServerAsync(dataTable);
        }
    }
}
