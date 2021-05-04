using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;

namespace TesteEmphasysITEvolucional.Services.Data
{
    public class UserDataService : IUserDataService
    {
        private readonly string _connectionString;
        private readonly IDataProtector _dataProtector;
        private readonly string _invalidUsernameOrPassword = "Invalid user and/or password";

        public UserDataService(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:projeto-evolucional"];
            dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException("Data protection provider argument cannot be null.");
            _dataProtector = dataProtectionProvider.CreateProtector(Startup.ApplicationName);
        }

        public async Task<OperationResult<bool>> CheckEnteredCredentialsAsync(string username, string password, CancellationToken cancellationToken)
        {
            bool userAndPasswordCheckOut = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.Connection = conn;

                    comm.Parameters.AddWithValue("@username", username);

                    comm.CommandText = "SELECT password FROM dbo.Users WHERE username = @username";

                    try
                    {
                        conn.Open();

                        var queryResult = await comm.ExecuteScalarAsync(cancellationToken);
                        if (queryResult == null)
                            return OperationResult<bool>.Success(false, _invalidUsernameOrPassword);

                        var decryptedPassword = _dataProtector.Unprotect($"{queryResult}");
                        userAndPasswordCheckOut = StringComparer.Ordinal.Compare(decryptedPassword, password).Equals(0);
                    }
                    catch (Exception exc)
                    {
                        return OperationResult<bool>.Failure($"Unable to check user credentials. {exc.Message}");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

            return OperationResult<bool>.Success(userAndPasswordCheckOut, !userAndPasswordCheckOut ? _invalidUsernameOrPassword : default);
        }
    }
}
