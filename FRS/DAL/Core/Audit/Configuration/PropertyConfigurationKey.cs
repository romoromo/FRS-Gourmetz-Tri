using System;

namespace DAL.Core.Audit.Configuration
{
    internal class PropertyConfigurationKey
    {
        internal PropertyConfigurationKey(string propertyName, string typeFullName)
        {
            PropertyName = propertyName;
            TypeFullName = typeFullName;
        }

        internal string PropertyName { get; }
        internal string TypeFullName { get; }

        public override bool Equals(object obj)
        {
            var otherEntity = (PropertyConfigurationKey)obj;
            bool isNameSame = otherEntity != null && otherEntity.PropertyName != null && otherEntity.PropertyName.Equals(PropertyName, StringComparison.OrdinalIgnoreCase);
            bool isTypeSame = otherEntity != null && otherEntity.TypeFullName != null && otherEntity.TypeFullName.Equals(TypeFullName, StringComparison.OrdinalIgnoreCase);

            return isNameSame && isTypeSame;
        }

        public override int GetHashCode()
        {
            return (PropertyName + TypeFullName).GetHashCode();
        }

        public override string ToString()
        {
            return TypeFullName + "." + PropertyName;
        }
    }
}
