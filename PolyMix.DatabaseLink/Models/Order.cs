using System;

namespace PolyMix.DatabaseLink.Models
{
    // Not used!
    public class Order
    {
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public int Quantity { get; set; }

        public string Name { get; set; }

        public DateTime FinishDate { get; set; }

        public Customer Customer { get; set; }
    }
}
