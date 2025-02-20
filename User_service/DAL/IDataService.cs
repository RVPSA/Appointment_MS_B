using System.Data.Common;

namespace User_service.DAL;

public interface IDataService
{
    public void CloseConnection(); //Close connection with database
    public DbDataReader ExecuteReader(string spName, DbParameter[]? parameters,int? timeOut); //Execute reader when getting data from database
    public int ExecuteNonQuery(string spName, DbParameter[] parameters,int? timeout); //Execute query when adding data to the database
    public void BeginTransaction(); //SQL transaction begin
    public void RollbackTransaction(); //SQL transaction roll back
    public void CommitTransaction(); //SQL transaction commit
}