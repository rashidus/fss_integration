using System;
using System.Configuration;

namespace FSSExtensions.Tools
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class StandardRuntimeEnumValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Type enumType;

        public Type EnumType
        {
            get
            {
                return this.enumType;
            }
            set
            {
                this.enumType = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new StandardRuntimeEnumValidator(this.enumType);
            }
        }

        public StandardRuntimeEnumValidatorAttribute(Type enumType)
        {
            this.EnumType = enumType;
        }
    }
    internal class StandardRuntimeEnumValidator : ConfigurationValidatorBase
    {
        private Type enumType;

        public StandardRuntimeEnumValidator(Type enumType)
        {
            this.enumType = enumType;
        }

        public override bool CanValidate(Type type)
        {
            return type.IsEnum;
        }

        public override void Validate(object value)
        {
            if (!Enum.IsDefined(this.enumType, value))
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value, this.enumType));
            }
        }
    }
}
