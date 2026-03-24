using System.Threading.Tasks;
using UnityEngine.Events;

namespace AINPC.Scripts.Core.AI.Interfaces
{
    public interface ILLMService
    {
        public Task<string> GetResponse(string prompt);
    }
}