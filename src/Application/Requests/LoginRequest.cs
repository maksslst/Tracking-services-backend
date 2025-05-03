using FluentValidation;

namespace Application.Requests;

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty()
            .MaximumLength(ValidationConstants.UserValidLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}