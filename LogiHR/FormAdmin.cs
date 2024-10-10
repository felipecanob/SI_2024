using MaterialSkin.Controls;
using MaterialSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Net.Mail;
using System.IO;

namespace LogiHR
{
    public partial class FormAdmin : MaterialForm
    {
        

        private DataAccess Da { get; set; }

        readonly MaterialSkin.MaterialSkinManager materialSkinManager;
        private FormLogin Fl { get; set; }


        public class EmailConfig
        {
            public string FromEmail { get; set; }
            public string SmtpServer { get; set; }
            public int SmtpPort { get; set; }
            public string SmtpUser { get; set; }
            public string SmtpPassword { get; set; }
            public string ToEmail { get; set; }
        }
        private EmailConfig emailConfig;
        public FormAdmin()
        {
            
            // Initializing materialskin
            InitializeComponent();
            LoadEmailConfig();
            this.Da = new DataAccess();
            this.PopulateEmployeeGridView();
            PopulateGridViewRawMeterials();
            PopulateGridViewFinishedProduct();

            // Initializin Materialskin
            materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Amber700, Primary.Amber900, Primary.Amber800, Accent.Amber200, TextShade.WHITE);


            // Raw materials Combo box Category (Filter)
            ArrayList RawCategory = new ArrayList { "Fabrics", "Buttons", "Threads", "Miscellaneous" };
            this.cmbRawMaterialsCategoryInput.DataSource = RawCategory;


            // Finished Product Combo box Category (Filter)
            ArrayList FinishedCategory = new ArrayList { "T-Shirts", "Bottoms weares", "Outerwears", "Workwears" };
            this.cmbFinishedProductCategoryInput.DataSource = FinishedCategory;

        }

        

        // Raw Materials Items
        ArrayList fabrics = new ArrayList { "Cotton", "Polyester", "Wool" };
        ArrayList buttons = new ArrayList { "Jeans buttons", "Rivets for reinforcement" };
        ArrayList threads = new ArrayList { "Sewing thread", "Embroidery thread", "Yarn for knitted" };
        ArrayList miscellaneous = new ArrayList { "Hangers", "Garment bags" };

        // Finished Product Items
        ArrayList tShirts = new ArrayList { "Basic T-shirts", "Polo shirts", "Sweaters" };
        ArrayList pants = new ArrayList { "Jeans", "Pants", "Shorts" };
        ArrayList outerwears = new ArrayList { "Jackets", "Coats", "Blazers", "Hoodies" };
        ArrayList workwears = new ArrayList { "Business suits", "Work shirts", "Chef uniforms" };

        // Getting Logged In Admin Id
        // Getting Manager request for reseting password

        private string AdminId;
        public FormAdmin(string info, string id,string Mid, FormLogin fl) : this()
        {
            this.Fl = fl;
            this.Text += info;
            AdminId = id;

            // Set employee id to reset in admin form
            this.lblPasswordReset.Text += Mid;
            this.lblPasswordReset.ForeColor = Color.Red;
        }

        // Set visibility to false of reset password label after click
        private void lblPasswordReset_Click(object sender, EventArgs e)
        {
            this.lblPasswordReset.Visible = false;
        }




        // Raw materials Combo box Products items filter
        private void cmbRawMaterialsCategoryInput_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var categoryChoose = this.cmbRawMaterialsCategoryInput.Text.ToLower();

            try
            {
                switch (categoryChoose)
                {
                    case "fabrics":
                        this.cmbRawProductNameInput.DataSource = fabrics;
                        break;
                    case "buttons":
                        this.cmbRawProductNameInput.DataSource = buttons;
                        break;
                    case "threads":
                        this.cmbRawProductNameInput.DataSource = threads;
                        break;
                    case "miscellaneous":
                        this.cmbRawProductNameInput.DataSource = miscellaneous;
                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Invalid Category Choose" + err.Message);
            }

        }



        // Finished Product Combo box Products items filter

