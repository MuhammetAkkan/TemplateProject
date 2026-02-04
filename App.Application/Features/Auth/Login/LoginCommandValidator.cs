using FluentValidation;

namespace App.Application.Features.Auth.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Kullanıcı adı veya e-posta boş olamaz.")
            .MaximumLength(100).WithMessage("Kullanıcı adı veya e-posta en fazla 100 karakter olabilir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parola boş olamaz.")
            .MinimumLength(6).WithMessage("Parola en az 6 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Parola en fazla 100 karakter olabilir.");
    }
}