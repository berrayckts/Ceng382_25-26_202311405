namespace Ceng382_25_26_202311405.ViewModels;

public class RegisteredUserViewModel
{
    public string Name { get; init; } = string.Empty;

    public string Surname { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string? ProfilePhotoPath { get; init; }

    public string DisplayPhotoPath =>
        string.IsNullOrWhiteSpace(ProfilePhotoPath)
            ? "/images/users/default-avatar.svg"
            : ProfilePhotoPath!;
}
