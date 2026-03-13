using System.ComponentModel.DataAnnotations;
using Ceng382_25_26_202311405.Data;
using Ceng382_25_26_202311405.Models;
using Ceng382_25_26_202311405.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311405.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private static readonly string[] AllowedPhotoExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private const long MaxPhotoSizeInBytes = 2 * 1024 * 1024;

    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        ApplicationDbContext dbContext,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _dbContext = dbContext;
        _environment = environment;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IList<RegisteredUserViewModel> RegisteredUsers { get; private set; } = [];

    [TempData]
    public string? StatusMessage { get; set; }

    public string? ReturnUrl { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        await LoadRegisteredUsersAsync();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        await LoadRegisteredUsersAsync();

        if (Input.ProfilePhoto is not null)
        {
            ValidatePhoto(Input.ProfilePhoto);
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            Name = Input.Name.Trim(),
            Surname = Input.Surname.Trim(),
            Address = Input.Address.Trim()
        };

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        string? savedPhotoPath = null;

        try
        {
            if (Input.ProfilePhoto is not null)
            {
                savedPhotoPath = await SaveProfilePhotoAsync(Input.ProfilePhoto);
                user.ProfilePhotoPath = savedPhotoPath;
            }

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("A new user registered with email {Email}.", Input.Email);
                StatusMessage = "User registered successfully and saved to the database.";

                if (_signInManager.IsSignedIn(User))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser is not null)
                    {
                        await _signInManager.RefreshSignInAsync(currentUser);
                    }
                }

                return RedirectToPage();
            }

            DeletePhotoIfExists(savedPhotoPath);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        catch
        {
            DeletePhotoIfExists(savedPhotoPath);
            throw;
        }

        return Page();
    }

    private async Task LoadRegisteredUsersAsync()
    {
        RegisteredUsers = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Surname)
            .Select(user => new RegisteredUserViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email ?? string.Empty,
                Address = user.Address,
                ProfilePhotoPath = user.ProfilePhotoPath
            })
            .ToListAsync();
    }

    private void ValidatePhoto(IFormFile photo)
    {
        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

        if (!AllowedPhotoExtensions.Contains(extension))
        {
            ModelState.AddModelError("Input.ProfilePhoto", "Please upload a JPG, PNG, GIF, or WEBP image.");
        }

        if (photo.Length > MaxPhotoSizeInBytes)
        {
            ModelState.AddModelError("Input.ProfilePhoto", "Profile photo size must be 2 MB or less.");
        }
    }

    private async Task<string> SaveProfilePhotoAsync(IFormFile photo)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "users");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(uploadsFolder, uniqueFileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await photo.CopyToAsync(stream);

        return $"/images/users/{uniqueFileName}";
    }

    private void DeletePhotoIfExists(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var normalizedPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_environment.WebRootPath, normalizedPath);

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<ApplicationUser>)_userStore;
    }

    public sealed class InputModel
    {
        [Required]
        [StringLength(40)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        [Display(Name = "Surname")]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Profile photo")]
        public IFormFile? ProfilePhoto { get; set; }
    }
}
