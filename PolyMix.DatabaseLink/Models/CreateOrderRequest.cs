using System;

namespace PolyMix.DatabaseLink.Models
{
    public class CreateOrderRequest
    {
        public string Name { get; set; }

        public bool IsDraft { get; set; }

        public int Quantity { get; set; }

        public DateTime FinishDate { get; set; }

        public string CustomerName { get; set; }

        public bool AutoCreateCustomer { get; set; }

        public int KindId { get; set; }

        public int CustomerId { get; set; }

        public decimal Rate { get; set; }

        public int RowColor { get; set; }

        public int OrderState { get; set; }

        public int PayState { get; set; }

        public decimal TotalCost { get; set; }

        public decimal TotalCostNative { get; set; }

        public decimal CustomerTotalCost { get; set; }

        public decimal OwnCost { get; set; }

        public decimal ContractorCost { get; set; }

        public decimal MatCost { get; set; }
    }
}
