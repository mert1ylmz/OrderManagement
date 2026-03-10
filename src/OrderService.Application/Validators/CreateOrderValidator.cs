using FluentValidation;
using OrderService.Application.DTOs;


namespace OrderService.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderNumber)
            .NotEmpty().WithMessage("Sipariş numarası bboş olamaz.")
            .MaximumLength(20).WithMessage("Sipariş numarası en fazla 20 karakter olabilir.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Müşteri adı boş olamaz")
            .MinimumLength(3).WithMessage("Müşteri adı en az 3 karakter olmalıdır.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Sipariş tutarı 0'dan büyük olmalıdır.");
    }
}