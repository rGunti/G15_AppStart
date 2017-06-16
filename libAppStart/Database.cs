using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.SQLite;

namespace libAppStart
{
    public class Database
    {
        private SQLiteConnection myConnection;
        private string myFileName;

        public Database(string filename = "apps.db")
        {
            myFileName = filename;

            myConnection = new SQLiteConnection(String.Format("Data Source={0}", myFileName));
        }

        public string FileName { get { return myFileName; } }

        public Exception InitializeDatabase(bool overwrite = false)
        {
            try
            {
                if (File.Exists(myFileName))
                {
                    if (!overwrite)
                        throw new FileAlreadyExistsException("Database File Already Exists!");

                    File.Delete(myFileName);
                }

                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand(myConnection);
                cmd.CommandText = "CREATE TABLE apps ( [AppPath] VARCHAR(255) NULL PRIMARY KEY, [AppName] VARCHAR(255) NULL )";
                if (!SQL.DoesTableExist(myConnection, "apps")) cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE settings ( [SettingKey] VARCHAR(30)  NULL PRIMARY KEY, [SettingType] CHAR(10) NOT NULL, [SettingValue] VARCHAR(255)  NULL)";
                cmd.ExecuteNonQuery();
                if (!SQL.DoesTableExist(myConnection, "settings")) cmd.ExecuteNonQuery();

                SetPreference(new Preference("4th_OpenConfigEnabled", true));
            }
            catch (Exception ex)
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();

                return ex;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return new EverythingIsFine("Everything's fine!");
        }

        public enum OrderBy
        {
            AppPath,
            AppName,
            AppPathDESC,
            AppNameDESC,
            None
        };

        string GetOrderByCommand(OrderBy o)
        {
            switch (o)
            {
                case OrderBy.AppName: return " ORDER BY AppName";
                case OrderBy.AppNameDESC: return " ORDER BY AppName DESC";
                case OrderBy.AppPath: return " ORDER BY AppPath";
                case OrderBy.AppPathDESC: return " ORDER BY AppPath DESC";
                default: return "";
            }
        }

        public Exception GetAllApplications(OrderBy order = OrderBy.AppName)
        {
            Exception rex;
            List<App> list = new List<App>();

            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM apps" + GetOrderByCommand(order), myConnection);

                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        App app = new App(reader["AppPath"].ToString(), reader["AppName"].ToString());
                        list.Add(app);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                rex = new AppListRecieved(list);
            }
            catch (Exception ex)
            {
                rex = ex;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return rex;
        }

        public Exception AddNewApplication(App a)
        {
            Exception rex = new EverythingIsFine("");

            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand("INSERT INTO apps VALUES(@path, @name)", myConnection);
                cmd.Parameters.AddWithValue("@path", a.Path);
                cmd.Parameters.AddWithValue("@name", a.Name);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                rex = ex;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return rex;
        }

        public Exception RemoveAppication(App a)
        {
            Exception rex = new EverythingIsFine();

            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand("DELETE FROM apps WHERE AppPath = @path", myConnection);
                cmd.Parameters.AddWithValue("@path", a.Path);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                rex = ex;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return rex;
        }

        public Exception EditApplication(App oldA, App newA)
        {
            Exception rex = new EverythingIsFine();

            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand("UPDATE apps SET AppName=@name, AppPath=@path WHERE AppPath=@oldPath", myConnection);
                cmd.Parameters.AddWithValue("@path", newA.Path);
                cmd.Parameters.AddWithValue("@name", newA.Name);
                cmd.Parameters.AddWithValue("@oldPath", oldA.Path);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                rex = ex;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return rex;
        }

        public App GetAppByPath(string path)
        {
            App returner = null;

            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand(
                    "SELECT * FROM apps WHERE AppPath=@path",
                    myConnection);
                cmd.Parameters.AddWithValue("@path", path);

                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    returner = new App(
                        reader["AppPath"].ToString(),
                        reader["AppName"].ToString());
                }
            }
            catch (Exception ex)
            {
                returner = null;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return returner;
        }

        public List<Preference> GetPrefList()
        {
            List<Preference> prefList = new List<Preference>();
            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand(
                    "SELECT * FROM settings ORDER BY SettingKey",
                    myConnection);

                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string key = reader["SettingKey"].ToString();
                    string value_String = reader["SettingValue"].ToString();
                    string type = reader["SettingType"].ToString();

                    object value = null;

                    switch (type)
                    {
                        case "bool":
                            value = ((value_String == "TRUE"));
                            break;
                        case "int":
                            int value_Int = 0;
                            Int32.TryParse(value_String, out value_Int);

                            value = value_Int;
                            break;
                        default:
                            value = String.Format("{0}", value_String);
                            break;
                    }

                    prefList.Add(new Preference(key, value));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return prefList;
        }

        public Preference GetPreferenceByKey(string key)
        {
            Preference pref = null;
            try
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand(
                    "SELECT * FROM settings WHERE SettingKey=@key",
                    myConnection);
                cmd.Parameters.AddWithValue("@key", key);

                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string value_String = reader["SettingValue"].ToString();
                    string type = reader["SettingType"].ToString();

                    object value = null;

                    switch (type)
                    {
                        case "bool":
                            value = ((value_String == "True"));
                            break;
                        case "int":
                            int value_Int = 0;
                            Int32.TryParse(value_String, out value_Int);

                            value = value_Int;
                            break;
                        default:
                            value = String.Format("{0}", value_String);
                            break;
                    }

                    pref = new Preference(reader["SettingKey"].ToString(), value);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            if (pref == null) return null;
            else return pref;
        }

        public bool SetPreference(Preference pref)
        {
            bool ok = true;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(myConnection);
                if (GetPreferenceByKey(pref.Key) == null)
                {
                    cmd.CommandText = "INSERT INTO settings (SettingKey, SettingType, SettingValue) " +
                        "VALUES(@key, @type, @value)";

                }
                else
                {
                    cmd.CommandText = "UPDATE settings SET SettingValue = @value WHERE SettingKey = @key";
                }

                Type t = pref.Value.GetType();

                if (t == typeof(int))
                    cmd.Parameters.AddWithValue("@type", "int");
                else if (t == typeof(bool))
                    cmd.Parameters.AddWithValue("@type", "bool");
                else
                    cmd.Parameters.AddWithValue("@type", "string");

                cmd.Parameters.AddWithValue("@key", pref.Key);
                cmd.Parameters.AddWithValue("@value", pref.Value.ToString());

                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ok = false;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return ok;
        }

        public bool RemovePreference(Preference pref)
        {
            bool ok = true;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(myConnection);
                cmd.CommandText = "DELETE FROM settings where SettingKey = @key";

                cmd.Parameters.AddWithValue("@key", pref.Key);

                if (myConnection.State != System.Data.ConnectionState.Open)
                    myConnection.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ok = false;
            }
            finally
            {
                if (myConnection.State == System.Data.ConnectionState.Open)
                    myConnection.Close();
            }

            return ok;
        }
    }
}
