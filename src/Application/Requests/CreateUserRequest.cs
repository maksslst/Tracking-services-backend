using FluentValidation;

namespace Application.Requests;

public class CreateUserRequest
{
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int CompanyId { get; set; }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
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
        RuleFor(x => x.CompanyId).NotNull()
            .GreaterThan(0).WithMessage("CompanyId must be positive")
            .LessThan(int.MaxValue).WithMessage("CompanyId is too long");
    }
}