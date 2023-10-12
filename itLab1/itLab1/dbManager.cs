using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itLab1
{
    public class dbManager
    {
        DataBase db;

        public bool CreateDB(string dbName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dbName))
                    throw new ArgumentException("Database name cannot be empty or null.");

                db = new DataBase(dbName);
                return true;
            }
            catch (ArgumentException ex)
            {
                
                return false; // Return false to indicate failure
            }
            catch (Exception ex)
            {
                
                return false; // Return false to indicate failure
            }
        }



        public bool AddTable(string tableName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentException("Table name cannot be empty or null.");

                if (db == null)
                    throw new InvalidOperationException("Database not created.");

                db.dbTablesList.Add(new Table(tableName));
                return true;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }


        public Table GetTable(int index)
        {
            if (index == -1) index = 0;
            return db.dbTablesList[index];
        }

        public bool AddColumn(int tIndex, string cname, string ctype)
        {

            try
            {
                if (db == null)
                    throw new InvalidOperationException("Database not created.");
                
                
                if (db.dbTablesList.Count <= 0)
                    throw new InvalidOperationException("No tables exist.");
                if (string.IsNullOrWhiteSpace(cname))
                    throw new ArgumentException("Column name cannot be empty or whitespace.");

                Table table = db.dbTablesList[tIndex];
                table.tColumnsList.Add(new Column(cname, ctype));

                // Check if there are no rows, then add a new row
                if (table.tRowsList.Count == 0)
                {
                    table.tRowsList.Add(new Row());

                    // Initialize the row with empty values for all columns
                    for (int i = 0; i < table.tColumnsList.Count; ++i)
                    {
                        table.tRowsList[0].rValuesList.Add("");
                    }
                }
                else
                {
                    // Update existing rows to include values for the new column
                    for (int i = 0; i < table.tRowsList.Count; ++i)
                    {
                        // Add an empty value for the new column
                        table.tRowsList[i].rValuesList.Add("");
                    }
                }

                return true;
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }




        public bool AddRow(int tIndex)
        {
            try
            {
                if (db == null) return false;
                if (db.dbTablesList.Count <= 0) return false;
                if (db.dbTablesList[tIndex].tColumnsList.Count <= 0) return false;

                db.dbTablesList[tIndex].tRowsList.Add(new Row());
                for (int i = 0; i < db.dbTablesList[tIndex].tColumnsList.Count; ++i)
                {
                    db.dbTablesList[tIndex].tRowsList.Last().rValuesList.Add("");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ChangeValue(string newValue, int tind, int cind, int rind)
        {
            if (tind >= 0 && tind < db.dbTablesList.Count &&
                cind >= 0 && cind < db.dbTablesList[tind].tColumnsList.Count &&
                rind >= 0 && rind < db.dbTablesList[tind].tRowsList.Count)
            {
                if (db.dbTablesList[tind].tColumnsList[cind].cType.Validation(newValue))
                {
                    db.dbTablesList[tind].tRowsList[rind].rValuesList[cind] = newValue;
                    return true;
                }
            }

            return false;
        }


        public void DeleteRow(int tind, int rind)
        {
            if (db == null || tind < 0 || tind >= db.dbTablesList.Count)
                return; // Handle invalid table index gracefully

            Table table = db.dbTablesList[tind];
            if (rind < 0 || rind >= table.tRowsList.Count)
                return; // Handle invalid row index gracefully

            if (table.tRowsList.Count > 1) // Check if there is more than one row
            {
                table.tRowsList.RemoveAt(rind);
            }
        }

        public void DeleteColumn(int tind, int cind)
        {
            if (db == null || tind < 0 || tind >= db.dbTablesList.Count)
                return;

            Table table = db.dbTablesList[tind];
            if (cind < 0 || cind >= table.tColumnsList.Count)
                return;

            string columnNameToDelete = table.tColumnsList[cind].cName;

            // Remove the column from the table's column list
            table.tColumnsList.RemoveAt(cind);

            // Remove the column from each row's values list
            foreach (Row r in table.tRowsList)
            {
                if (cind < r.rValuesList.Count)
                {
                    r.rValuesList.RemoveAt(cind);
                }
            }
        }

        public void DeleteTable(int tind)
        {
            db.dbTablesList.RemoveAt(tind);
        }

        private const char sep = '$';
        private const char space = '#';
        public void SaveDB(string path)
        {
            try
            {
                if (db == null || db.dbTablesList.Count == 0)
                {
                    MessageBox.Show("The database is empty. Nothing to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                StreamWriter sw = new StreamWriter(path);

                sw.WriteLine(db.dbName);
                foreach (Table t in db.dbTablesList)
                {
                    sw.WriteLine(sep);
                    sw.WriteLine(t.tName);
                    foreach (Column c in t.tColumnsList)
                    {
                        sw.Write(c.cName + space);
                    }
                    sw.WriteLine();
                    foreach (Column c in t.tColumnsList)
                    {
                        sw.Write(c.typeName + space);
                    }
                    sw.WriteLine();
                    foreach (Row r in t.tRowsList)
                    {
                        foreach (string s in r.rValuesList)
                        {
                            sw.Write(s + space);
                        }
                        sw.WriteLine();
                    }
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OpenDB(string path)
        {
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            string[] parts = file.Split(sep);

            db = new DataBase(parts[0]);

            for (int i = 1; i < parts.Length; ++i)
            {
                parts[i] = parts[i].Replace("\r\n", "\r");
                List<string> buf = parts[i].Split('\r').ToList();
                buf.RemoveAt(0);
                buf.RemoveAt(buf.Count - 1);

                if (buf.Count > 0)
                {
                    db.dbTablesList.Add(new Table(buf[0]));
                }
                if (buf.Count > 2)
                {
                    string[] cname = buf[1].Split(space);
                    string[] ctype = buf[2].Split(space);
                    int length = cname.Length - 1;
                    for (int j = 0; j < length; ++j)
                    {
                        db.dbTablesList[i - 1].tColumnsList.Add(new Column(cname[j], ctype[j]));
                    }

                    for (int j = 3; j < buf.Count; ++j)
                    {
                        db.dbTablesList[i - 1].tRowsList.Add(new Row());
                        List<string> values = buf[j].Split(space).ToList();
                        values.RemoveAt(values.Count - 1);
                        db.dbTablesList[i - 1].tRowsList.Last().rValuesList.AddRange(values);
                    }
                }
            }

            sr.Close();
        }

        public List<string> GetTableNameList()
        {
            List<string> res = new List<string>();
            foreach (Table t in db.dbTablesList)
                res.Add(t.tName);
            return res;
        }

        public string GetCurrentDBName()
        {
            if (db != null)
            {
                return db.dbName;
            }
            else
            {
                return "No database open";
            }
        }

    }
}
