using KEngine.Attributes.UIA;
using KEngine.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;

namespace KEngine.Classes.Entities
{
    [UIAEntity(bAddMenuRecord = true, MenuRecordName = "Employees", EntityType = typeof(Ent_Employee))]
    public class Ent_Employee : KBaseEntityGen<Ent_Employee>
    {
        [StringLength(128)]
        [UIAProperty(Label = "Full Name", EditorType = EUIAEditorType.TextEdit)]
        public string FIO { get; set; }

        [UIAProperty(Label = "Position", EditorType = EUIAEditorType.ComboBox, TypeOfValues = typeof(Ent_Position))]
        public long PositionID { get; set; }

        [UIAProperty(Label = "Start Date", EditorType = EUIAEditorType.DateEdit)]
        public DateTime DB { get; set; } = new DateTime(1900, 1, 1);

        [UIAProperty(Label = "End Date", EditorType = EUIAEditorType.DateEdit)]
        public DateTime DE { get; set; } = new DateTime(2100, 1, 1);

        [StringLength(512)]
        [UIAProperty(Label = "Reason for Dismissal", EditorType = EUIAEditorType.TextEdit)]
        public string DismissalRem { get; set; }

        public override string ListBoxDisplay { get { return FIO; } }

        public Ent_Employee()
        {

        }


        public override void Delete()
        {
            if (ID != null)
            {
                SQLiteCommand cmd = DB_Bridge.Instance.CreateCommand();
                cmd.CommandText = "delete from tbl_employees where id = @ID";
                cmd.Parameters.Add(new SQLiteParameter("ID", ID));
                DB_Bridge.Instance.ExecuteNonQuery(cmd);
            }
        }

        public override ObservableCollection<Ent_Employee> LoadAll()
        {
            ObservableCollection<Ent_Employee> answer = new ObservableCollection<Ent_Employee>();
            SQLiteDataReader rdr = DB_Bridge.Instance.ExecuteReader("select * from tbl_employees order by 1");
            while (rdr.Read())
            {
                var e = new Ent_Employee()
                {
                    ID = rdr.GetInt32(0),
                    FIO = rdr.GetString(1),
                    PositionID = rdr.GetInt64(2),
                    //DB = (rdr.GetValue(3) == DBNull.Value) ? new DateTime(1900, 1, 1) : rdr.GetDateTime(3),
                    //DE = (rdr.GetValue(4) == DBNull.Value) ? new DateTime(2100, 1, 1) : rdr.GetDateTime(4),
                    //DB = rdr.GetDateTime(3),
                    //DE = rdr.GetDateTime(4),
                    DismissalRem = (rdr.GetValue(5) == DBNull.Value) ? string.Empty : rdr.GetString(5)
                };
                try
                {
                    e.DB = rdr.GetDateTime(3);
                }
                catch (Exception)
                {
                    e.DB = new DateTime(1900, 1, 1);
                }
                try
                {
                    e.DE = rdr.GetDateTime(4);
                }
                catch (Exception)
                {
                    e.DE = new DateTime(2100, 1, 1);
                }

                answer.Add(e);
            }
            return answer;
        }

        public override void Save()
        {
            SQLiteCommand cmd = DB_Bridge.Instance.CreateCommand();
            if (ID != null)
            {
                cmd.CommandText = "update tbl_employees set FIO = @FIO, db = @DB, de = @DE, positionid = @PID, rem = @REM where id = @ID";
                cmd.Parameters.Add(new SQLiteParameter("FIO", FIO));
                cmd.Parameters.Add(new SQLiteParameter("DB", DB));
                cmd.Parameters.Add(new SQLiteParameter("DE", DE));
                cmd.Parameters.Add(new SQLiteParameter("PID", PositionID));
                cmd.Parameters.Add(new SQLiteParameter("REM", DismissalRem));
                cmd.Parameters.Add(new SQLiteParameter("ID", ID));
            }
            else
            {
                cmd.CommandText = "insert into tbl_employees (fio,db,de,positionid,rem) values (@FIO,@DB,@DE,@PID,@REM)";
                cmd.Parameters.Add(new SQLiteParameter("FIO", FIO));
                cmd.Parameters.Add(new SQLiteParameter("DB", DB));
                cmd.Parameters.Add(new SQLiteParameter("DE", DE));
                cmd.Parameters.Add(new SQLiteParameter("PID", PositionID));
                cmd.Parameters.Add(new SQLiteParameter("REM", DismissalRem));
            }
            DB_Bridge.Instance.ExecuteNonQuery(cmd);
            if (ID == null) ID = DB_Bridge.Instance.GetLastInsertId();
        }
    }
}
