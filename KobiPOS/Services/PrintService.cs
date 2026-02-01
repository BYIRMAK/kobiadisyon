using KobiPOS.Models;
using System.Text;
using System.IO;

namespace KobiPOS.Services
{
    public class PrintService
    {
        public static string GenerateReceiptText(Order order, List<OrderDetail> details, Table table, User user)
        {
            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine("         KOBİ BİLİŞİM");
            sb.AppendLine("   CAFE & RESTORAN SİSTEMİ");
            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine($"Telefon: 0552 165 04 35");
            sb.AppendLine($"Web: www.kobibilisim.com");
            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine($"Masa: {table.TableName}");
            sb.AppendLine($"Tarih: {order.OrderDate:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Garson: {user.FullName}");
            sb.AppendLine($"Fiş No: {order.ID}");
            sb.AppendLine();
            sb.AppendLine("───────────────────────────────────────");
            sb.AppendLine("ÜRÜN               ADET    FİYAT   TUTAR");
            sb.AppendLine("───────────────────────────────────────");

            var db = DatabaseService.Instance;
            foreach (var detail in details)
            {
                var product = db.GetAllProducts().FirstOrDefault(p => p.ID == detail.ProductID);
                if (product != null)
                {
                    var productName = product.ProductName.Length > 18
                        ? product.ProductName.Substring(0, 15) + "..."
                        : product.ProductName;
                    var subtotal = detail.Quantity * detail.UnitPrice;
                    sb.AppendLine($"{productName,-18} {detail.Quantity,4} {detail.UnitPrice,7:F2} {subtotal,7:F2}");

                    if (!string.IsNullOrEmpty(detail.Notes))
                    {
                        sb.AppendLine($"  Not: {detail.Notes}");
                    }
                }
            }

            sb.AppendLine("───────────────────────────────────────");
            sb.AppendLine($"Ara Toplam:              {order.TotalAmount,12:F2} ₺");

            if (order.DiscountAmount > 0)
            {
                sb.AppendLine($"İndirim:                 {order.DiscountAmount,12:F2} ₺");
                var finalTotal = order.TotalAmount - order.DiscountAmount;
                sb.AppendLine($"TOPLAM:                  {finalTotal,12:F2} ₺");
            }
            else
            {
                sb.AppendLine($"TOPLAM:                  {order.TotalAmount,12:F2} ₺");
            }

            var kdv = (order.TotalAmount - order.DiscountAmount) * 0.18m;
            sb.AppendLine($"(KDV %18:                {kdv,12:F2} ₺)");

            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine("     Bizi tercih ettiğiniz için");
            sb.AppendLine("         teşekkür ederiz!");
            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════");

            return sb.ToString();
        }

        public static void PrintReceipt(string receiptText)
        {
            // In a real application, this would send to a thermal printer
            // For now, we'll just save to a file or show in a dialog
            try
            {
                var filename = $"Receipt_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KobiPOS", "Receipts");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllText(Path.Combine(path, filename), receiptText, Encoding.UTF8);
            }
            catch
            {
                // Handle error silently or show message
            }
        }
    }
}
