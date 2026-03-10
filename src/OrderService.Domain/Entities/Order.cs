using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        // Neden Guid? Tahmin edilebilir (1, 2, 3 gibi) ID'ler güvenlik açığıdır. 
        // Guid benzersizdir ve tahmin edilemez.
        public Guid Id
            { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        // decimal kullanıyoruz çünkü para birimlerinde float/double hassasiyet kaybına neden olur.
        public decimal TotalAmount { get; set; } 
        public DateTime CreatedDate { get; set; }

        // Status (Siparişin durumu): Başlangıçta basit bir string, 
        // ileride Enum (Pending, Shipped, Cancelled) yapacağız.
        public string Status { get; set; } = "Pending";

        // Soft Delete: Veriyi gerçekten silmek yerine "silindi" işaretlemek profesyonel standarttır.
        public bool IsDeleted { get; set; }
    }
}
