using System;
using System.Data;
using System.Data.SqlClient;

namespace SigortaVip.Models
{
    internal class dal
    {
        private string lastQuerySQL;
        public SqlConnection myConnection = new SqlConnection("server =93.89.230.234 ; database = sigortavipserver ; uid = sigortaVipDbUser; pwd =Asdfzxcv.12321!!; integrated security=false;MultipleActiveResultSets=true");


        public DataSet CommandExecuteReader(String sql, SqlConnection conn)
        {

            lastQuerySQL = sql;

            DataSet ds = new DataSet();
            privateOpen(conn);
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn);
                myCommand.CommandTimeout = 600;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(myCommand);

                dataAdapter.Fill(ds);

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);

            }
            finally
            {
                privateClose(conn);
            }


            return ds;

        }
        public void CommandExecuteNonQuery(String sql, SqlConnection conn)
        {
            lastQuerySQL = sql;
            privateOpen(conn);
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn);
                myCommand.CommandTimeout = 600;
                myCommand.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);


                try
                {
                    string sqllog = $"insert into ms_sql_log(log_text) values('{exp.Message}') ";
                    SqlCommand myCommand = new SqlCommand(sqllog, conn);
                    myCommand.CommandTimeout = 600;
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    privateClose(conn);
                }
            }
        }

        private void privateClose(SqlConnection connection)
        {
            try
            {
                connection.Close();
            }
            catch (Exception)
            {
            }
        }

        private void privateOpen(SqlConnection connection)
        {
            try
            {
                ConControl(connection);
            }
            catch (Exception ex)
            {
            }
        }

        private void ConControl(SqlConnection c)
        {
            if (c.State != ConnectionState.Open)
            {
                c.Open();
            }

        }
        public void OpenSQLConnection(String ConnectionString, SqlConnection conn)
        {
            myConnection = new SqlConnection(ConnectionString);

            ConControl(myConnection);

        }
        ~dal()
        {
            privateClose(myConnection);
            try
            {
                myConnection.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
