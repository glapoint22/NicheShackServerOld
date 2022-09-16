using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class TempProductPricePoint
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [MaxLength(50)]
        public string TextBefore { get; set; }
        public int WholeNumber { get; set; }
        public int Decimal { get; set; }

        [MaxLength(50)]
        public string TextAfter { get; set; }
        public int Index { get; set; }
    }
}
