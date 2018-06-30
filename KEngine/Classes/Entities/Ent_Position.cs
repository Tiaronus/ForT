using KEngine.Attributes.UIA;
using KEngine.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;

namespace KEngine.Classes.Entities
{
    [UIAEntity(bAddMenuRecord = true, MenuRecordName = "Positions", EntityType = typeof(Ent_Position))]
    public class Ent_Position : KBaseEntityGen<Ent_Position>
    {
        [StringLength(128)]
        [UIAProperty(Label = "Position", EditorType = EUIAEditorType.TextEdit)]
        public string Nam { get; set; }

        public override string ListBoxDisplay { get { return Nam; } }

        public Ent_Position()
        {
            
        }

        public override ObservableCollection<Ent_Position> LoadAll()
        {
            ObservableCollection<Ent_Position> answer = new ObservableCollection<Ent_Position>();
            SQLiteDataReader rdr = DB_Bridge.Instance.ExecuteReader("select * from tbl_positions order by 1");
            while (rdr.Read())
            {
                answer.Add(new Ent_Position()
                {
                    ID = rdr.GetInt32(0),
                    Nam = rdr.GetString(1)
                });
            }
            return answer;
        }

        public override void Save()
        {
            SQLiteCommand cmd = DB_Bridge.Instance.CreateCommand();
            if (ID != null)
            {
                cmd.CommandText = "update tbl_positions set nam = @N where id = @ID";
                cmd.Parameters.Add(new SQLiteParameter("N", Nam));
                cmd.Parameters.Add(new SQLiteParameter("ID", ID));                
            }
            else
            {
                cmd.CommandText = "insert into tbl_positions (nam) values (@N)";
                cmd.Parameters.Add(new SQLiteParameter("N", Nam));
            }
            DB_Bridge.Instance.ExecuteNonQuery(cmd);
            if (ID == null) ID = DB_Bridge.Instance.GetLastInsertId();
        }

        public override void Delete()
        {
            if(ID != null)
            {
                SQLiteCommand cmd = DB_Bridge.Instance.CreateCommand();
                cmd.CommandText = "delete from tbl_positions where id = @ID";
                cmd.Parameters.Add(new SQLiteParameter("ID", ID));
                DB_Bridge.Instance.ExecuteNonQuery(cmd);
            }
        }        
    }
}
