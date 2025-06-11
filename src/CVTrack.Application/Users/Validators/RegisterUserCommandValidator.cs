using CVTrack.Application.Users.Commands;
using FluentValidation;

namespace CVTrack.Application.Users.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email girin.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password boş olamaz.")
            .MinimumLength(6).WithMessage("Password en az 6 karakter olmalı.");
    }
}