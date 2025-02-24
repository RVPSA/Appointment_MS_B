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
                    UserName = dataReader.GetString("email"),
                    Email = dataReader.GetString("email"),
                    Password = dataReader.GetString("password")
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

    public SignedUpUser? SignUp(SignUpRequest signUpRequest)
    {
        try
        {
            DbParameter[] dbParameters = new DbParameter[12];
            
            dbParameters[0] = DataServiceCreator.CreateDbParameter("@firstname",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.FirstName);
            dbParameters[1] = DataServiceCreator.CreateDbParameter("@lastname",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.LastName);
            dbParameters[2] = DataServiceCreator.CreateDbParameter("@surename",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.SureName);
            dbParameters[3] = DataServiceCreator.CreateDbParameter("@address",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.Address);
            dbParameters[4] = DataServiceCreator.CreateDbParameter("@email",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.Email);
            dbParameters[5] = DataServiceCreator.CreateDbParameter("@role",System.Data.DbType.Int32,
                System.Data.ParameterDirection.Input,2); // TODO Need to add enum
            dbParameters[6] = DataServiceCreator.CreateDbParameter("@password",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.Password);
            dbParameters[7] = DataServiceCreator.CreateDbParameter("@contactnumber",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.ContactNumber);
            dbParameters[8] = DataServiceCreator.CreateDbParameter("@dob",System.Data.DbType.Date,
                System.Data.ParameterDirection.Input,signUpRequest.Dob);
            dbParameters[9] = DataServiceCreator.CreateDbParameter("@gender",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.Gender);
            dbParameters[10] = DataServiceCreator.CreateDbParameter("@nicnumber",System.Data.DbType.String,
                System.Data.ParameterDirection.Input,signUpRequest.NicNumber);
            dbParameters[11] = DataServiceCreator.CreateDbParameter("@status",System.Data.DbType.Int32,
                System.Data.ParameterDirection.Input,1); // TODO Need to add enum
            
            DbDataReader dbDataReader = dataService.ExecuteReader("login.AddUserPatient",dbParameters,null);
            
            if (!dbDataReader.HasRows) return null;
            while (dbDataReader.Read())
            {
                DataReader dataReader = new DataReader(dbDataReader);
                return new SignedUpUser()
                {
                    LastName = dataReader.GetString("lastname"),
                    UserName = dataReader.GetString("email"),
                    Email = dataReader.GetString("email"),
                    ContactNumber = dataReader.GetString("contactnumber"),
                    NicNumber = dataReader.GetString("nicnumber"),
                    Role = dataReader.GetString("role"),
                    PatientNumber = dataReader.GetString("number")
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