using Dapper;
using InvestixDev.Models;
using Microsoft.Data.SqlClient;

namespace InvestixDev.Repository.DB
{
    public class DB_User
    {
        private readonly string _connectionString;

        public DB_User(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task ModifyAppKey(string userName, string appkey)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = $"update [AspNetUsers] set [AppKey] = @AppKey where [UserName] = @UserName";

                await connection.ExecuteAsync(sql, new { AppKey = appkey ,UserName = userName });
            }
        }

        public async Task ModifyAppSecret(string userName, string appsecret)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = $"update [AspNetUsers] set [AppSecret] = @AppSecret where [UserName] = @UserName";

                await connection.ExecuteAsync(sql, new { AppSecret = appsecret, UserName = userName });
            }
        }
        public async Task<UserAPIKey> GetUserAPIKeys(string userName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = $"select [AppKey], [AppSecret] from [AspNetUsers] where [UserName] = @UserName";

                var result = await connection.QueryFirstOrDefaultAsync<UserAPIKey>(sql, new { UserName = userName });
                
                return result;
            }
        }
    }
}
