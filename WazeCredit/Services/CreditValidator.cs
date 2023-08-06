using WazeCredit.Models;

namespace WazeCredit.Services;


public class CreditValidator : ICreditValidator
{
    private readonly IEnumerable<IValidationChecker> _validations;

    public CreditValidator(IEnumerable<IValidationChecker> validations)
    {
        this._validations = validations;
    }

    public async Task<(bool, IEnumerable<string>)> PassAllValidations(CreditApplication model)
    {
        bool validationPassed = true;

        List<string> errorMessages = new List<string>();

        foreach(var validation in _validations)
        {
            if (!validation.ValidatorLogic(model))
            {
                errorMessages.Add(validation.ErrorMessage);    
                validationPassed = false;
            }
        }

        return (validationPassed, errorMessages);
    }
}
