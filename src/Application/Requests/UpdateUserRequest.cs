using Domain.Enums;
using FluentValidation;

namespace Application.Requests;

public class UpdateUserRequest
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; } = null!;
    public required string Password { get; set; }
    public string Email { get; set; } = null!;
    public UserRoles Role { get; set; }
    public int CompanyId { get; set; }
    public int? LogoAttachmentId { get; set; }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id).NotNull()
            .GreaterThan(0).WithMessage("Id must be positive");

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

        RuleFor(x => x.CompanyId).NotNull()
            .GreaterThan(0).WithMessage("CompanyId must be positive")
            .LessThan(int.MaxValue).WithMessage("CompanyId is too long");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(ValidationConstants.MinimumPasswordLength)
            .WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}