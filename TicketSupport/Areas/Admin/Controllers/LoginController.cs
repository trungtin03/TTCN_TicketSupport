using System;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Linq;
using TicketSupport.Models;
using System.Drawing.Imaging;
using System.IO;

namespace TicketSupport.Areas.Admin.Controllers
{
    
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        // Action để tạo CAPTCHA
        public ActionResult GenerateCaptcha()
        {
            string captchaCode = GenerateRandomCode(6); // Số ký tự của mã CAPTCHA
            Session["Captcha"] = captchaCode; // Lưu mã CAPTCHA vào Session

            using (var bitmap = new Bitmap(200, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Tạo màu nền và màu văn bản
                graphics.Clear(Color.White);
                using (var font = new Font("Arial", 20, FontStyle.Bold))
                {
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        graphics.DrawString(captchaCode, font, brush, new PointF(10, 10));
                    }
                }

                // Thêm các đường nét ngẫu nhiên để làm khó nhận diện
                var pen = new Pen(Color.Gray, 2);
                for (int i = 0; i < 5; i++)
                {
                    graphics.DrawLine(pen, new PointF(i * 40, 0), new PointF(i * 40, 50));
                }

                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    return File(memoryStream.ToArray(), "image/png");
                }
            }
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public ActionResult Login(string email, string password, string captcha)
        {
            // Kiểm tra mã CAPTCHA
            if (Session["Captcha"] == null || Session["Captcha"].ToString() != captcha)
            {
                ViewBag.Message = "CAPTCHA không hợp lệ. Vui lòng thử lại.";
                return View();
            }

            using (var db = new Tech_Support_TicketEntities())
            {
                var user = db.tblnguoidungs.FirstOrDefault(u => u.email == email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    ViewBag.Message = "Invalid username or password.";
                    return View();
                }

                // Lưu thông tin đăng nhập vào Session
                Session["UserId"] = user.ma_nguoi_dung;
                Session["Username"] = user.email;

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
