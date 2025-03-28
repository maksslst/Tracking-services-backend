using FluentValidation;

namespace Application.Requests;

public class CreateCompanyRequest
{
    public string CompanyName { get; set; } = null!;
}

public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
    }
}