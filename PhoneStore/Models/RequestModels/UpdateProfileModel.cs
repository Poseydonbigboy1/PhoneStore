namespace PhoneStore.Models.RequestModels;

public class UpdateProfileModel
{
    public string? Name { get; set; }
    public string? Login { get; set; }
}

public class ChangePasswordModel
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