        private void cmbFinishedProductCategoryInput_SelectionChangeCommitted_1(object sender, EventArgs e)
        {
            var categoryChoose = this.cmbFinishedProductCategoryInput.Text.ToLower();

            try
            {
                switch (categoryChoose)
                {
                    case "t-shirts":
                        this.cmbFinishedProductNameInput.DataSource = tShirts;
                        break;
                    case "bottoms weares":
                        this.cmbFinishedProductNameInput.DataSource = pants;
                        break;
                    case "outerwears":
                        this.cmbFinishedProductNameInput.DataSource = outerwears;
                        break;
                    case "workwears":
                        this.cmbFinishedProductNameInput.DataSource = workwears;
                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Invalid Category Choose" + err.Message);
            }
        }


        // Admin Logging Out
        private void btnLogOutAdmin_Click(object sender, EventArgs e)
        {
            this.Hide();
            MessageBox.Show("Logged Out");
            this.Fl.Show();
            
        }


        // ********************************CRUD OPERATION FOR EMPLOYEES***************************************


        // Populate Grid View From Manager Table
        private void PopulateEmployeeGridView(string sql = "select * from Manager_info;")
        {
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvEmployeeInformationShow.AutoGenerateColumns = false;
            this.dgvEmployeeInformationShow.DataSource = ds.Tables[0];
        }


        // Auto Search By Employee Name
        private void txtSearchEmployee_TextChanged(object sender, EventArgs e)
        {
            var sql = "select * from Manager_Info where M_Name like '" + this.txtSearchEmployee.Text + "%';";
            this.PopulateEmployeeGridView(sql);
        }
        string gender;

        // Insert and Update Operation of Employees
        private void btnSaveEmployeeInfo_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int empId = random.Next(50, 100000);
            txtEmployeeId.Text = empId.ToString();

            try
            {
                
                if (!this.IsValidToSave())
                {  
                    MessageBox.Show("Empty fields are not allowed. Please fill all the information.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var query = "select * from Manager_info where M_id = '" + this.txtEmployeeId.Text + "';";
                var dt = this.Da.ExecuteQuery(query);
                if (dt.Tables[0].Rows.Count == 1)
                {
                    

                    if (this.rdbEmployeeFemale.Checked == true)
                    {
                        gender = "Female";
                    }
                    else if (this.rdbEmployeeMale.Checked == true) 
                    {
                        gender = "Male";
                    }
                    //update
                    var sql = @"update Manager_info
                                set M_Name = '" + this.txtEmployeeName.Text + @"',
                                M_Gender = '" + gender + @"',
                                M_Address = '" + this.txtEmployeeAddress.Text + @"',
                                M_Role = '" + this.cmbEmployeeRole.Text + @"',
                                M_Joining_Date = '" + this.dtpJoiningDate.Text + @"',
                                M_Salary = '" + this.txtEmployeeSalary.Text + @"',
                                M_Password = '" + this.txtEmployeePassword.Text + @"',
                                A_Id = '" + AdminId + @"'
                                where M_Id = '" + this.txtEmployeeId.Text + "';";
                    var count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data updated properly.");
                    else
                        MessageBox.Show("Failure while updating the data.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                    var queryIdA = "select * from Admin_info where A_id = '" + this.txtEmployeeId.Text + "';";
                    var idExistA = this.Da.ExecuteQuery(queryIdA);

                    if (idExistA.Tables[0].Rows.Count == 1)
                    {
                       
                        MessageBox.Show("Can not take this ID", "Id already taken", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //insert
                        var sql = "insert into Manager_info values('" + this.txtEmployeeId.Text + "', '" + this.txtEmployeeName.Text + "', '" + gender + "', '" + this.txtEmployeeAddress.Text + "', '" + this.cmbEmployeeRole.Text + "', '" + this.dtpJoiningDate.Text + "', '" + this.txtEmployeeSalary.Text + "', '" + this.txtEmployeePassword.Text + "', '" + AdminId + "'); ";
                        var count = this.Da.ExecuteDMLQuery(sql);

                        if (count == 1)
                            MessageBox.Show("Data saved properly.");
                        else
                            MessageBox.Show("Failure while updating the data.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

                this.PopulateEmployeeGridView();
                /*this.ClearAll();*/
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured due to invalid input.\nError Info:" + exc.Message);
            }
        }


        // Check Wheather all the data are entered in employee form
        private bool IsValidToSave()
        {
            if (String.IsNullOrEmpty(this.txtEmployeeId.Text) || String.IsNullOrEmpty(this.txtEmployeeName.Text) ||
               String.IsNullOrEmpty(this.txtEmployeePassword.Text) || String.IsNullOrEmpty(this.txtEmployeeSalary.Text) ||
               String.IsNullOrEmpty(this.txtEmployeeAddress.Text) || String.IsNullOrEmpty(this.cmbEmployeeRole.Text) ||
               !this.rdbEmployeeFemale.Checked && !this.rdbEmployeeMale.Checked)
            {
                return false;
            }
            else
                return true;
        }

        
        // Entering row value by double clicking on the row
        private void dgvEmployeeInformationShow_DoubleClick_1(object sender, EventArgs e)
        {
            this.txtEmployeeId.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_id"].Value.ToString();
            this.txtEmployeeName.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Name"].Value.ToString();
            this.txtEmployeePassword.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Password"].Value.ToString();
            this.txtEmployeeSalary.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Salary"].Value.ToString();
            this.txtEmployeeAddress.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Address"].Value.ToString();
            this.cmbEmployeeRole.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Role"].Value.ToString();
            this.dtpJoiningDate.Text = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Joining_Date"].Value.ToString();
        }

        // Form closed Event
        private void FormAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            MessageBox.Show("Logged Out");
            this.Fl.Show();
        }


        // Changing Theme to dark and light mode
        private void switchThemeChangeEmployee_CheckedChanged(object sender, EventArgs e)
        {
            if (switchThemeChangeEmployee.Checked)
            {
                materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
                this.switchThemeChangeEmployee.Text = "Light Mode";
                this.dgvEmployeeInformationShow.DefaultCellStyle.BackColor = Color.Black;
                this.dgvEmployeeInformationShow.DefaultCellStyle.ForeColor = Color.White;
                this.dgvEmployeeInformationShow.BackgroundColor = Color.Black;


                this.dgvFinishedProductShow.DefaultCellStyle.BackColor = Color.Black;
                this.dgvFinishedProductShow.DefaultCellStyle.ForeColor = Color.White;
                this.dgvFinishedProductShow.BackgroundColor = Color.Black;


                this.dgvRawMaterialsShow.DefaultCellStyle.BackColor = Color.Black;
                this.dgvRawMaterialsShow.DefaultCellStyle.ForeColor = Color.White;
                this.dgvRawMaterialsShow.BackgroundColor = Color.Black;
            }
            else
            {
                materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
                this.switchThemeChangeEmployee.Text = "Dark Mode";
                this.dgvEmployeeInformationShow.DefaultCellStyle.BackColor = Color.White;
                this.dgvEmployeeInformationShow.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvEmployeeInformationShow.BackgroundColor = Color.White;


                this.dgvFinishedProductShow.DefaultCellStyle.BackColor = Color.White;
                this.dgvFinishedProductShow.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvFinishedProductShow.BackgroundColor = Color.White;


                this.dgvRawMaterialsShow.DefaultCellStyle.BackColor = Color.White;
                this.dgvRawMaterialsShow.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvRawMaterialsShow.BackgroundColor = Color.White;
            }
        }


        // Deleting Employee
        private void btnDeleteEmployeeInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvEmployeeInformationShow.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a Row first to delete the data", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var id = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Id"].Value.ToString();
                var name = this.dgvEmployeeInformationShow.CurrentRow.Cells["M_Name"].Value.ToString();

                DialogResult result = MessageBox.Show($"Are you sure you want to delete {name}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }

                var sql = "delete from Manager_info where M_id = '" + id + "';";
                var count = this.Da.ExecuteDMLQuery(sql);

                if (count == 1)
                    MessageBox.Show(name + " employee removed properly.");
                else
                    MessageBox.Show("Failure while updating the data.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.PopulateEmployeeGridView();
               
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured due to invalid choice.\nError Info:" + exc.Message);
            }
        }





        // ********************************CRUD OPERATION FOR RAW MATERIALS AND PRODUCTS***************************************



        //curd Operations


        //************************************************Raw Meterials CRUD*********************************************

        private void PopulateGridViewRawMeterials(string sql = "select * from RawMeterialInfo;")
        {
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvRawMaterialsShow.AutoGenerateColumns = false;
            this.dgvRawMaterialsShow.DataSource = ds.Tables[0];
        }



        private void ClearAllRawMeterialInput()
        {
            this.txtRawMaterialsId.Clear();
            this.cmbRawMaterialsCategoryInput.SelectedIndex = -1;
            this.cmbRawProductNameInput.SelectedIndex = -1;
            this.txtRawQuantityInput.Clear();
            this.txtRawCostInput.Clear();
            this.dtpRawMaterialReceiveDate.Text = "";


            this.txtSearchRawMaterial.Clear();
            this.dgvRawMaterialsShow.ClearSelection();
        }

        private bool IsValidToSaveRawMeterial()
        {
            if (String.IsNullOrEmpty(this.txtRawMaterialsId.Text) || String.IsNullOrEmpty(this.cmbRawMaterialsCategoryInput.Text) ||
               String.IsNullOrEmpty(this.cmbRawProductNameInput.Text) || String.IsNullOrEmpty(this.txtRawQuantityInput.Text) ||
               String.IsNullOrEmpty(this.txtRawCostInput.Text) || String.IsNullOrEmpty(this.dtpRawMaterialReceiveDate.Text))
            {
                return false;
            }
            else
                return true;
        }


       
        private void btnSaveRawMaterials_Click(object sender, EventArgs e)
        {

            Random random = new Random();
            int empId = random.Next(50, 100000);
            txtRawMaterialsId.Text = empId.ToString();

            try
            {
                if (!this.IsValidToSaveRawMeterial())
                {
                    MessageBox.Show("Empty fields are not allowed. Please fill all the information");
                    return;
                }

                var query = "select * from RawMeterialInfo where RawId = '" + this.txtRawMaterialsId.Text + "';";

                var dt = this.Da.ExecuteQuery(query);
                if (dt.Tables[0].Rows.Count == 1)
                {
                    //update raw materials
                    var sql = @"update RawMeterialInfo
                                set 
                                RawCategory = '" + this.cmbRawMaterialsCategoryInput.Text + @"',
                                RawProductName = '" + this.cmbRawProductNameInput.Text + @"',
                                RawQuantity = " + this.txtRawQuantityInput.Text + @",
                                RawCost = " + this.txtRawCostInput.Text + @",
                                RawDate = '" + this.dtpRawMaterialReceiveDate.Text + @"'
                                where RawId = '" + this.txtRawMaterialsId.Text + "';";
                    var count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data updated properly.");
                    else
                        MessageBox.Show("Failure While updating the data");
                }
                else
                {
                    //insert raw materials
                    var sql = "insert into RawMeterialInfo values('" + this.txtRawMaterialsId.Text + "', '" + this.cmbRawMaterialsCategoryInput.Text + "', '" + this.cmbRawProductNameInput.Text + "', " + this.txtRawQuantityInput.Text + ", " + this.txtRawCostInput.Text + ", '" + this.dtpRawMaterialReceiveDate.Text + "'); ";
                    var count = this.Da.ExecuteDMLQuery(sql);
                    

                    if (count == 1)
                        MessageBox.Show("Data saved properly.");
                    else
                        MessageBox.Show("Failure While saving the data");

                }

                this.PopulateGridViewRawMeterials();
                this.ClearAllRawMeterialInput();
              
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured due to invalid input.\nError Info:" + exc.Message);
            }
        }


        // Deleting raw materials
        private void btnDeleteRawMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvRawMaterialsShow.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a Row first to delete the data", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var id = this.dgvRawMaterialsShow.CurrentRow.Cells[0].Value.ToString();
                var ProductName = this.dgvRawMaterialsShow.CurrentRow.Cells[2].Value.ToString();

                DialogResult result = MessageBox.Show($"Are you sure you want to delete {ProductName}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }

                var sql = "delete from RawMeterialInfo where RawId = '" + id + "';";
                var count = this.Da.ExecuteDMLQuery(sql);

                if (count == 1)
                    MessageBox.Show(ProductName + " removed properly.");
                else
                    MessageBox.Show("Failure While removing the data");

                this.PopulateGridViewRawMeterials();
                this.ClearAllRawMeterialInput();
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured due to invalid choice.\nError Info:" + exc.Message);
            }
        }


        // Double click event to auto enter the row items from table
        private void dgvRawMaterialsShow_DoubleClick(object sender, EventArgs e)
        {
            this.txtRawMaterialsId.Text = this.dgvRawMaterialsShow.CurrentRow.Cells[0].Value.ToString();
            this.cmbRawMaterialsCategoryInput.Text = this.dgvRawMaterialsShow.CurrentRow.Cells[1].Value.ToString();
            this.cmbRawProductNameInput.Text = this.dgvRawMaterialsShow.CurrentRow.Cells[2].Value.ToString();
            this.txtRawQuantityInput.Text = this.dgvRawMaterialsShow.CurrentRow.Cells[3].Value.ToString();
            this.txtRawCostInput.Text = this.dgvRawMaterialsShow.CurrentRow.Cells[4].Value.ToString();
            this.dtpRawMaterialReceiveDate.Text = this.dgvRawMaterialsShow.CurrentRow.Cells[5].Value.ToString();
        }






        //*************************************Finished Products CRUD************************************

        private void txtFinishedProductSearch_TextChanged(object sender, EventArgs e)
        {
            var sql = "select * from FinishedProductInfo where FinishedCatagory like '" + this.txtFinishedProductSearch.Text + "%';";
            this.PopulateGridViewFinishedProduct(sql);
        }

        private void PopulateGridViewFinishedProduct(string sql = "select * from FinishedProductInfo;")
        {
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvFinishedProductShow.AutoGenerateColumns = false;
            this.dgvFinishedProductShow.DataSource = ds.Tables[0];
        }

        private void ClearAllFinishedProduct()
        {
            this.txtFinishedProductId.Clear();
            this.cmbFinishedProductCategoryInput.SelectedIndex = -1;
            this.cmbFinishedProductNameInput.SelectedIndex = -1;
            this.txtFinishedProductQuantityInput.Clear();
            this.txtFinishedProductCostInput.Clear();
            this.dtpManufacturedDateInput.Text = "";


            this.txtFinishedProductSearch.Clear();
            this.dgvFinishedProductShow.ClearSelection();
        }

        private bool IsValidToSaveFinishedProduct()
        {
            if (String.IsNullOrEmpty(this.txtFinishedProductId.Text) || String.IsNullOrEmpty(this.cmbFinishedProductCategoryInput.Text) ||
               String.IsNullOrEmpty(this.cmbFinishedProductNameInput.Text) || String.IsNullOrEmpty(this.txtFinishedProductQuantityInput.Text) ||
               String.IsNullOrEmpty(this.txtFinishedProductCostInput.Text) || String.IsNullOrEmpty(this.dtpManufacturedDateInput.Text))
            {
                return false;
            }
            else
                return true;
        }


      
        private void btnFinishedProductSave_Click(object sender, EventArgs e)
        {

            Random random = new Random();
            int empId = random.Next(50, 100000);
            txtFinishedProductId.Text = empId.ToString();

            try
            {
                if (!this.IsValidToSaveFinishedProduct())
                {
                    MessageBox.Show("Empty fields are not allowed. Please fill all the information");
                    return;
                }

                var query = "select * from FinishedProductInfo where FinishedProductId = '" + this.txtFinishedProductId.Text + "';";

                var dt = this.Da.ExecuteQuery(query);
                if (dt.Tables[0].Rows.Count == 1)
                {
                    //update finished products
                    var sql = @"update FinishedProductInfo
                                set 
                                FinishedCatagory = '" + this.cmbFinishedProductCategoryInput.Text + @"',
                                FinishedProductName = '" + this.cmbFinishedProductNameInput.Text + @"',
                                FinishedQuantity = " + this.txtFinishedProductQuantityInput.Text + @",
                                FinishedCost = '" + this.txtFinishedProductCostInput.Text + @"',
                                FinishDate = '" + this.dtpManufacturedDateInput.Text + @"'
                                where FinishedProductId = '" + this.txtFinishedProductId.Text + "';";
                    var count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data updated properly.");
                    else
                        MessageBox.Show("Failure While updating the data");
                }
                else
                {
                    //insert finished products
                    var sql = "insert into FinishedProductInfo values('" + this.txtFinishedProductId.Text + "', '" + this.cmbFinishedProductCategoryInput.Text + "', '" + this.cmbFinishedProductNameInput.Text + "', " + this.txtFinishedProductQuantityInput.Text + ", " + this.txtFinishedProductCostInput.Text + ", '" + this.dtpManufacturedDateInput.Text + "'); ";
                    var count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data saved properly.");
                    else
                        MessageBox.Show("Failure While saving the data");

                }

                this.PopulateGridViewFinishedProduct();
                this.ClearAllFinishedProduct();
               
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured due to invalid input.\nError Info:" + exc.Message);
            }
        }

        private void btnFinishedProductDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvFinishedProductShow.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a Row first to delete the data", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var id = this.dgvFinishedProductShow.CurrentRow.Cells[0].Value.ToString();
                var ProductName = this.dgvFinishedProductShow.CurrentRow.Cells[2].Value.ToString();

                DialogResult result = MessageBox.Show($"Are you sure you want to delete {ProductName}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }

                var sql = "delete from FinishedProductInfo where FinishedProductId = '" + id + "';";
                var count = this.Da.ExecuteDMLQuery(sql);

                if (count == 1)
                    MessageBox.Show(ProductName + " removed properly.");
                else
                    MessageBox.Show("Failure While removing the data");

                this.PopulateGridViewFinishedProduct();
                this.ClearAllFinishedProduct();
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured due to invalid choice.\nError Info:" + exc.Message);
            }
        }

        private void btnManagerLogOut_Click(object sender, EventArgs e)
        {
            this.Hide();
            MessageBox.Show("Logged Out");
            this.Fl.Close();
        }

        private void dgvFinishedProductShow_DoubleClick_1(object sender, EventArgs e)
        {
            this.txtFinishedProductId.Text = this.dgvFinishedProductShow.CurrentRow.Cells[0].Value.ToString();
            this.cmbFinishedProductCategoryInput.Text = this.dgvFinishedProductShow.CurrentRow.Cells[1].Value.ToString();
            this.cmbFinishedProductNameInput.Text = this.dgvFinishedProductShow.CurrentRow.Cells["ProductName"].Value.ToString();
            this.txtFinishedProductQuantityInput.Text = this.dgvFinishedProductShow.CurrentRow.Cells[3].Value.ToString();
            this.txtFinishedProductCostInput.Text = this.dgvFinishedProductShow.CurrentRow.Cells[4].Value.ToString();
            this.dtpManufacturedDateInput.Text = this.dgvFinishedProductShow.CurrentRow.Cells[5].Value.ToString();
        }

        // Raw Materials Search product
        private void txtSearchRawMaterial_TextChanged(object sender, EventArgs e)
        {
            var sql = "select * from RawMeterialInfo where Rawcategory like '" + this.txtSearchRawMaterial.Text + "%';";
            this.PopulateGridViewRawMeterials(sql);
        }



        // Printing Invoice for raw materials

        string billRawId = "BillRaw-";
        int countRaw = 1;
        private void printInvoiceDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            printInvoiceDocument.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("Custom", 600, 800);

            printInvoiceDocument.PrinterSettings.DefaultPageSettings.Landscape = false;



            e.Graphics.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230));

            e.Graphics.DrawString("Bill Id: " + billRawId + countRaw, new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 70));



