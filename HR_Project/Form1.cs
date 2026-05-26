using HRBusniss;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HR_Project
{
    public partial class Form1 : Form
    {

        DataView V = new DataView();
        ClsEmployee Emp;
        enum Emodes { eNew = 1, eUpdate = 2 };
        Emodes mode = Emodes.eNew;

        public Form1()
        {
            InitializeComponent();
        }

        private void _LoadTable()
        {
            V = ClsEmployee.GetAllEmployees().DefaultView;
            dataGridView1.DataSource = V;
        }




        private bool _Add()
        {

            if (!_CheckData())
            {
                return false;
            }



            string FirstName = txtFirstName.Text;
            string LastName = txtLastName.Text;
            char gendor = RBf.Checked ? 'F' : 'M';

            decimal.TryParse(txtSalary.Text, out decimal Salary);
            float.TryParse(txtBouns.Text, out float Bouns);

            int DeptID = Convert.ToInt32(CBDept.SelectedValue);
            int countryID = Convert.ToInt32(CBcounty.SelectedIndex);


            DateTime Birth = dateTimeBirth.Value;
            DateTime HireDate = dateTimePHire.Value;
            DateTime? ExitDate;

            if (dateTimeExit.Value == DateTime.Now)
            {
                ExitDate = null;
            }
            else
            {
                ExitDate = dateTimeExit.Value;
            }

            Emp = new ClsEmployee();


            Emp.FirstName = FirstName;
            Emp.LastName = LastName;
            Emp.Salary = Salary;
            Emp.Bouns = Bouns;
            Emp.ExitDate = ExitDate;
            Emp.HireDate = HireDate;
            Emp.CountryID = countryID;
            Emp.DepartmentID = DeptID;
            Emp.DateOfBirth = Birth;
            Emp.Gendor = gendor;

            if (Emp.Save())
            {
                MessageBox.Show($"Employee Added With ID = {Emp.ID}");
                ClearBoxes();
                comboBox1.SelectedItem = "None";
                return true;
            }
            else
            {
                MessageBox.Show($"Failed To Save.");
                return false;
            }


        }


        private void ClearBoxes()
        {
            txtBouns.Clear();
            txtSalary.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            comboBox1.SelectedItem = "None";
            CBcounty.SelectedIndex = -1;
            CBDept.SelectedIndex = -1;

            RBf.Checked = false;
            RBmale.Checked = false;
            dateTimeBirth.Value = DateTime.Now;
            dateTimeExit.Value = DateTime.Now;
            dateTimePHire.Value = DateTime.Now;


        }
        private bool _CheckData()
        {

            if (string.IsNullOrWhiteSpace(txtBouns.Text) ||
                  string.IsNullOrWhiteSpace(txtSalary.Text) ||
            string.IsNullOrWhiteSpace(txtFirstName.Text) ||
              string.IsNullOrWhiteSpace(txtLastName.Text) ||
                (!RBf.Checked && !RBmale.Checked) ||
                 (CBcounty.SelectedIndex == -1) ||
                 (CBDept.SelectedIndex == -1))
            {
                MessageBox.Show("Enter All Field");

                return false;

            }
            else if (!double.TryParse(txtBouns.Text, out _))
            {
                MessageBox.Show("Bouns Must Be A Number.");
                txtBouns.Clear();
                txtBouns.Focus();
                return false;
            }
            else if (!double.TryParse(txtSalary.Text, out _))
            {
                MessageBox.Show("Salary Must Be A Number.");
                txtSalary.Clear();
                txtSalary.Focus();
                return false;
            }
            else if (txtFirstName.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Enter Name with Characters Only.");
                txtFirstName.Clear();
                txtFirstName.Focus();
                return false;
            }
            else if (txtLastName.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Enter Name with Characters Only.");
                txtLastName.Clear();
                txtLastName.Focus();
                return false;
            }
            return true;

        }
        private void _LoadIDComboBox()
        {
            DataTable IDs = ClsEmployee.GetAllEmployees();

            // أضف None أول شي
            comboBox1.Items.Add("None");

            // بعدين دور على الجدول وأضف كل ID لحاله
            foreach (DataRow row in IDs.Rows)
            {
                comboBox1.Items.Add(row["ID"]);
            }

            comboBox1.SelectedItem = "None";

        }

        private void _LoadDeptComboBox()
        {
            DataTable Dept = clsDepartments.getAllDepartments();
            CBDept.DisplayMember = "Name";
            CBDept.ValueMember = "ID";
            CBDept.DataSource = Dept;
            CBDept.SelectedIndex = -1;

        }

        private void _LoadCountriesComboBox()
        {
            DataTable Dept = clsCountry.getAllCountries();
            CBcounty.DisplayMember = "Name";
            CBcounty.ValueMember = "ID";
            CBcounty.DataSource = Dept;
            CBcounty.SelectedIndex = -1;

        }
        private void _LoadCBFilter()
        {
            DataTable table = ClsEmployee.GetAllEmployees();

            // أضف None أول شي
            cbFilter.Items.Add("None");

        
            foreach (DataColumn column in table.Columns)
            {
                cbFilter.Items.Add(column.ColumnName);
            }

            cbFilter.SelectedItem = "None";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _LoadTable();
            _LoadIDComboBox();
            _LoadCountriesComboBox();
            _LoadDeptComboBox();
            _LoadCBFilter();
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "None")
            {

                if (int.TryParse(comboBox1.SelectedItem.ToString(), out int SelectID))
                {
                    string Filter = $"ID = {SelectID}";
                    V.RowFilter = Filter;
                }
            }
            else
            {
                if (V != null) V.RowFilter = "";

            }
        }



        private void btnSort_Click(object sender, EventArgs e)
        {
            if (txtSort.Text == "")
            {
                return;
            }

            string Sort = txtSort.Text;
            try
            {
                V.Sort = Sort;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter Sort Corecctly like [ID Asc]");
                txtSort.Clear();
            }

        }

        private void LoadFiledsByID(int id)
        {
            ClsEmployee emp = ClsEmployee.FindByID(id);

            comboBox1.SelectedItem = id;
            CBDept.SelectedValue = emp.DepartmentID;

            CBcounty.SelectedValue = emp.CountryID;

            if (emp.DateOfBirth.HasValue)
            {
                dateTimeBirth.Value = (DateTime)emp.DateOfBirth;
            }
            else
            {

                dateTimeBirth.Value = DateTime.Now;
            }


            dateTimePHire.Value = emp.HireDate;

            if (emp.ExitDate.HasValue)
            {
                dateTimeExit.Value = (DateTime)emp.ExitDate;
            }
            else
            {

                dateTimeExit.Value = DateTime.Now;
            }

            txtBouns.Text = emp.Bouns.ToString();
            txtFirstName.Text = emp.FirstName;
            txtLastName.Text = emp.LastName;
            txtSalary.Text = emp.Salary.ToString();

            if (emp.Gendor == 'F')
            {
                RBf.Checked = true;
            }
            else
            {
                RBmale.Checked = true;

            }


        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0) { return; }
            int id = (int)dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;

            LoadFiledsByID(id);



        }



        private void btnShowAll_Click_1(object sender, EventArgs e)
        {
            _LoadTable();
            ClearBoxes();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "None")
            {
                return;
            }
            if (MessageBox.Show("Are You sure to delete this Contact ?" + comboBox1.SelectedItem.ToString()) == DialogResult.OK)
            {
                int id;
                if (int.TryParse(comboBox1.SelectedItem.ToString(), out id))
                {
                    if (ClsEmployee.DeleteEmployee((int)dataGridView1.CurrentRow.Cells[0].Value))
                    {
                        MessageBox.Show("Deleted Successfully");
                    }
                }
                else
                {
                    MessageBox.Show("Not Delete...");
                }

                _LoadTable();
                _LoadIDComboBox();

            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            if (_Add())
            {
                _LoadTable();
                _LoadIDComboBox();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!_CheckData())
            {
                return;
            }



            string FirstName = txtFirstName.Text;
            string LastName = txtLastName.Text;
            char gendor = RBf.Checked ? 'F' : 'M';

            decimal.TryParse(txtSalary.Text, out decimal Salary);
            double.TryParse(txtBouns.Text, out double Bouns);

            int DeptID = Convert.ToInt32(CBDept.SelectedValue);
            int countryID = Convert.ToInt32(CBcounty.SelectedValue);


            DateTime Birth = dateTimeBirth.Value;
            DateTime HireDate = dateTimePHire.Value;
            DateTime? ExitDate;

            if (dateTimeExit.Value == DateTime.Now)
            {
                ExitDate = null;
            }
            else
            {
                ExitDate = dateTimeExit.Value;
            }

            Emp = ClsEmployee.FindByID(int.Parse(comboBox1.SelectedItem.ToString()));


            Emp.FirstName = FirstName;
            Emp.LastName = LastName;
            Emp.Salary = Salary;
            Emp.Bouns = Bouns;
            Emp.ExitDate = ExitDate;
            Emp.HireDate = HireDate;
            Emp.CountryID = countryID;
            Emp.DepartmentID = DeptID;
            Emp.DateOfBirth = Birth;
            Emp.Gendor = gendor;

            if (Emp.Save())
            {
                MessageBox.Show($"Employee Updated");
                ClearBoxes();
                comboBox1.SelectedItem = "None";
                _LoadTable();

            }
            else
            {
                MessageBox.Show($"Failed To Save.");
            }
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilter.SelectedItem.ToString() == "None")
            {
                txtFilter.Enabled = false;

            }
            else
            {
                txtFilter.Enabled = true;
                txtFilter.Focus();

            }
          
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFilter.Text))
            {
                if (txtFilter.Text == "CountryID" || txtFilter.Text == "DepartmentID" || txtFilter.Text == "ID" || txtFilter.Text == "MonthlySalary" || txtFilter.Text == "BounsPerc")
                {
                    try
                    {
                        V.RowFilter = $"{cbFilter.SelectedItem.ToString()} = {txtFilter.Text}";
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        V.RowFilter = $"{cbFilter.SelectedItem.ToString()} = '{txtFilter.Text}'";
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                V.RowFilter = "";

            }
        }
    }
}
