

using CVTrack.Application.CVs.Commands;
using FluentValidation;

namespace CVTrack.Application.CVs.Validators;

public class CreateCvCommandValidator : AbstractValidator<CreateCvCommand>
{
    public CreateCvCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId boş olamaz.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName boş olamaz.")
            .MaximumLength(255).WithMessage("FileName en fazla 255 karakter olabilir.");
    }
}