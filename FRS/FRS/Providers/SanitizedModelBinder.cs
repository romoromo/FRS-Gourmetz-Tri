using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FRS.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class SanitizedModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Get the property descriptor for the property being bound
        var propertyDescriptor = TypeDescriptor.GetProperties(bindingContext.ModelType)
                                               .Find(bindingContext.ModelName, true);

        // Check if the property type is a string (you can add more conditions for other types if needed)
        if (propertyDescriptor.PropertyType == typeof(string))
        {
            // Check if the property has the SanitizeAttribute
            var sanitizeAttribute = propertyDescriptor.Attributes.OfType<SanitizeAttribute>().FirstOrDefault();
            if (sanitizeAttribute != null)
            {
                // Get the value of the property from the request
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                var value = valueProviderResult.FirstValue;

                // Sanitize the value directly in ASP.NET Core
                string sanitizedValue = SanitizeString(value);

                // Set the sanitized value to the property
                bindingContext.Result = ModelBindingResult.Success(sanitizedValue);
                return Task.CompletedTask;
            }
        }

        // Use the default model binding behavior for other property types or properties without the SanitizeAttribute
        return Task.CompletedTask;
    }

    // Implement your own sanitization logic here
    private string SanitizeString(string input)
    {
        // Implement your custom sanitization logic here
        // For example, you can remove any script tags and other potential harmful content
        // For demonstration purposes, let's just replace "<" and ">" with their HTML entities
        return input?.Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