            e.Graphics.DrawString("Material Id: " + dgvRawMaterialsShow.SelectedRows[0].Cells[0].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 100));

            e.Graphics.DrawString("Category Name: " + dgvRawMaterialsShow.SelectedRows[0].Cells[1].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 130));

            e.Graphics.DrawString("Product Name: " + dgvRawMaterialsShow.SelectedRows[0].Cells[2].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 160));

            e.Graphics.DrawString("Quantity: " + dgvRawMaterialsShow.SelectedRows[0].Cells[3].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 190));
            e.Graphics.DrawString("Total Cost: " + dgvRawMaterialsShow.SelectedRows[0].Cells[3].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 220));
            

            e.Graphics.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230, 400));



            e.HasMorePages = false;
            countRaw++;

        }

        private void btnPrintRawMaterialInvoice_Click(object sender, EventArgs e)
        {
            printPreviewInvoice.Document = printInvoiceDocument;
            printPreviewInvoice.ShowDialog();
        }



        // Printing Invoice for Finished Product
        string billFinishedId = "BillFinished-";
        int countFinished = 1;
        private void btnPrintFinishedProduct_Click(object sender, EventArgs e)
        {
            printFinishedPreviewDialog.Document = printFinishedDocument;
            printFinishedPreviewDialog.ShowDialog();
        }

        private void printFinishedDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            printFinishedDocument.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("Custom", 600, 800);

            printFinishedDocument.PrinterSettings.DefaultPageSettings.Landscape = false;



            e.Graphics.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230));

            e.Graphics.DrawString("Bill Id: " + billFinishedId + countFinished, new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 70));



            e.Graphics.DrawString("Product Id: " + dgvFinishedProductShow.SelectedRows[0].Cells[0].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 100));

            e.Graphics.DrawString("Category Name: " + dgvFinishedProductShow.SelectedRows[0].Cells[1].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 130));

            e.Graphics.DrawString("Product Name: " + dgvFinishedProductShow.SelectedRows[0].Cells[2].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 160));

            e.Graphics.DrawString("Quantity: " + dgvFinishedProductShow.SelectedRows[0].Cells[3].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 190));
            e.Graphics.DrawString("Total Cost: " + dgvFinishedProductShow.SelectedRows[0].Cells[3].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 220));
            

            e.Graphics.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230, 400));



            e.HasMorePages = false;
            countFinished++;
        }
        private void LoadEmailConfig()
        {
            try
            {
                emailConfig = new EmailConfig();

                // Load the XML file
                XmlDocument xmlDoc = new XmlDocument();
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string xmlFilePath = Path.Combine(exeDirectory, "Config.xml");
                xmlDoc.Load(xmlFilePath);

                // Parse the XML and populate the emailConfig object
                XmlNode emailSettings = xmlDoc.SelectSingleNode("configuration/emailSettings");

                emailConfig.FromEmail = emailSettings["fromEmail"].InnerText;
                emailConfig.SmtpServer = emailSettings["smtpServer"].InnerText;
                emailConfig.SmtpPort = int.Parse(emailSettings["smtpPort"].InnerText);
                emailConfig.SmtpUser = emailSettings["smtpUser"].InnerText;
                emailConfig.SmtpPassword = emailSettings["smtpPassword"].InnerText;
                emailConfig.ToEmail = emailSettings["toEmail"].InnerText;

                MessageBox.Show("Email configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading email configuration: " + ex.Message);
            }
        }
        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            // Create a bitmap to simulate drawing on the printer page
            int width = 600;
            int height = 800;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Draw the same content that you would draw in the print document
                g.Clear(Color.White); // White background

                // Simulate the same drawing as in the print document
                g.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230));
                g.DrawString("Bill Id: " + billFinishedId + countFinished, new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 70));
                g.DrawString("Product Id: " + dgvFinishedProductShow.SelectedRows[0].Cells[0].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 100));
                g.DrawString("Category Name: " + dgvFinishedProductShow.SelectedRows[0].Cells[1].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 130));
                g.DrawString("Product Name: " + dgvFinishedProductShow.SelectedRows[0].Cells[2].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 160));
                g.DrawString("Quantity: " + dgvFinishedProductShow.SelectedRows[0].Cells[3].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 190));
                g.DrawString("Total Cost: " + dgvFinishedProductShow.SelectedRows[0].Cells[4].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 220));
                g.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230, 400));
            }

            // Save the Bitmap to a file (for example, as a PNG)
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FinishedProductInvoice.png");
            bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            // Send the email with the saved image attached
            SendEmailWithAttachment(filePath);

        }
        private void btnSendEmail_Click2(object sender, EventArgs e)
        {
            // Create a bitmap to simulate drawing on the printer page
            int width = 600;
            int height = 800;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Draw the same content that you would draw in the print document
                g.Clear(Color.White); // White background

                // Simulate the same drawing as in the print document
                g.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230));
                g.DrawString("Bill Id: " + billFinishedId + countFinished, new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 70));
                g.DrawString("Product Id: " + dgvRawMaterialsShow.SelectedRows[0].Cells[0].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 100));
                g.DrawString("Category Name: " + dgvRawMaterialsShow.SelectedRows[0].Cells[1].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 130));
                g.DrawString("Product Name: " + dgvRawMaterialsShow.SelectedRows[0].Cells[2].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 160));
                g.DrawString("Quantity: " + dgvRawMaterialsShow.SelectedRows[0].Cells[3].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 190));
                g.DrawString("Total Cost: " + dgvRawMaterialsShow.SelectedRows[0].Cells[4].Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Blue, new Point(100, 220));
                g.DrawString("Wizard Fashion", new Font("Arial", 25, FontStyle.Bold), Brushes.DimGray, new Point(230, 400));
            }

            // Save the Bitmap to a file (for example, as a PNG)
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FinishedProductInvoice.png");
            bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            // Send the email with the saved image attached
            SendEmailWithAttachment(filePath);

        }
        private void SendEmailWithAttachment(string filePath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(emailConfig.SmtpServer);
                mail.From = new MailAddress(emailConfig.FromEmail);
                mail.To.Add(emailConfig.ToEmail);
                mail.Subject = "Invoice LogiHR";
                mail.Body = "Please find attached the invoice for the finished product.";

                // Attach the saved document (PNG image)
                Attachment attachment = new Attachment(filePath);
                mail.Attachments.Add(attachment);

                // SMTP server configuration
                SmtpServer.Port = emailConfig.SmtpPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(emailConfig.SmtpUser, emailConfig.SmtpPassword);
                SmtpServer.EnableSsl = true;

                // Send the email
                SmtpServer.Send(mail);
                MessageBox.Show("Invoice email sent successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            // Open a SaveFileDialog to choose where to save the CSV file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Save Selected Rows to CSV";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the file path from the SaveFileDialog
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Collect the selected rows from the DataGridView
                    List<DataGridViewRow> selectedRows = dgvRawMaterialsShow.SelectedRows.Cast<DataGridViewRow>().ToList();

                    // Write the selected rows to the CSV file
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        // Write the header row
                        string header = string.Join(",", dgvRawMaterialsShow.Columns.Cast<DataGridViewColumn>().Select(column => column.HeaderText));
                        sw.WriteLine(header);

                        // Write each selected row's data
                        foreach (DataGridViewRow row in selectedRows)
                        {
                            if (!row.IsNewRow)
                            {
                                var cells = row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value?.ToString() ?? string.Empty);
                                sw.WriteLine(string.Join(",", cells));
                            }
                        }
                    }

                    MessageBox.Show("Selected rows successfully saved to CSV!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the CSV file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void materialButton1_Click2(object sender, EventArgs e)
        {
            // Open a SaveFileDialog to choose where to save the CSV file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Save Selected Rows to CSV";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the file path from the SaveFileDialog
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Collect the selected rows from the DataGridView
                    List<DataGridViewRow> selectedRows = dgvFinishedProductShow.SelectedRows.Cast<DataGridViewRow>().ToList();

                    // Write the selected rows to the CSV file
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        // Write the header row
                        string header = string.Join(",", dgvFinishedProductShow.Columns.Cast<DataGridViewColumn>().Select(column => column.HeaderText));
                        sw.WriteLine(header);

                        // Write each selected row's data
                        foreach (DataGridViewRow row in selectedRows)
                        {
                            if (!row.IsNewRow)
                            {
                                var cells = row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value?.ToString() ?? string.Empty);
                                sw.WriteLine(string.Join(",", cells));
                            }
                        }
                    }

                    MessageBox.Show("Selected rows successfully saved to CSV!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the CSV file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

}
