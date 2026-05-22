namespace ChefWebAPI.Dtos
{
    public class BerlesResponseDto
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int ChefId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DailyRate { get; set; }
        public int BaseFee { get; set; }
        public int TotalPrice { get; set; }
    }
}
