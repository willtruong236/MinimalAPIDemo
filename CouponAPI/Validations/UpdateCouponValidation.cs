using CouponAPI.Models.DTO;
using FluentValidation;

namespace CouponAPI.Validations
{
    public class UpdateCouponValidation : AbstractValidator<UpdateCouponDTO>
    {
        public UpdateCouponValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
