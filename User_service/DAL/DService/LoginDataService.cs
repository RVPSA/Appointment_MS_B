using System.Data.Common;
using User_service.BusinessObjects.Login;
using User_service.DAL.IDService;

namespace User_service.DAL.DService;

public class LoginDataService(IDataService dataService) : ILoginDataService
{
    public LoggedUser? Login(LoginUserRequest loginUserRequest)
    {
        try
        {
            DbParameter[] dbParameters = new DbParameter[1];
            
            dbParameters[0] = DataServiceCreator.CreateDbParameter("@userName",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,loginUserRequest.UserName);
            
            DbDataReader dbDataReader = dataService.ExecuteReader("login.GetUserCredentials",dbParameters,null);
            if (!dbDataReader.HasRows) return null;
            while (dbDataReader.Read())
            {
                DataReader dataReader = new DataReader(dbDataReader);
                return new LoggedUser()
                {
                    UserName = dataReader.GetString("username"),
                    Email = dataReader.GetString("email"),
                };
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}