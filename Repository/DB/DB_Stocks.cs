using Dapper;
using InvestixDev.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;

namespace InvestixDev.Repository.DB
{
    public class DB_Stocks
    {
        private readonly string _connectionString;
        private readonly UserManager<Users> _userManager;
        private readonly DB_User _dbuser;

        

        public DB_Stocks(IConfiguration configuration, UserManager<Users> userManager, DB_User dbuser)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); 
            _userManager = userManager;
            _dbuser = dbuser;
        }

        public async Task<List<AutocompleteElement>> GetAutocomplete(string parm)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @" SELECT TOP (50) [Symbol],[Currency],[Name],[Exchange],[Type] FROM [Stocks] WHERE [Symbol] LIKE @search OR [Name] LIKE @search";

                var result = await connection.QueryAsync<AutocompleteElement>(sql, new { search = parm + "%" });

                var list = new List<AutocompleteElement>();

                foreach (var item in result)
                    list.Add(item);

                return list;
            }
        }
    }
}
