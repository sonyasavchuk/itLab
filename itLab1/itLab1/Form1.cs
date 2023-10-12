using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace itLab1
{
    public partial class Form1 : Form
    {
        dbManager dbm = new dbManager();
        string cellOldValue = "";
        string cellNewValue = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string userInput = ShowInputDialog("Enter Text", "Please enter name:");
            dbm.CreateDB(userInput);
            tabControl.TabPages.Clear();
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            lblCurrentDB.Text = dbm.GetCurrentDBName();

        }
        private string ShowInputDialog(string caption, string text)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmation = new Button() { Text = "OK", Left = 250, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
        private (string comboBoxValue, string textBoxValue) ShowColumnDialog(string caption, string text)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200, // Increased height to accommodate the ComboBox
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };

            // Create the ComboBox and add items
            ComboBox comboBox = new ComboBox() { Left = 50, Top = 80, Width = 300 };
            comboBox.Items.AddRange(new string[] { "Integer", "Real", "Char", "String", "Date", "DateInvl" });
            comboBox.SelectedIndex = 0; // Set the default selected item

            Button confirmation = new Button() { Text = "OK", Left = 250, Width = 100, Top = 120, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(comboBox);
            prompt.Controls.Add(confirmation);

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                string comboBoxValue = comboBox.SelectedItem.ToString();
                string textBoxValue = textBox.Text;
                return (comboBoxValue, textBoxValue);
            }
            else
            {
                return (null, null); // Return null values if the user cancels the dialog
            }
        }



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;

            sfdSaveDB.Filter = "tdb files (*.tdb)|*.tdb";
            sfdSaveDB.FilterIndex = 1;
            sfdSaveDB.RestoreDirectory = true;

            if (sfdSaveDB.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = sfdSaveDB.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    myStream.Close();

                    dbm.SaveDB(sfdSaveDB.FileName);
                }
            }
        }
        void VisualTable(Table t)
        {
            try
            {
                // Clear existing rows and columns
                dataGridView.Rows.Clear();
                dataGridView.Columns.Clear();

                // Check if the table is null
                if (t == null)
                {
                    // Handle the case where the provided table is null
                    // You can display a message or take appropriate action
                    return;
                }

                // Populate columns
                foreach (Column c in t.tColumnsList)
                {

                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Name = c.cName;
                    column.HeaderText = c.cName;
                    dataGridView.Columns.Add(column);
                }

                // Populate rows
                foreach (Row r in t.tRowsList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    foreach (string s in r.rValuesList)
                    {
                        DataGridViewCell cell = new DataGridViewTextBoxCell();
                        cell.Value = s;
                        row.Cells.Add(cell);
                    }

                    dataGridView.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here or log them for debugging
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdOpenDB.Filter = "tdb files (*.tdb)|*.tdb";
            ofdOpenDB.FilterIndex = 1;
            ofdOpenDB.RestoreDirectory = true;

            if (ofdChooseFilePath.ShowDialog() == DialogResult.OK)
            {
                dbm.OpenDB(ofdChooseFilePath.FileName);
                lblCurrentDB.Text = dbm.GetCurrentDBName();

            }

            tabControl.TabPages.Clear();
            List<string> buf = dbm.GetTableNameList();
            foreach (string s in buf)
                tabControl.TabPages.Add(s);

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(dbm.GetTable(ind));
        }


        private void addTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string userInput = ShowInputDialog("Enter Text", "Please enter name:");

            if (dbm.AddTable(userInput))
            {
                tabControl.TabPages.Add(userInput);
            }
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount == 0) return;
            try
            {
                dbm.DeleteTable(tabControl.SelectedIndex);
                tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
            }
            catch { }
            if (tabControl.TabCount == 0) return;

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(dbm.GetTable(ind));

        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (string comboBoxValue, string textBoxValue) result = ShowColumnDialog("Enter Text", "Please enter name:");
            string selectedValue = result.comboBoxValue;
            string enteredText = result.textBoxValue;
            if (dbm.AddColumn(tabControl.SelectedIndex, enteredText, selectedValue))
            {

                int ind = tabControl.SelectedIndex;
                if (ind != -1) VisualTable(dbm.GetTable(ind));
            }
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int currentTabIndex = tabControl.SelectedIndex;

            if (currentTabIndex >= 0 && dataGridView.Columns.Count > 0)
            {
                int currentColumnIndex = dataGridView.CurrentCell != null ? dataGridView.CurrentCell.ColumnIndex : -1;

                if (currentColumnIndex >= 0 && currentColumnIndex < dataGridView.Columns.Count)
                {
                    try
                    {
                        dbm.DeleteColumn(currentTabIndex, currentColumnIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting column: " + ex.Message);
                    }

                    VisualTable(dbm.GetTable(currentTabIndex));
                }
                else
                {
                    MessageBox.Show("Please select a valid column to delete.");
                }
            }
            else
            {
                MessageBox.Show("There are no columns to delete.");
            }
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dbm.AddRow(tabControl.SelectedIndex))
            {

                int ind = tabControl.SelectedIndex;
                if (ind != -1) VisualTable(dbm.GetTable(ind));
            }
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 0) return;
            try
            {
                dbm.DeleteRow(tabControl.SelectedIndex, dataGridView.CurrentCell.RowIndex);
            }
            catch { }

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(dbm.GetTable(ind));
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string search = tbSearch.Text;
            if (string.IsNullOrEmpty(search))
            {
                return;
            }
            int i = 0;
            while (i < dataGridView.Rows.Count)
            {
                bool find = false;
                for (int j = 0; j < dataGridView.Rows[i].Cells.Count; ++j)
                {
                    DataGridViewCell cell = dataGridView.Rows[i].Cells[j];
                    if (cell.Value != null && cell.Value.ToString() == search)
                    {
                        find = true;
                        break;
                    }
                }
                if (find)
                {
                    ++i;
                }
                else
                {
                    dataGridView.Rows.RemoveAt(i);
                }
            }
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(dbm.GetTable(ind));
            tbSearch.Text = "";
        }

        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.RowIndex < dataGridView.Rows.Count && e.ColumnIndex < dataGridView.Columns.Count)
            {
                object cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null)
                {
                    cellOldValue = cellValue.ToString();
                }
                else
                {
                    // Handle the case where the cell value is null
                    cellOldValue = string.Empty;
                }
            }
            else
            {
                // Handle the case where the row or column index is out of range
                cellOldValue = string.Empty;
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            cellNewValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            if (!dbm.ChangeValue(cellNewValue, tabControl.SelectedIndex, e.ColumnIndex, e.RowIndex))
            {
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellOldValue;
            }

            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(dbm.GetTable(ind));
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ind = tabControl.SelectedIndex;
            if (ind != -1) VisualTable(dbm.GetTable(ind));
        }
    }
}
