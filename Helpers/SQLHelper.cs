using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Trainning.Data;

namespace Trainning.Helpers
{
  
    public class SQLHelper
    {
        private readonly ApplicationDbContext _context;

        public SQLHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to execute stored procedure with parameters
        public async Task<int> ExecuteNonQueryAsync(string storedProcedure, params object[] parameters)
        {
            var command = CreateCommand(storedProcedure, parameters);
            return await command.ExecuteNonQueryAsync();
        }

        // Method to execute stored procedure and return results
        public async Task<List<T>> ExecuteQueryAsync<T>(string storedProcedure, params object[] parameters) where T : class
        {
            var command = CreateCommand(storedProcedure, parameters);
            using (var reader = await command.ExecuteReaderAsync())
            {
                var result = new List<T>();
                while (await reader.ReadAsync())
                {
                    var entity = Activator.CreateInstance<T>();
                    // Mapping logic between reader and entity should be implemented here
                    result.Add(entity);
                }
                return result;
            }
        }

        // Helper to create a command
        private DbCommand CreateCommand(string storedProcedure, params object[] parameters)
        {
            var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i += 2)
                {
                    var paramName = parameters[i].ToString();
                    var paramValue = parameters[i + 1];
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = paramName;
                    parameter.Value = paramValue ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }

            _context.Database.OpenConnection();
            return command;
        }
    }
}