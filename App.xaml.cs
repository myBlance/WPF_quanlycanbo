using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using WPF_quanlycanbo.Model;


namespace WPF_quanlycanbo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SeedData(); 
        }

        private void SeedData()
        {
            using (var context = new AppDbContext())
            {
                var canBo = new CanBo
                {
                    SoHieuCAND = "123456",
                    HoVaTen = "Nguyen Van A",
                    GioiTinh = "Nam",
                    NgaySinh = DateTime.Parse("1990-01-01"),
                    DienThoai = "0123456789",
                    CCCD = "9876543210",
                    DiaChi = "Hà Nội",
                    DanToc = "Kinh",
                    DonVi = "Phòng An Ninh",
                    ChucVu = "Nhân viên",
                    ChucDanh = "Cử nhân",
                    CapBacHam = "Cấp 1",
                    TrinhDo = "Đại học",
                    TonGiao = "Không",
                    HinhAnh = null
                };

                context.CanBos.Add(canBo);
                context.SaveChanges();
            }

            using (var db = new AppDbContext())
            {
                if (!db.Users.Any(u => u.Username == "admin"))
                {
                    db.Users.Add(new User
                    {
                        Username = "admin",
                        PasswordHash = HashPassword("admin123"),
                        FullName = "Quản trị viên",
                        Role = "Admin",
                        DateCreated = DateTime.Now
                    });
                    db.SaveChanges();
                }
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
