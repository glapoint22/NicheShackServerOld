using System.Collections.Generic;

namespace Manager.Classes
{
    public struct UpdatedPriceIndices
    {
        public int ProductContentId { get; set; }
        public bool[] PriceIndices { get; set; }
    }
}
