using System.Data;
using Microsoft.Data.SqlClient;

namespace Trainning.Helpers
{
    public class SQLHelperNoContext
    {
        private readonly string _connectionString;

        private SqlConnection _connection;
        private SqlTransaction? _transaction;

        public SQLHelperNoContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        // Mở kết nối
        private async Task OpenConnectionAsync()
        {
            _connection ??= new SqlConnection(_connectionString);

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        // Đóng kết nối
        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }
        // Bắt đầu transaction
        public void BeginTransaction()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must be open to begin a transaction.");
            }

            _transaction = _connection.BeginTransaction();
        }

        // Commit transaction
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null; // Reset transaction sau khi commit
        }

        // Rollback transaction
        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null; // Reset transaction sau khi rollback
        }
        // Phương thức thực hiện truy vấn SELECT và trả về DataTable
        public async Task<DataTable> ExecuteQueryAsync(string query, SqlParameter[]? parameters = null)
        {
            await OpenConnectionAsync();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        // Phương thức thực hiện các truy vấn không trả về dữ liệu (INSERT, UPDATE, DELETE)
        public async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[] parameters = null)
        {
            await OpenConnectionAsync();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await command.ExecuteNonQueryAsync();
            }
        }

        // Phương thức thực hiện truy vấn scalar (trả về giá trị đơn lẻ)
        public async Task<object> ExecuteScalarAsync(string query, SqlParameter[] parameters = null)
        {
            await OpenConnectionAsync();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await command.ExecuteScalarAsync();
            }
        }

        // Phương thức lấy DataReader (để xử lý dữ liệu từng hàng, nếu cần)
        public async Task<SqlDataReader> ExecuteReaderAsync(string query, SqlParameter[] parameters = null)
        {
            await OpenConnectionAsync();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
        }

        // Dispose để dọn dẹp tài nguyên
        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }

            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}