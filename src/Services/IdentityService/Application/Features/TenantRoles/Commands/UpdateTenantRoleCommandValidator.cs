using FluentValidation;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class UpdateTenantRoleCommandValidator : AbstractValidator<UpdateTenantRoleCommand>
    {
        public UpdateTenantRoleCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty();

            RuleFor(v => v.Description)
                .MaximumLength(300).WithMessage("Description must not exceed 300 characters.");
            
            // Permissions listesi için daha karmaşık kurallar eklenebilir.
            // Örneğin, her permission'ın bir Resource'u olması gerektiği gibi.
            RuleForEach(v => v.Permissions).ChildRules(permission =>
            {
                permission.RuleFor(p => p.Resource).NotEmpty();
            });
        }
    }
} 