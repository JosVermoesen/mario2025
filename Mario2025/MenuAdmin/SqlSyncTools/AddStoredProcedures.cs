using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace MarioApp
{
    class AddstoredProcedures
    {
        public void AddSP(string sqlPathSP, string cnn)
        {
            string sql = System.IO.File.ReadAllText(sqlPathSP);

            using (SqlConnection connection = new SqlConnection(cnn))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
