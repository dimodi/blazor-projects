namespace UserSessions.Data
{
    public class Session
    {
        internal DateTime Expires { get; set; } = DateTime.Now.AddDays(1);

        internal bool Pesistent { get; set; }

        internal string Token { get; set; } = string.Empty;

        internal int UserId { get; set; }
    }
}
