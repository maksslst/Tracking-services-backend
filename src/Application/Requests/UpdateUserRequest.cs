using FluentValidation;

namespace Application.Requests;

public class UpdateUserRequest
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int CompanyId { get; set; }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id).NotNull()
            .GreaterThan(0).WithMessage("Id must be positive");
        RuleFor(x => x.Username).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.FirstName).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.LastName ).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.Patronymic)
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.CompanyId).NotNull()
            .GreaterThan(0).WithMessage("CompanyId must be positive")
            .LessThan(int.MaxValue).WithMessage("CompanyId is too long");
    }
}