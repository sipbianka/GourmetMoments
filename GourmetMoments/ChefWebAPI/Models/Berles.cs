using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChefWebAPI.Models
{
    public class Berles
    {
        [Key]
        public int Id { get; set; }

        public int Uid { get; set; }

        public int ChefId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DailyRate { get; set; }

        public int BaseFee { get; set; }

        [NotMapped]
        public int TotalPrice => BaseFee + (DailyRate * DaysInclusive);

        [NotMapped]
        public int DaysInclusive => (EndDate.Date - StartDate.Date).Days + 1;
    }
}