using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoonLight_Agency
{
    public partial class TripForm : Form
    {
        string connString = @"Server=.\SQLEXPRESS;Database=MoonLight_DB;Trusted_Connection=True;";
        int selectedTripID = 0;

        public TripForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(TripForm_Load);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);

            // ربط حدث تغيير النص في مربع البحث للبحث التلقائي (اختياري لكن احترافي)
            if (txtSearch != null)
            {
                txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged);
            }
        }

        public void LoadTripData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT * FROM Trips";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;

                    if (dataGridView1.Columns.Contains("TripID"))
                    {
                        dataGridView1.Columns["TripID"].Visible = false;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading data: " + ex.Message); }
        }

        private void TripForm_Load(object sender, EventArgs e) { LoadTripData(); }

        // --- ميثود البحث الجديدة ---
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // البحث باستخدام TripName والربط مع txtSearch
                    string query = "SELECT * FROM Trips WHERE TripName LIKE @name";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@name", "%" + txtSearch.Text + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;

                    if (dataGridView1.Columns.Contains("TripID"))
                    {
                        dataGridView1.Columns["TripID"].Visible = false;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Search Error: " + ex.Message); }
        }

        // كود إضافي: يرجع البيانات لو المستخدم مسح خانة البحث
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadTripData();
            }
            else
            {
                btnSearch_Click(null, null); // ينفذ البحث أوتوماتيك أثناء الكتابة
            }
        }

        // زرار الحفظ
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "INSERT INTO Trips (TripName, Price, TripDate, TripType) VALUES (@name, @price, @date, @type)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txt_TripName.Text);
                    cmd.Parameters.AddWithValue("@price", txt_Price.Text);
                    cmd.Parameters.AddWithValue("@type", cb_TripType.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.TryParse(txt_TripDate.Text, out var d) ? d : DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم الحفظ بنجاح!");
                    LoadTripData();
                    btnClear_Click(null, null);
                }
            }
            catch (Exception ex) { MessageBox.Show("Save Error: " + ex.Message); }
        }

        // زرار التعديل
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTripID == 0) { MessageBox.Show("اختار الرحلة من الجدول الأول!"); return; }
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "UPDATE Trips SET TripName=@name, Price=@price, TripDate=@date, TripType=@type WHERE TripID=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedTripID);
                    cmd.Parameters.AddWithValue("@name", txt_TripName.Text);
                    cmd.Parameters.AddWithValue("@price", txt_Price.Text);
                    cmd.Parameters.AddWithValue("@type", cb_TripType.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.Parse(txt_TripDate.Text));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم التعديل بنجاح!");
                    LoadTripData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Update Error: " + ex.Message); }
        }

        // زرار الحذف
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTripID == 0) return;
            if (MessageBox.Show("هل تريد الحذف؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = "DELETE FROM Trips WHERE TripID=@id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedTripID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم الحذف!");
                        LoadTripData();
                        btnClear_Click(null, null);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Delete Error: " + ex.Message); }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedTripID = Convert.ToInt32(row.Cells["TripID"].Value);

                txt_TripName.Text = row.Cells["TripName"].Value.ToString();
                txt_Price.Text = row.Cells["Price"].Value.ToString();
                txt_TripDate.Text = row.Cells["TripDate"].Value.ToString();
                cb_TripType.Text = row.Cells["TripType"].Value.ToString();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txt_TripName.Clear(); txt_Price.Clear(); txt_TripDate.Clear();
            if (txtSearch != null) txtSearch.Clear(); // يمسح خانة البحث أيضاً
            cb_TripType.SelectedIndex = -1; selectedTripID = 0;
        }
    }
}