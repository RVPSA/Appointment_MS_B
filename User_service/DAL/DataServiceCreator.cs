using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace User_service.DAL;

public class DataServiceCreator
{
    public static IDataService CreateDataService()
    {
        return new DataService();
    }

    public static DbParameter CreateDbParameter(string parameterName, DbType paramType, ParameterDirection paramDirection,object value)
    {
        SqlParameter parameter = new SqlParameter()
        {
            DbType = paramType,
            ParameterName = parameterName,
            Direction = paramDirection,
            Value = value
        };
        return parameter;
    }

    public static DbParameter CreateDataListParameter(string parameterName, ParameterDirection paramDirection,object value)
    {
        SqlParameter parameter = new SqlParameter()
        {
            SqlDbType = SqlDbType.Structured,
            ParameterName = parameterName,
            Direction = paramDirection,
            Value = value
        };
        return parameter;
    }
}