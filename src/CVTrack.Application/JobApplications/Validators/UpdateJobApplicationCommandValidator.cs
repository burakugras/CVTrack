using CVTrack.Application.JobApplications.Commands;
using FluentValidation;

namespace CVTrack.Application.JobApplications.Validators;

public class UpdateJobApplicationCommandValidator
        : AbstractValidator<UpdateJobApplicationCommand>
{
    public UpdateJobApplicationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id boş olamaz.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId boş olamaz.");

        RuleFor(x => x.CVId)
            .NotEmpty().WithMessage("CVId boş olamaz.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("CompanyName boş olamaz.")
            .MaximumLength(200).WithMessage("CompanyName en fazla 200 karakter olabilir.");

        RuleFor(x => x.ApplicationDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("ApplicationDate gelecekte olamaz.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz Status değeri.");

        // not: Notes zaten nullable, ekstra kural gerekmez
    }
}