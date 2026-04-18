using AINPC.Scripts.Core.Gameplay.Validator;

namespace AINPC.Scripts.Core.Gameplay.Interfaces
{
    public interface IValidator
    {
        public ValidationResult Validate(IRecipe targetRecipe, IRecipe correctRecipe);
    }
}