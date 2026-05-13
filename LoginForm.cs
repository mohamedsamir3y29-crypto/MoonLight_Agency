using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // المكتبة الأساسية للتعامل مع قاعدة البيانات
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoonLight_Agency
{
    public partial class LoginForm : Form
    {
        // جملة الاتصال بقاعدة البيانات - تأكد إن اسم القاعدة MoonLight_DB
        string connString = @"Server=.\SQLEXPRESS;Database=MoonLight_DB;Trusted_Connection=True;";

        public LoginForm()
        {
            InitializeComponent();
            // لجعل الشاشة تظهر في المنتصف عند التشغيل
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // التحقق من أن الخانات ليست فارغة (Clean UI Policy)
            if (string.IsNullOrWhiteSpace(txt_User.Text) || string.IsNullOrWhiteSpace(txt_Pass.Text))
            {
                MessageBox.Show("من فضلك أدخل اسم المستخدم وكلمة المرور");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // استعلام للتحقق من وجود المستخدم في الجدول الذي أنشأناه
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @user AND Password = @pass";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    // استخدام Parameters لمنع الـ SQL Injection (أمان عالي)
                    cmd.Parameters.AddWithValue("@user", txt_User.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txt_Pass.Text.Trim());

                    conn.Open();
                    int userExists = (int)cmd.ExecuteScalar();

                    if (userExists > 0)
                    {
                        MessageBox.Show("تم تسجيل الدخول بنجاح");

                        // فتح الفورم الرئيسية (تأكد أن اسمها MainForm في مشروعك)
                        MainForm main = new MainForm();
                        main.Show();

                        // إخفاء شاشة الدخول
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("خطأ في اسم المستخدم أو كلمة المرور!");
                        txt_Pass.Clear(); // مسح كلمة المرور لتسهيل إعادة المحاولة
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("عفواً، حدث خطأ في الاتصال بقاعدة البيانات: " + ex.Message);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
