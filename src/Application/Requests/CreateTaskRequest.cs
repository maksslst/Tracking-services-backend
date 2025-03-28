using FluentValidation;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Requests;

public class CreateTaskRequest
{
    public int ResourceId { get; set; }
    public string Description { get; set; } = null!;
    public int AssignedUserId { get; set; }
    public int CreatedById { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Opened;
}

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.ResourceId).NotNull()
            .GreaterThan(0).WithMessage("ResourceId must be positive")
            .LessThan(int.MaxValue).WithMessage("ResourceId is too long");
        RuleFor(x => x.Description).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.AssignedUserId).NotNull()
            .GreaterThan(0).WithMessage("AssignedUserId must be positive")
            .LessThan(int.MaxValue).WithMessage("AssignedUserId is too long");
        RuleFor(x => x.CreatedById).NotNull()
            .GreaterThan(0).WithMessage("CreatedById must be positive")
            .LessThan(int.MaxValue).WithMessage("CreatedById is too long");
        RuleFor(x => x.Status).NotNull();
    }
}