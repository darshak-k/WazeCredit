using WazeCredit.Models;

namespace WazeCredit.Services;

public class CreditValidationChecker : IValidationChecker
{
    public string ErrorMessage => "You did not meet Age/Salary/Credit Requirements.";

    public bool ValidatorLogic(CreditApplication model)
    {
        if(DateTime.Now.AddYears(-18) < model.DOB)
        {
            return false;
        }

        if(model.Salary < 100000)
        {
            return false;
        }

        return true;
    }
}
