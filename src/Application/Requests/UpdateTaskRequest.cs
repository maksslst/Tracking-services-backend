using FluentValidation;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Requests;

public class UpdateTaskRequest
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public string Description { get; set; } = null!;
    public int AssignedUserId { get; set; }
    public TaskStatus Status { get; set; }
}

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Id).NotNull()
            .GreaterThan(0).WithMessage("Id must be positive");
        RuleFor(x => x.ResourceId).NotNull()
            .GreaterThan(0).WithMessage("ResourceId must be positive")
            .LessThan(int.MaxValue).WithMessage("ResourceId is too long");
        RuleFor(x => x.Description).NotEmpty()
            .MaximumLength(ValidationConstants.TaskDescriptionLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.AssignedUserId).NotNull()
            .GreaterThan(0).WithMessage("AssignedUserId must be positive")
            .LessThan(int.MaxValue).WithMessage("AssignedUserId is too long");
        RuleFor(x => x.Status).NotNull();
    }
}