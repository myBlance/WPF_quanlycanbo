using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WPF_quanlycanbo.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPF_quanlycanbo
{
    public partial class frmCanBo : Window
    {
        public ObservableCollection<CanBo> DanhSachCanBo { get; set; } = new ObservableCollection<CanBo>();

        public frmCanBo()
        {
            InitializeComponent();
            dgDanhSach.ItemsSource = DanhSachCanBo; 

            LoadDataFromDatabase();

            InitializeComboBoxes();
        }

        // Initialize ComboBoxes with predefined lists
        private void InitializeComboBoxes()
        {
            // Ethnicities
            List<string> danTocList = new List<string> { "Kinh", "Tày", "Thái", "Mường", "H'mong" };
            txtDanToc.ItemsSource = danTocList;

            // Units
            List<string> donViList = new List<string> { "Phòng Hành Chính", "Phòng Kỹ Thuật", "Phòng An Ninh" };
            cbDonVi.ItemsSource = donViList;

            // Positions
            List<string> chucVuList = new List<string> { "Giám đốc", "Phó giám đốc", "Trưởng phòng", "Nhân viên" };
            cbChucVu.ItemsSource = chucVuList;

            // Titles
            List<string> chucDanhList = new List<string> { "Thạc sĩ", "Cử nhân", "Kỹ sư" };
            cbChucDanh.ItemsSource = chucDanhList;

            // Ranks
            List<string> capBacHamList = new List<string> { "Cấp 1", "Cấp 2", "Cấp 3" };
            txtCapBacHam.ItemsSource = capBacHamList;

            // Education Levels
            List<string> trinhDoList = new List<string> { "Đại học", "Cao đẳng", "Trung cấp" };
            cbTrinhDo.ItemsSource = trinhDoList;

            // Religions
            List<string> tonGiaoList = new List<string> { "Không", "Phật giáo", "Công giáo", "Hòa Hảo" };
            cbTonGiao.ItemsSource = tonGiaoList;
        }

        // Load data from the database
        private void LoadDataFromDatabase()
        {
            using (var db = new AppDbContext())
            {
                var canBoList = db.CanBos
                    .Where(cb => cb.NgaySinh != null) 
                    .ToList();

                foreach (var canBo in canBoList)
                {
                    // Kiểm tra và xử lý các trường nullable
                    canBo.NgaySinh = canBo.NgaySinh ?? DateTime.Now;
                    canBo.CCCD = canBo.CCCD ?? string.Empty;
                    canBo.DiaChi = canBo.DiaChi ?? string.Empty;
                    canBo.DonVi = canBo.DonVi ?? string.Empty;

                    // Thêm cán bộ vào danh sách
                    DanhSachCanBo.Add(canBo);

                    // Ngày sinh đã được gán giá trị mặc định, không còn nullable nữa
                    if (canBo.NgaySinh == DateTime.Now)
                    {
                        Console.WriteLine("Ngày sinh không được cung cấp.");
                    }
                    else
                    {
                        DateTime ngaySinhValue = canBo.NgaySinh.Value;
                        Console.WriteLine($"Ngày sinh: {ngaySinhValue.ToShortDateString()}");
                    }
                }
            }
        }

        // Add new personnel
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text) || string.IsNullOrWhiteSpace(txtSoHieuCAND.Text))
            {
                MessageBox.Show("Họ và tên, Số hiệu CAND không được để trống.");
                return;
            }

            var canBo = new CanBo
            {
                SoHieuCAND = txtSoHieuCAND.Text,
                HoVaTen = txtHoVaTen.Text,
                GioiTinh = chkNam.IsChecked == true ? "Nam" : "Nữ",
                NgaySinh = dpNgaySinh.SelectedDate ?? DateTime.Now,
                DienThoai = txtDienThoai.Text,
                CCCD = txtCCCD.Text,
                DiaChi = txtDiaChi.Text,
                DonVi = cbDonVi.SelectedItem?.ToString() ?? string.Empty,
            };

            // Handle image upload
            if (imgProfile.Source != null)
            {
                using (var ms = new MemoryStream())
                {
                    var encoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(imgProfile.Source as System.Windows.Media.Imaging.BitmapSource));
                    encoder.Save(ms);
                    canBo.HinhAnh = ms.ToArray();
                }
            }
            else
            {
                canBo.HinhAnh = null; 
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    await db.CanBos.AddAsync(canBo);  // Add personnel to the database
                    await db.SaveChangesAsync(); // Save changes asynchronously
                }
                DanhSachCanBo.Add(canBo); // Add to the ObservableCollection
                ClearForm(); // Clear input form
                MessageBox.Show("Cán bộ đã được thêm thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm cán bộ: {ex.Message}");
            }
        }

        // Save personnel data (currently a placeholder)
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dữ liệu đã được lưu!");
        }

        // Edit personnel information
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dgDanhSach.SelectedItem is CanBo canBo)
            {
                txtSoHieuCAND.Text = canBo.SoHieuCAND;
                txtHoVaTen.Text = canBo.HoVaTen;
                chkNam.IsChecked = canBo.GioiTinh == "Nam";
                chkNu.IsChecked = canBo.GioiTinh == "Nữ";
                dpNgaySinh.SelectedDate = canBo.NgaySinh;
                txtDienThoai.Text = canBo.DienThoai;
                txtCCCD.Text = canBo.CCCD;
                txtDiaChi.Text = canBo.DiaChi;
                cbDonVi.SelectedItem = canBo.DonVi;

                // Set image if available
                if (canBo.HinhAnh != null)
                {
                    var image = System.Windows.Media.Imaging.BitmapFrame.Create(new MemoryStream(canBo.HinhAnh));
                    imgProfile.Source = image;
                }
            }
        }

        // Delete personnel information
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgDanhSach.SelectedItem is CanBo canBo)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        db.CanBos.Remove(canBo); // Remove personnel from the database
                        await db.SaveChangesAsync(); // Save changes asynchronously
                    }
                    DanhSachCanBo.Remove(canBo); // Remove from ObservableCollection
                    MessageBox.Show("Cán bộ đã bị xóa!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa cán bộ: {ex.Message}");
                }
            }
        }

        // Select image for profile
        private void ChonHinh_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName));
                imgProfile.Source = bitmap;
            }
        }

        // Clear input form
        private void ClearForm()
        {
            txtSoHieuCAND.Clear();
            txtHoVaTen.Clear();
            chkNam.IsChecked = false;
            chkNu.IsChecked = false;
            dpNgaySinh.SelectedDate = null;
            txtDienThoai.Clear();
            txtCCCD.Clear();
            txtDiaChi.Clear();
            cbDonVi.SelectedIndex = -1;
            imgProfile.Source = null;
        }

        // Search personnel
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var keyword = txtSearch.Text.ToLower();
            using (var db = new AppDbContext())
            {
                var filteredList = db.CanBos
                                     .Where(cb => cb.HoVaTen.ToLower().Contains(keyword) || cb.SoHieuCAND.ToLower().Contains(keyword))
                                     .ToList();

                DanhSachCanBo.Clear(); // Clear current list
                foreach (var canBo in filteredList)
                {
                    DanhSachCanBo.Add(canBo); // Add to new list
                }
            }
        }
    }
}
