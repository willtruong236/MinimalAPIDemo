using CouponAPI.Models.DTO;
using FluentValidation;

namespace CouponAPI.Validations
{
    public class CreateCouponValidation : AbstractValidator<CouponDTO>
    {
        public CreateCouponValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
