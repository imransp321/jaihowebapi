namespace JaiHoWebApi.ViewModels
{
    public partial class ChangePassword
    {
        public string email { get; set; }
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
    }
}
