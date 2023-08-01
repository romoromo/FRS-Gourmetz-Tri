using System;
using FRS.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class SanitizedModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.IsComplexType || context.Metadata.ModelType == typeof(string))
        {
            return null;
        }

        var propertyAttributes = context.Metadata.ContainerType
                                   .GetProperty(context.Metadata.PropertyName)
                                   .GetCustomAttributes(typeof(SanitizeAttribute), inherit: true);

        if (propertyAttributes.Length > 0)
        {
            return new SanitizedModelBinder();
        }

        return null;
    }
}
