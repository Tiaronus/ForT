using ExtensionMethods;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace KEngine.Classes
{
    public class DB_Bridge
    {
        private readonly static DB_Bridge _instance = new DB_Bridge();
        public static DB_Bridge Instance { get { return _instance; } }

        public SQLiteConnection Conn;

        private DB_Bridge()
        {
            Conn = new SQLiteConnection(GenerateConnectionString());
            Conn.Open();
        }

        ~DB_Bridge()
        {
            if (Conn != null)
                try
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                catch (Exception) { }
        }

        private string GenerateConnectionString()
        {
            string DBName = "KDBase.db";
            string DBFullPath = Path.Combine(KCore.FS.PathDB, DBName);
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
            csb.DataSource = DBFullPath;

            if (!File.Exists(DBFullPath))
            {
                SQLiteConnection.CreateFile(DBFullPath);
                using (SQLiteConnection conn = new SQLiteConnection(csb.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"BEGIN TRANSACTION;
                                        CREATE TABLE IF NOT EXISTS `tbl_positions` (
	                                        `id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                                        `nam`	TEXT NOT NULL UNIQUE
                                        );
                                        CREATE TABLE IF NOT EXISTS `tbl_employees` (
	                                        `ID`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                                        `FIO`	TEXT NOT NULL,
	                                        `PositionID`	INTEGER NOT NULL,
	                                        `DB`	TEXT,
	                                        `DE`	TEXT,
	                                        `Rem`	TEXT,
	                                        FOREIGN KEY(`PositionID`) REFERENCES `tbl_positions`(`id`)
                                        );
                                        CREATE TABLE IF NOT EXISTS `tbl_admins` (
	                                        `id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                                        `login`	TEXT NOT NULL UNIQUE,
	                                        `pass`	TEXT NOT NULL
                                        );
                                        COMMIT;";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return csb.ToString();

        }

        public bool CheckIfAdminExist(string login, string password = null)
        {
            bool answer = false;
            string q = string.Format("select count(*) from tbl_admins a where a.login = @L {0}", (string.IsNullOrWhiteSpace(password)) ? string.Empty : " and a.pass = @P");
            SQLiteCommand cmd = Conn.CreateCommand();
            cmd.CommandText = q;
            cmd.Parameters.Add(new SQLiteParameter("L", login));
            if (!string.IsNullOrWhiteSpace(password))
                cmd.Parameters.Add(new SQLiteParameter("P", password.MD5Hash()));
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int res = rdr.GetInt32(0);
                answer = res.Equals(1);
            }
            return answer;
        }

        public bool CreateAdmin(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login)) throw new Exception("Empty Login");
            if (string.IsNullOrWhiteSpace(password)) throw new Exception("Empty Password");
            string q = "insert into tbl_admins (login,pass) values (@L,@P)";
            SQLiteCommand cmd = Conn.CreateCommand();
            cmd.CommandText = q;
            cmd.Parameters.Add(new SQLiteParameter("L", login));
            cmd.Parameters.Add(new SQLiteParameter("P", password.MD5Hash()));
            try
            {
                SQLiteTransaction trans = Conn.BeginTransaction();
                cmd.ExecuteNonQuery();
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                KCore.ShowKError(ex.Message);
                return false;
            }

        }

        public SQLiteDataReader ExecuteReader(string q)
        {
            SQLiteCommand cmd = Conn.CreateCommand();
            cmd.CommandText = q;
            return cmd.ExecuteReader();
        }

        public void ExecuteNonQuery(string q)
        {
            SQLiteCommand cmd = Conn.CreateCommand();
            cmd.CommandText = q;
            SQLiteTransaction t = Conn.BeginTransaction();
            cmd.ExecuteNonQuery();
            t.Commit();
        }

        public bool ExecuteNonQuery(SQLiteCommand cmd)
        {
            bool answer = false;
            SQLiteTransaction t = Conn.BeginTransaction();
            try
            {
                cmd.ExecuteNonQuery();
                t.Commit();
                answer = true;
            }
            catch (SQLiteException sex)
            {
                KCore.ShowKError(sex.Message);
                t.Rollback();
            }
            catch (Exception ex)
            {
                KCore.ShowKError(ex.Message);
                t.Rollback();
            }
            return answer;
        }

        public SQLiteCommand CreateCommand()
        {
            return Conn.CreateCommand();
        }

        public long GetLastInsertId()
        {
            return Conn.LastInsertRowId;
        }
    }
}
