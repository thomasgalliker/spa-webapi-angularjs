using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HomeCinema.Web.Infrastructure.Validators;

namespace HomeCinema.Web.Models
{
    public class StockViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public Guid UniqueKey { get; set; }
        public bool IsAvailable { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new StockViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}