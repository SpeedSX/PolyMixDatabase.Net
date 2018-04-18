namespace PolyMix.DatabaseLink.Models
{
    public class CreateProcessItemRequest
    {
        public int OrderId { get; set; }

        public int Part { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public decimal Cost { get; set; }

        public int ProcessId { get; set; }

        public decimal? ItemProfit { get; set; }

        public bool IsItemInProfit { get; set; }

        public int? EquipCode { get; set; }

        public int? ProductIn { get; set; }

        public int? ProductOut { get; set; }

        public float? Multiplier { get; set; }

        public int? ContractorId { get; set; }

        public decimal? ContractorPercent { get; set; }

        public bool ContractorProcess { get; set; }

        public decimal OwnCost { get; set; }

        public decimal ContractorCost { get; set; }

        public decimal OwnPercent { get; set; }

        public decimal MatCost { get; set; }

        public decimal? MatPercent { get; set; }

        public int? EstimatedDuration { get; set; }

        public int? LinkedItemId { get; set; }

        public int? SideCount { get; set; }
    }
}
