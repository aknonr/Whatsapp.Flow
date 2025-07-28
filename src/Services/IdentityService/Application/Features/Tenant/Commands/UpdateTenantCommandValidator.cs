using FluentValidation;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
    {
        public UpdateTenantCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty();
                
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Tenant name is required.")
                .MaximumLength(100).WithMessage("Tenant name must not exceed 100 characters.");

            RuleFor(v => v.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

            RuleFor(v => v.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(v => v.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required.");
        }
    }
} 