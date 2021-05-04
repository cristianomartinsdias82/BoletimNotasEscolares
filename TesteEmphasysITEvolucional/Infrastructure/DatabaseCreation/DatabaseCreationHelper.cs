using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TesteEmphasysITEvolucional.Infrastructure.DatabaseCreation
{
    public static class DatabaseCreationHelper
    {
        public static void Run(
            IConfiguration configuration,
            IHostApplicationLifetime appLifetime,
            IDataProtector dataProtector,
            ILogger<Startup> logger)
        {
            if ((configuration["CREATEDB"] ?? "false") == "true" && !string.IsNullOrWhiteSpace(configuration["MASTERDBCONNSTR"]))
            {
                logger?.LogInformation("Creating database. Please wait...");

                //Gets sql instructions from embedded resource
                const string databaseName = "projeto-evolucional";
                var assembly = Assembly.GetEntryAssembly();
                var assemblyName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
                var resource = assembly.GetManifestResourceStream($"{assemblyName}.Resources.SqlScripts.{databaseName}.sql");
                var resourceStream = new StreamReader(resource);

                var sql = resourceStream.ReadToEnd();
                resourceStream.Close();

                //Creates the database
                Func<string, string> removeSpecialChars = (input) => Regex.Replace(input, @"\n|\r", " ");
                var sqlCommandRows = sql.Split("GO", StringSplitOptions.RemoveEmptyEntries);
                var createDatabaseCommand = sqlCommandRows[0];
                var masterDbConnection = configuration["MASTERDBCONNSTR"].Trim();
                if (!RunSqlScripts(true, masterDbConnection, new List<string> { createDatabaseCommand }, removeSpecialChars, logger))
                {
                    logger?.LogWarning("There were errors when trying to create the database!");
                    appLifetime.StopApplication();
                    return;
                }
                if (!RunSqlScripts(false,masterDbConnection.Replace("master", databaseName), sqlCommandRows.Skip(1), removeSpecialChars, logger))
                {
                    logger?.LogWarning("There were errors when trying to create the database objects!");

                    RunSqlScripts(true, masterDbConnection, new List<string> { "ALTER DATABASE [" + databaseName + "TEMP]" + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE", "DROP DATABASE [" + databaseName + "TEMP]" }, removeSpecialChars, logger);
                    appLifetime.StopApplication();
                    return;
                }

                //Inserts the administrator user
                if (!string.IsNullOrWhiteSpace(configuration["ADMINUSERNAME"]) && !string.IsNullOrWhiteSpace(configuration["ADMINPASSWORD"]))
                {
                    var username = configuration["ADMINUSERNAME"].Trim();
                    var encryptedPassword = dataProtector.Protect(configuration["ADMINPASSWORD"].Trim());

                    logger?.LogWarning("Creating default user. Please wait...");
                    RunSqlScripts(false, masterDbConnection.Replace("master", databaseName), new List<string> { $"INSERT dbo.Users (Username,Password) VALUES ('{username}','{encryptedPassword}')" }, removeSpecialChars, logger);
                    logger?.LogWarning("Default user successfully created!");
                }

                logger?.LogInformation("Database creation successful!");
                appLifetime.StopApplication();
                return;
            }
        }

        private static bool RunSqlScripts(bool isDatabaseCreationOrRemovalCommand, string connectionString, IEnumerable<string> sqlCommandScripts, Func<string, string> stripOutSpecialChars, ILogger<Startup> logger)
        {
            var conn = new SqlConnection(connectionString);
            var tran = default(SqlTransaction);
            var comm = conn.CreateCommand();
            comm.Connection = conn;
            string command = string.Empty;

            try
            {
                conn.Open();

                if (!isDatabaseCreationOrRemovalCommand)
                {
                    tran = conn.BeginTransaction();
                    comm.Transaction = tran;

                    foreach (var sqlScript in sqlCommandScripts)
                    {
                        command = stripOutSpecialChars(sqlScript);
                        comm.CommandText = command;

                        comm.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                else
                {
                    foreach (var sqlScript in sqlCommandScripts)
                    {
                        command = stripOutSpecialChars(sqlScript);
                        comm.CommandText = command;

                        comm.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception exc)
            {
                tran?.Rollback();

                logger?.LogError($"Unable to run the sql command \"{command}\"! {exc.Message}");
                logger?.LogInformation(exc.StackTrace);

                return false;
            }
            finally
            {
                tran?.Dispose();
                comm.Dispose();
                conn.Dispose();
            }
        }
    }
}
