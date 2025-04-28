using FluentValidation;

namespace Application.Requests;

public class RegistrationRequest
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Patronymic { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmation { get; set; }
}

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty()
            .MaximumLength(ValidationConstants.UserValidLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        
        RuleFor(x => x.FirstName).NotEmpty()
            .MaximumLength(ValidationConstants.UserValidLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        
        RuleFor(x => x.LastName).NotEmpty()
            .MaximumLength(ValidationConstants.UserValidLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        
        RuleFor(x => x.Patronymic)
            .MaximumLength(ValidationConstants.UserValidLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(ValidationConstants.UserValidLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        
        RuleFor(x => x.Password).NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        
        RuleFor(x => x.PasswordConfirmation).NotEmpty()
            .WithMessage("The password confirmation field is required")
            .Equal(x => x.Password)
            .WithMessage("Password and Password Confirmation don't match");
    }
}