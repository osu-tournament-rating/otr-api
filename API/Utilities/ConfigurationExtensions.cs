using System.ComponentModel.DataAnnotations;

namespace API.Utilities
{
    public static class ConfigurationExtensions
    {
        public static T BindAndValidate<T>(this IConfiguration configuration, string sectionName)
            where T : class, new()
        {
            T section = configuration.GetSection(sectionName).Get<T>() ?? throw new InvalidOperationException($"Section {sectionName} not found in configuration.");
            var context = new ValidationContext(section, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(section, context, validationResults, true);

            if (!isValid)
            {
                string?[] errorMessages = validationResults.Select(result => result.ErrorMessage).ToArray();
                throw new InvalidOperationException(
                    $"Configuration validation failed for {sectionName}: {string.Join(", ", errorMessages)}"
                );
            }

            return section;
        }
    }
}
