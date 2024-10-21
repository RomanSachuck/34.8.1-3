using FluentValidation;
using HomeApi.Contracts.Models.Rooms;

namespace HomeApi.Contracts.Validation
{
    public class EditRoomReqestValidator : AbstractValidator<EditRoomRequest>
    {
        public EditRoomReqestValidator() 
        {
            RuleFor(x => x.NewName).NotEmpty(); 
            RuleFor(x => x.NewArea).NotEmpty().Must(BeSupportedArea)
                .WithMessage("The area of the room should be greater than zero!");
            RuleFor(x => x.NewVoltage).NotEmpty()
                .InclusiveBetween(120, 220)
                .WithMessage("The voltage must be in the range from 120 to 220!");
            RuleFor(x => x.NewGasConnected).NotEmpty();
        }

        private bool BeSupportedArea(int area) => 
            area > 0;
    }
}