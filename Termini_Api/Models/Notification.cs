namespace Termini_Api.Models
{
    public class Notification
    {

        public long id { get; set; }
        public DateTime created_at { get; set; }
        public bool isRead { get; set; }
        public string message { get; set; }
        public int clientId { get; set; }

    }
}

