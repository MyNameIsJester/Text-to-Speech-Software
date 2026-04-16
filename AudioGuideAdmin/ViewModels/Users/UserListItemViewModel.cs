namespace AudioGuideAdmin.ViewModels.Users
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string? FullName { get; set; }
        public string RoleName { get; set; } = "";
    }
}