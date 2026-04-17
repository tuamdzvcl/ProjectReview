using EventTick.Model.Models;

namespace projectDemo.Service.EmailService
{
    public static class EmailBodyBuilder
    {
        public static string BuildBookingSuccessBody(BookingEmailData data)
        {
            var ticketRows = BuildTicketListHtml(data.Tickets, isSuccess: true);

            return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
</head>
<body style=""margin:0; padding:0; background-color:#f0f2f5; font-family:'Segoe UI', Arial, sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f2f5; padding:30px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 4px 24px rgba(0,0,0,0.08);"">
                    
                    <!-- HEADER -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%); padding:32px 40px; text-align:center;"">
                            <h1 style=""margin:0; color:#ffffff; font-size:28px; font-weight:700; letter-spacing:0.5px;"">🎉 TickEvent</h1>
                            <p style=""margin:8px 0 0; color:rgba(255,255,255,0.9); font-size:16px; font-weight:400;"">Xác nhận đặt vé thành công</p>
                        </td>
                    </tr>

                    <!-- STATUS BADGE -->
                    <tr>
                        <td style=""padding:24px 40px 0; text-align:center;"">
                            <div style=""display:inline-block; background:#e8f5e9; color:#2e7d32; padding:8px 24px; border-radius:20px; font-size:14px; font-weight:600;"">
                                ✅ Thanh toán thành công
                            </div>
                        </td>
                    </tr>

                    <!-- GREETING -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <p style=""margin:0; font-size:16px; color:#333;"">
                                Xin chào <strong style=""color:#2e7d32;"">{data.FullName}</strong>,
                            </p>
                            <p style=""margin:8px 0 0; font-size:14px; color:#666; line-height:1.6;"">
                                Cảm ơn bạn đã đặt vé tại <strong>TickEvent</strong>. Đơn hàng của bạn đã được thanh toán thành công. 
                                Dưới đây là thông tin chi tiết:
                            </p>
                        </td>
                    </tr>

                    <!-- THÔNG TIN CÁ NHÂN -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <h3 style=""margin:0 0 12px; font-size:16px; color:#333; border-bottom:2px solid #4CAF50; padding-bottom:8px;"">
                                👤 Thông tin người đặt
                            </h3>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-size:14px; color:#555;"">
                                <tr>
                                    <td style=""padding:6px 0; width:140px; font-weight:600; color:#333;"">Họ và tên:</td>
                                    <td style=""padding:6px 0;"">{data.FullName}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Email:</td>
                                    <td style=""padding:6px 0;"">{data.Email}</td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- THÔNG TIN SỰ KIỆN -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <h3 style=""margin:0 0 12px; font-size:16px; color:#333; border-bottom:2px solid #4CAF50; padding-bottom:8px;"">
                                📅 Thông tin sự kiện
                            </h3>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-size:14px; color:#555;"">
                                <tr>
                                    <td style=""padding:6px 0; width:140px; font-weight:600; color:#333;"">Sự kiện:</td>
                                    <td style=""padding:6px 0; font-weight:600; color:#2e7d32;"">{data.EventName}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Địa điểm:</td>
                                    <td style=""padding:6px 0;"">{data.EventLocation}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Bắt đầu:</td>
                                    <td style=""padding:6px 0;"">{data.EventStartDate:dd/MM/yyyy HH:mm}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Kết thúc:</td>
                                    <td style=""padding:6px 0;"">{data.EventEndDate:dd/MM/yyyy HH:mm}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Mã đơn hàng:</td>
                                    <td style=""padding:6px 0;"">
                                        <span style=""background:#f5f5f5; padding:4px 12px; border-radius:4px; font-family:monospace; font-weight:700; color:#333;"">{data.OrderCode}</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- DANH SÁCH VÉ -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <h3 style=""margin:0 0 12px; font-size:16px; color:#333; border-bottom:2px solid #4CAF50; padding-bottom:8px;"">
                                🎟️ Chi tiết vé
                            </h3>
                            {ticketRows}
                        </td>
                    </tr>

                    <!-- TỔNG TIỀN -->
                    <tr>
                        <td style=""padding:16px 40px 0;"">
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f8fdf8; border-radius:8px; padding:12px 16px;"">
                                <tr>
                                    <td style=""padding:12px 16px; font-size:16px; font-weight:700; color:#333;"">Tổng thanh toán:</td>
                                    <td style=""padding:12px 16px; font-size:20px; font-weight:700; color:#2e7d32; text-align:right;"">{data.TotalAmount:N0} VNĐ</td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- LƯU Ý -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <div style=""background:#fff3e0; border-left:4px solid #ff9800; padding:12px 16px; border-radius:0 8px 8px 0;"">
                                <p style=""margin:0; font-size:13px; color:#e65100; line-height:1.6;"">
                                    ⚠️ <strong>Lưu ý quan trọng:</strong><br/>
                                    • Vui lòng mang theo mã QR khi đến check-in sự kiện.<br/>
                                    • Mỗi mã QR chỉ sử dụng được 1 lần duy nhất.<br/>
                                    • Vé không được hoàn trả sau khi thanh toán.
                                </p>
                            </div>
                        </td>
                    </tr>

