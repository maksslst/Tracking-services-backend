using FluentValidation;

namespace Application.Requests;

public class UpdateCompanyRequest
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
}

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Id).NotNull()
            .GreaterThan(0).WithMessage("CompanyId must be positive");
        RuleFor(x => x.CompanyName).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
    }
}