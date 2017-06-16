using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace libAppStart
{
    static class SQL
    {
        public static bool DoesTableExist(SQLiteConnection con, string table, bool closeConnection = false)
        {
            string name = "";

            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                SQLiteCommand cmd = new SQLiteCommand(con);
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@table";
                cmd.Parameters.AddWithValue("@table", table);

                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    name = reader["name"].ToString();
                }

                if (closeConnection) con.Close();
            }
            catch (Exception ex)
            {
                if (closeConnection) con.Close();
                return false;
            }

            return (name == table);
        }
    }
}
