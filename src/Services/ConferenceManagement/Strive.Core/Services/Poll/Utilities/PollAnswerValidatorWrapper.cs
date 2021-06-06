using System;
using System.Reflection;
using Autofac;
using Strive.Core.Dto;

namespace Strive.Core.Services.Poll.Utilities
{
    public class PollAnswerValidatorWrapper
    {
        private readonly IComponentContext _context;

        public PollAnswerValidatorWrapper(IComponentContext context)
        {
            _context = context;
        }

        public Error? Validate(PollInstruction instruction, PollAnswer answer)
        {
            var genericMethod = GetType().GetMethod(nameof(ValidateGeneric),
                BindingFlags.NonPublic | BindingFlags.Instance);

            var instructionType = instruction.GetType();
            var baseType = GetBaseType(instructionType, typeof(PollInstruction<>));

            genericMethod = genericMethod!.MakeGenericMethod(instructionType, baseType);

            return (Error?) genericMethod.Invoke(this, new object[] {instruction, answer})!;
        }

        private Error? ValidateGeneric<TInstruction, TAnswer>(PollInstruction instruction, PollAnswer answer)
            where TAnswer : PollAnswer where TInstruction : PollInstruction<TAnswer>
        {
            if (instruction is not TInstruction typedInstruction)
                throw new ArgumentException(
                    $"The generic parameter {nameof(TAnswer)} must match the {nameof(instruction)}");

            var validator = _context.Resolve<IPollAnswerValidator<TInstruction, TAnswer>>();
            return validator.Validate(typedInstruction, (TAnswer) answer);
        }

        private static Type GetBaseType(Type type, Type baseType)
        {
            while (type.BaseType != null)
            {
                type = type.BaseType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                {
                    return type.GenericTypeArguments[0];
                }
            }

            throw new InvalidOperationException("Base type was not found");
        }
    }
}
