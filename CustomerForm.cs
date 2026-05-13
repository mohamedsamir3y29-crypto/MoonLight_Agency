using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoonLight_Agency
{
    public partial class CustomerForm : Form
    {
        // سطر الاتصال الخاص بك
        SqlConnection con = new SqlConnection(@"Data Source=ELSHENAWY\SQLEXPRESS;Initial Catalog=MoonLight_DB;Integrated Security=True;Encrypt=False;");

        public CustomerForm()
        {
            InitializeComponent();
        }

        // دالة عرض البيانات في الجدول وتنسيقه
        void FillGrid()
        {
            try
            {
                if (con.State == ConnectionState.Open) con.Close();
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Customers", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvCustomers.DataSource = dt;

                // تنسيق العناوين بالعربي
                if (dgvCustomers.Columns.Count > 0)
                {
                    dgvCustomers.Columns[0].HeaderText = "كود العميل";
                    dgvCustomers.Columns[1].HeaderText = "اسم العميل";
                    dgvCustomers.Columns[2].HeaderText = "رقم الهاتف";
                    dgvCustomers.Columns[3].HeaderText = "رقم الباسبور";
                    dgvCustomers.Columns[4].HeaderText = "نوع الرحلة";
                    dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("مشكلة في عرض الجدول: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    MessageBox.Show("يرجى إدخال اسم العميل أولاً");
                    return;
                }
                con.Open();
                string query = "INSERT INTO Customers (CustomerName, Phone, PassportNo, TripType) VALUES ('" + txtName.Text + "', '" + txtPhone.Text + "', '" + txtPassport.Text + "', '" + cbTripType.Text + "')";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("تم الحفظ بنجاح");
                FillGrid();
                btnClear_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في الحفظ: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtPhone.Clear();
            txtPassport.Clear();
            cbTripType.SelectedIndex = -1;
            txtName.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string customerID = dgvCustomers.CurrentRow.Cells[0].Value.ToString();
                con.Open();
                string query = "UPDATE Customers SET CustomerName = '" + txtName.Text + "', Phone = '" + txtPhone.Text + "', PassportNo = '" + txtPassport.Text + "', TripType = '" + cbTripType.Text + "' WHERE CustomerID = " + customerID;
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("تم التعديل بنجاح!");
                FillGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في التعديل: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text)) { MessageBox.Show("اختار العميل اللي عاوز تمسحه"); return; }
                DialogResult result = MessageBox.Show("هل أنت متأكد من مسح هذا العميل؟", "تنبيه", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    con.Open();
                    string query = "DELETE FROM Customers WHERE CustomerName = '" + txtName.Text + "'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("تم حذف العميل بنجاح");
                    FillGrid();
                    btnClear_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في الحذف: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                txtName.Text = dgvCustomers.CurrentRow.Cells[1].Value.ToString();
                txtPhone.Text = dgvCustomers.CurrentRow.Cells[2].Value.ToString();
                txtPassport.Text = dgvCustomers.CurrentRow.Cells[3].Value.ToString();
                cbTripType.Text = dgvCustomers.CurrentRow.Cells[4].Value.ToString();
            }
        }

        // --- كود البحث المطور ---
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // لو الخانة فاضية أو فيها النص التوضيحي، اعرض كل البيانات
            if (string.IsNullOrWhiteSpace(txtSearch.Text) || txtSearch.Text == "ابحث هنا عن طريق الاسم...")
            {
                FillGrid();
                return;
            }

            try
            {
                if (con.State == ConnectionState.Open) con.Close();
                con.Open();
                // تأكد أن CustomerName هو اسم العمود الفعلي في جدول Customers في الـ SQL
                string query = "SELECT * FROM Customers WHERE CustomerName LIKE N'%" + txtSearch.Text + "%'";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvCustomers.DataSource = dt;

                // الحفاظ على العناوين بالعربي بعد البحث
                if (dgvCustomers.Columns.Count > 0)
                {
                    dgvCustomers.Columns[0].HeaderText = "كود العميل";
                    dgvCustomers.Columns[1].HeaderText = "اسم العميل";
                    dgvCustomers.Columns[2].HeaderText = "رقم الهاتف";
                    dgvCustomers.Columns[3].HeaderText = "رقم الباسبور";
                    dgvCustomers.Columns[4].HeaderText = "نوع الرحلة";
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في البحث: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "ابحث هنا عن طريق الاسم...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "ابحث هنا عن طريق الاسم...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void label2_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
    }
}