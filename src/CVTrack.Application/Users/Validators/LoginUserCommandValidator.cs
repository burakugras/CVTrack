using CVTrack.Application.Users.Commands;
using FluentValidation;

namespace CVTrack.Application.Users.Validators;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email girin.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password boş olamaz.");
    }
}