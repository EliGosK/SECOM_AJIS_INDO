using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.BatchService
{
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    public class ApplicationDataservice
    {

        private const string PROCEDURE = "sp_CM_GenerateBatchQueue";

        public void GenerateBatchQueue()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SECOM_AJIS.BatchService.Properties.Settings.SECOM_AJIS_TESTConnectionString"].ConnectionString;

                using (SqlCommand command = new SqlCommand(PROCEDURE, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ReferenceDate", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
