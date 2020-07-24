using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace ReviewApi.Application.Validators.Extensions
{
    public static class AbstractValidatorExtensions
    {
        public static async Task ValidateRequestModelAndThrow<T>(this AbstractValidator<T> validator, T requestModel)
        {
            ValidationResult validationResponse = await validator.ValidateAsync(requestModel);
            if (!validationResponse.IsValid)
            {
                StringBuilder builder = new StringBuilder();
                ValidationFailure lastElement = validationResponse.Errors[validationResponse.Errors.Count - 1];
                validationResponse.Errors.Remove(lastElement);
                validationResponse.Errors.ToList().ForEach(e => builder.AppendLine(e.ErrorMessage));
                builder.Append(lastElement.ErrorMessage);
                string errors = builder.ToString();
                throw new ValidationException(errors.Replace(errors));
            }
        }
    }
}
