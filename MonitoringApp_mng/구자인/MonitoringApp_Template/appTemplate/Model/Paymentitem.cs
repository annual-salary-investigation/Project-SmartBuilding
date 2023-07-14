using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartApp.Models
{
    public class PaymentItem
    {
        public int ProductId { get; set; }
        public string Id { get; set; }
        public string Product { get; set; }
        public int Price { get; set; }
        public int Count { set; get; }
        public string Category { get; set; }
        public string Image { get; set; }
        public DateTime DateTime { get; set; }
    }
}
