namespace FinalProjectAPI.Models
{
    public class Book
    {
        public int Id { get; set; }                 // PK
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }               // Publication year
        public bool Archived { get; set; }          // Used by validation
        public DateTime? LastValidated { get; set; }// Governance: audit trail
    }
}
