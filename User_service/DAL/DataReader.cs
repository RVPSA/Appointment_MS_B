using System.Data.Common;

namespace User_service.DAL;

public class DataReader(DbDataReader dbDataReader)
{
    public int GetInt32(string column)
    {
        int data = 0;
        if (CheckColumnExists(dbDataReader, column))
        {
            data = dbDataReader.IsDBNull(dbDataReader.GetOrdinal(column))? 0 : (int)dbDataReader[column];
        }

        return data;
    }

    public string GetString(string column)
    {
        string data = string.Empty;
        if (CheckColumnExists(dbDataReader, column))
        {
            data = dbDataReader[column].ToString() ?? String.Empty;
        }

        return data;
    }

    private bool CheckColumnExists(DbDataReader dataReader,string columnName)
    {
        int columnCount = dataReader.GetColumnSchema().Count(t => t.ColumnName == columnName);
        return columnCount > 0;
    }
}