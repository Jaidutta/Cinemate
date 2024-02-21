using Npgsql;

namespace Cinemate.Services
{  
    // Be careful of using too many Static classes/methods as they use long term heap memory
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            /* The IConfiguration instance will reach out to the appsettings and grab the 
             * connection string
             */
            var connectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];

            // this variable will be only available if the app is running on host like Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            /* if databaseUrl is Empty, then it is running locally and return the connectionString
             * if not Empty, it will call the function BuildConnectionString and return the string
             */
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);

            /* UserInfo of the databaseUri class has a string containing ":" When it is split on ":"
             * it will turn it into an Array of elements before the : and one element after the :
             */

            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }
    }
}
