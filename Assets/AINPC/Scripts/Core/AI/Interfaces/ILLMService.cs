using System.Threading.Tasks;
using AINPC.Scripts.Data;
using UnityEngine.Events;

namespace AINPC.Scripts.Core.AI.Interfaces
{
    public interface ILLMService
    {
        public Task<APIResult> GetResponseAsync(string prompt);
    }
}