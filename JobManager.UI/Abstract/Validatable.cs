using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JobManager.UI
{
    public abstract class Validatable : Observable, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> validationErrors;
        private readonly Collection<ValidationResult> validationResults;
        private readonly Collection<string> validatableProperties;

        public Validatable()
        {
            validationErrors = new Dictionary<string, List<string>>();
            validationResults = new Collection<ValidationResult>();
            validatableProperties = new Collection<string>();
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => validationErrors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && validationErrors.ContainsKey(propertyName))
            {
                return validationErrors[propertyName];
            }
            else return null;
        }
        public bool TryValidateModel()
        {
            ValidationContext context = new ValidationContext(this);

            validationResults.Clear();
            validationErrors.Clear();

            if (!Validator.TryValidateObject(this, context, validationResults, true))
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    string property = validationResult.MemberNames.ElementAt(0);
                    if (validationErrors.ContainsKey(property))
                    {
                        validationErrors[property].Add(validationResult.ErrorMessage);
                    }
                    else
                    {
                        validationErrors.Add(property, new List<string> { validationResult.ErrorMessage });
                    }
                }

                foreach (var property in GetValidatableProperties())
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
                return false;
            }

            foreach (var property in GetValidatableProperties())
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
            return true;
        }
        public void ClearValidationErrors()
        {
            validationErrors.Clear();
            foreach (var property in GetValidatableProperties())
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }
        private IEnumerable<string> GetValidatableProperties()
        {
            if (!validatableProperties.Any())
            {
                foreach (var property in GetType().GetProperties())
                {
                    if (property.CustomAttributes.Any())
                        validatableProperties.Add(property.Name);
                }
            }

            return validatableProperties;
        }


        //protected void ValidateProperty<T>(T value, [CallerMemberName] string propertyName = null)
        //{
        //    var results = new List<ValidationResult>();
        //    ValidationContext context = new ValidationContext(this) { MemberName = propertyName };

        //    if (Validator.TryValidateProperty(value, context, results))
        //        validationErrors.Remove(propertyName);
        //    else
        //        validationErrors[propertyName] = results.Select(vr => vr.ToString()).ToList();

        //    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        //}
    }
}
