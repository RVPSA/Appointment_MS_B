using System.Data.Common;
using Microsoft.Data.SqlClient;
using User_service.Common;

namespace User_service.DAL;

public class DataService:IDataService
{
    private readonly string _connectionString = AppSettings.ConnectionString ?? throw new InvalidOperationException("Connection string is not valid");
    private SqlConnection? _sqlConnection;
    private SqlDataReader? _sqlDataReader;
    private SqlTransaction? _sqlTransaction;
    
    public DataService()
    {
        _sqlConnection = new SqlConnection(_connectionString); //Open sql connection
        _sqlConnection.Open();
    }

    public void CloseConnection()
    {
        if (_sqlConnection != null && _sqlConnection.State != System.Data.ConnectionState.Closed)
        {
            _sqlConnection.Close();
            _sqlConnection.Dispose();
        }

        if (_sqlDataReader != null && !_sqlDataReader.IsClosed)
        {
            _sqlDataReader.Dispose();
        }

        _sqlConnection?.Dispose();
    }

    public DbDataReader ExecuteReader(string spName, DbParameter[]? parameters,int? timeOut = null)
    {
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = _sqlConnection;
        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

        if (timeOut != null)
        {
            sqlCommand.CommandTimeout = Convert.ToInt32(timeOut);
        }
        sqlCommand.CommandText = spName;
        sqlCommand.Parameters.Clear();
        AddParameters(sqlCommand,parameters);

        if (_sqlTransaction != null)
        {
            sqlCommand.Transaction = _sqlTransaction;
        }
        _sqlDataReader = sqlCommand.ExecuteReader();
        return _sqlDataReader;
    }

    public int ExecuteNonQuery(string spName, DbParameter[] parameters,int? timeOut = null)
    {
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = _sqlConnection;
        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

        if (timeOut != null)
        {
            sqlCommand.CommandTimeout = Convert.ToInt32(timeOut);
        }
        sqlCommand.CommandText = spName;
        sqlCommand.Parameters.Clear();
        AddParameters(sqlCommand,parameters);

        if (_sqlTransaction != null)
        {
            sqlCommand.Transaction = _sqlTransaction;
        }
        int result = sqlCommand.ExecuteNonQuery();
        return result;
    }

    public void BeginTransaction()
    {
        if (_sqlConnection is not { State: System.Data.ConnectionState.Closed }) return;
        _sqlConnection.Open();
        _sqlTransaction = _sqlConnection.BeginTransaction();
    }

    public void RollbackTransaction()
    {
        if (_sqlConnection == null) return;
        if (_sqlTransaction != null)
        {
            _sqlTransaction.Rollback();
            _sqlTransaction = null;
        }

        if (_sqlConnection is not { State: System.Data.ConnectionState.Open }) return;
        _sqlConnection.Close();
        _sqlConnection.Dispose();

    }

    public void CommitTransaction()
    {
        if (_sqlConnection == null) return;
        if (_sqlTransaction != null)
        {
            _sqlTransaction.Commit();
            _sqlTransaction = null;
        }
        if (_sqlConnection is not { State: System.Data.ConnectionState.Open }) return;
        _sqlConnection.Close();
        _sqlConnection.Dispose();
    }

    private void AddParameters(SqlCommand sqlCommand,DbParameter[]? parameters)
    {
        if (parameters == null) return;
        
        foreach (DbParameter parameter in parameters)
        {
          sqlCommand.Parameters.Add(parameter);
        }
        
    }
}