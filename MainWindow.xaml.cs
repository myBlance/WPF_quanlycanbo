using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WPF_quanlycanbo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (AuthenticateUser(username, password))
            {
                // Điều hướng tới màn hình chính
                frmCanBo window1 = new frmCanBo();
                window1.Show();

                // Đóng cửa sổ đăng nhập
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (var db = new AppDbContext())
            {
                // Kiểm tra người dùng có tồn tại trong cơ sở dữ liệu không
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null) return false;

                // Kiểm tra xem mật khẩu có tồn tại không và có giá trị hợp lệ không
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return false;  // Mật khẩu của người dùng không hợp lệ (null hoặc rỗng)
                }

                // Mã hóa mật khẩu nhập vào và so sánh với hash lưu trong cơ sở dữ liệu
                string hashedPassword = HashPassword(password);
                return user.PasswordHash == hashedPassword;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