                    <!-- CTA BUTTON -->
                    <tr>
                        <td style=""padding:28px 40px; text-align:center;"">
                            <a href=""http://localhost:4200/my-ticket"" 
                               style=""display:inline-block; background:linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%); color:#ffffff; padding:14px 36px; text-decoration:none; border-radius:8px; font-size:15px; font-weight:600; letter-spacing:0.3px;"">
                                🎫 Xem vé của tôi
                            </a>
                        </td>
                    </tr>

                    <!-- FOOTER -->
                    <tr>
                        <td style=""background:#f9f9f9; padding:20px 40px; text-align:center; border-top:1px solid #eee;"">
                            <p style=""margin:0; font-size:12px; color:#999; line-height:1.6;"">
                                © 2026 TickEvent - Hệ thống quản lý sự kiện<br/>
                                Email này được gửi tự động, vui lòng không trả lời.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        public static string BuildBookingFailedBody(BookingEmailData data)
        {
            var ticketRows = BuildTicketListHtml(data.Tickets, isSuccess: false);

            return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
</head>
<body style=""margin:0; padding:0; background-color:#f0f2f5; font-family:'Segoe UI', Arial, sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f2f5; padding:30px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 4px 24px rgba(0,0,0,0.08);"">
                    
                    <!-- HEADER -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #f44336 0%, #c62828 100%); padding:32px 40px; text-align:center;"">
                            <h1 style=""margin:0; color:#ffffff; font-size:28px; font-weight:700; letter-spacing:0.5px;"">⚠️ TickEvent</h1>
                            <p style=""margin:8px 0 0; color:rgba(255,255,255,0.9); font-size:16px; font-weight:400;"">Thông báo đặt vé không thành công</p>
                        </td>
                    </tr>

                    <!-- STATUS BADGE -->
                    <tr>
                        <td style=""padding:24px 40px 0; text-align:center;"">
                            <div style=""display:inline-block; background:#ffebee; color:#c62828; padding:8px 24px; border-radius:20px; font-size:14px; font-weight:600;"">
                                ❌ Thanh toán thất bại
                            </div>
                        </td>
                    </tr>

                    <!-- GREETING -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <p style=""margin:0; font-size:16px; color:#333;"">
                                Xin chào <strong style=""color:#c62828;"">{data.FullName}</strong>,
                            </p>
                            <p style=""margin:8px 0 0; font-size:14px; color:#666; line-height:1.6;"">
                                Rất tiếc, đơn đặt vé của bạn tại <strong>TickEvent</strong> đã không thành công. 
                                Thanh toán đã bị hủy hoặc gặp lỗi trong quá trình xử lý.
                            </p>
                        </td>
                    </tr>

                    <!-- THÔNG TIN CÁ NHÂN -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <h3 style=""margin:0 0 12px; font-size:16px; color:#333; border-bottom:2px solid #f44336; padding-bottom:8px;"">
                                👤 Thông tin người đặt
                            </h3>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-size:14px; color:#555;"">
                                <tr>
                                    <td style=""padding:6px 0; width:140px; font-weight:600; color:#333;"">Họ và tên:</td>
                                    <td style=""padding:6px 0;"">{data.FullName}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Email:</td>
                                    <td style=""padding:6px 0;"">{data.Email}</td>
                                </tr>
                                
                            </table>
                        </td>
                    </tr>

                    <!-- THÔNG TIN ĐƠN HÀNG -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <h3 style=""margin:0 0 12px; font-size:16px; color:#333; border-bottom:2px solid #f44336; padding-bottom:8px;"">
                                📋 Thông tin đơn hàng bị hủy
                            </h3>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-size:14px; color:#555;"">
                                <tr>
                                    <td style=""padding:6px 0; width:140px; font-weight:600; color:#333;"">Sự kiện:</td>
                                    <td style=""padding:6px 0;"">{data.EventName}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Mã đơn hàng:</td>
                                    <td style=""padding:6px 0;"">
                                        <span style=""background:#f5f5f5; padding:4px 12px; border-radius:4px; font-family:monospace; font-weight:700; color:#333;"">{data.OrderCode}</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Địa điểm:</td>
                                    <td style=""padding:6px 0;"">{data.EventLocation}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:6px 0; font-weight:600; color:#333;"">Thời gian SK:</td>
                                    <td style=""padding:6px 0;"">{data.EventStartDate:dd/MM/yyyy HH:mm}</td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- DANH SÁCH VÉ ĐÃ CHỌN -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <h3 style=""margin:0 0 12px; font-size:16px; color:#333; border-bottom:2px solid #f44336; padding-bottom:8px;"">
                                🎟️ Vé đã chọn (chưa được xác nhận)
                            </h3>
                            {ticketRows}
                        </td>
                    </tr>

                    <!-- TỔNG TIỀN -->
                    <tr>
                        <td style=""padding:16px 40px 0;"">
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#fce4ec; border-radius:8px;"">
                                <tr>
                                    <td style=""padding:12px 16px; font-size:16px; font-weight:700; color:#333;"">Số tiền chưa thanh toán:</td>
                                    <td style=""padding:12px 16px; font-size:20px; font-weight:700; color:#c62828; text-align:right; text-decoration:line-through;"">{data.TotalAmount:N0} VNĐ</td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- HƯỚNG DẪN -->
                    <tr>
                        <td style=""padding:24px 40px 0;"">
                            <div style=""background:#e3f2fd; border-left:4px solid #1976d2; padding:12px 16px; border-radius:0 8px 8px 0;"">
                                <p style=""margin:0; font-size:13px; color:#0d47a1; line-height:1.6;"">
                                    💡 <strong>Bạn có thể thử lại:</strong><br/>
                                    • Kiểm tra số dư tài khoản/ví MoMo của bạn.<br/>
                                    • Đảm bảo kết nối internet ổn định.<br/>
                                    • Thử đặt lại vé từ trang sự kiện.<br/>
                                    • Liên hệ hỗ trợ nếu vẫn gặp lỗi.
                                </p>
                            </div>
                        </td>
                    </tr>

                    <!-- CTA BUTTON -->
                    <tr>
                        <td style=""padding:28px 40px; text-align:center;"">
                            <a href=""http://localhost:4200"" 
                               style=""display:inline-block; background:linear-gradient(135deg, #1976d2 0%, #0d47a1 100%); color:#ffffff; padding:14px 36px; text-decoration:none; border-radius:8px; font-size:15px; font-weight:600; letter-spacing:0.3px;"">
                                🔄 Thử đặt vé lại
                            </a>
                        </td>
                    </tr>

                    <!-- FOOTER -->
                    <tr>
                        <td style=""background:#f9f9f9; padding:20px 40px; text-align:center; border-top:1px solid #eee;"">
                            <p style=""margin:0; font-size:12px; color:#999; line-height:1.6;"">
                                © 2026 TickEvent - Hệ thống quản lý sự kiện<br/>
                                Email này được gửi tự động, vui lòng không trả lời.<br/>
                                Nếu cần hỗ trợ, vui lòng liên hệ: <a href=""mailto:support@tickevent.com"" style=""color:#1976d2;"">support@tickevent.com</a>
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
        private static string BuildTicketListHtml(List<TicketEmailItem> tickets, bool isSuccess)
        {
            if (tickets == null || tickets.Count == 0)
                return "<p style=\"font-size:14px; color:#999;\">Không có thông tin vé.</p>";

            var sb = new System.Text.StringBuilder();

            foreach (var ticket in tickets)
            {
                var borderColor = isSuccess ? "#4CAF50" : "#f44336";
                var badgeBg = isSuccess ? "#e8f5e9" : "#ffebee";
                var badgeColor = isSuccess ? "#2e7d32" : "#c62828";
                var badgeText = isSuccess ? "✅ Đã xác nhận" : "❌ Chưa xác nhận";

                sb.Append($@"
                <div style=""border:1px solid #e0e0e0; border-left:4px solid {borderColor}; border-radius:8px; padding:16px; margin-bottom:12px; background:#fafafa;"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                            <td style=""vertical-align:top; width:65%;"">
                                <p style=""margin:0 0 4px; font-size:15px; font-weight:700; color:#333;"">
                                    🎫 {ticket.TicketTypeName}
                                </p>
                                <p style=""margin:0 0 2px; font-size:13px; color:#666;"">
                                    Mã vé: <strong style=""font-family:monospace;"">tesst</strong>
                                </p>
                                <p style=""margin:0 0 2px; font-size:13px; color:#666;"">
                                    Giá: <strong>{ticket.Price:N0} VNĐ</strong>
                                </p>
                                <span style=""display:inline-block; background:{badgeBg}; color:{badgeColor}; padding:2px 10px; border-radius:10px; font-size:11px; font-weight:600; margin-top:4px;"">
                                    {badgeText}
                                </span>
                            </td>");

                if (isSuccess && !string.IsNullOrWhiteSpace(ticket.QRCodeUrl))
                {
                    sb.Append($@"
                            <td style=""vertical-align:middle; text-align:right; width:35%;"">
                                <div style=""background:#fff; padding:8px; border:1px solid #e0e0e0; border-radius:8px; display:inline-block;"">
                                    <img src=""{ticket.QRCodeUrl}"" alt=""QR Code"" width=""120"" height=""120"" style=""display:block; border-radius:4px;"" />
                                    <p style=""margin:4px 0 0; font-size:10px; color:#999; text-align:center;"">Scan để check-in</p>
                                </div>
                            </td>");
                }

                sb.Append(@"
                        </tr>
                    </table>
                </div>");
            }

            return sb.ToString();
        }
    }

}
