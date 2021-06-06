using System.Linq;
using FluentValidation;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Strive.Core.Services.Poll.Types.Numeric;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Poll.Types.TagCloud;
using Strive.Hubs.Core.Dtos;

namespace Strive.Hubs.Core.Validators
{
    public class CreatePollDtoValidator : AbstractValidator<CreatePollDto>
    {
        public CreatePollDtoValidator()
        {
            RuleFor(x => x.Instruction).NotNull();
            RuleFor(x => x.Config).NotNull();
            RuleFor(x => x.InitialState).NotNull();

            RuleFor(x => x.Instruction).SetInheritanceValidator(v =>
            {
                v.Add(new SingleChoiceInstructionValidator());
                v.Add(new MultipleChoiceInstructionValidator());
                v.Add(new NumericInstructionValidator());
                v.Add(new TagCloudInstructionValidator());
            });
        }

        private class SingleChoiceInstructionValidator : AbstractValidator<SingleChoiceInstruction>
        {
            public SingleChoiceInstructionValidator()
            {
                RuleFor(x => x.Options).NotEmpty();
                RuleFor(x => x.Options).Must(x => x.Distinct().Count() == x.Length)
                    .WithMessage("Options must be unique");
            }
        }

        private class MultipleChoiceInstructionValidator : AbstractValidator<MultipleChoiceInstruction>
        {
            public MultipleChoiceInstructionValidator()
            {
                RuleFor(x => x.Options).NotEmpty();
                RuleFor(x => x.Options).Must(x => x.Distinct().Count() == x.Length)
                    .WithMessage("Options must be unique");
                RuleFor(x => x.MaxSelections).GreaterThan(0);
            }
        }

        private class NumericInstructionValidator : AbstractValidator<NumericInstruction>
        {
            public NumericInstructionValidator()
            {
                RuleFor(x => x).Must(x => x.Min == null || x.Max == null || x.Max > x.Min)
                    .WithMessage("Min must be smaller than max");
            }
        }

        private class TagCloudInstructionValidator : AbstractValidator<TagCloudInstruction>
        {
            public TagCloudInstructionValidator()
            {
                RuleFor(x => x.Mode).IsInEnum();
                RuleFor(x => x.MaxTags).GreaterThanOrEqualTo(1);
            }
        }
    }
}
