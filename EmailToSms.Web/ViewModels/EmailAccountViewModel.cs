namespace EmailToSms.Web.ViewModels
{
    public class EmailAccountViewModel
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime LastChecked { get; set; }
    }
}
