namespace UserSessions.Data
{
    public class Session
    {
        public DateTime Expires { get; set; } = DateTime.Now.AddDays(1);

        public bool Pesistent { get; set; }

        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}
