using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Helios.Web.Tests {
    internal class ModelTestHelper {
        internal static IEnumerable<ValidationResult> ValidateModel<T>(T model) {
            var type = typeof(T);
            var meta = type.GetCustomAttributes(false).OfType<MetadataTypeAttribute>().FirstOrDefault();
            if (meta != null) {
                type = meta.MetadataClassType;
            }

            ValidationContext context = new ValidationContext(model, null, null);

            foreach (var info in type.GetProperties()) {
                var attributes = info.GetCustomAttributes(false).OfType<ValidationAttribute>();

                context.MemberName = info.Name;
                foreach (var attribute in attributes) {
                    var result = attribute.GetValidationResult(info.GetValue(model, null), context);
                    if (result != ValidationResult.Success) {
                        yield return result;
                    }
                }
            }
        }
    }
}