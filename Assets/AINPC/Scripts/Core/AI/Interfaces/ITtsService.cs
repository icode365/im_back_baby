using System.Threading.Tasks;
using AINPC.Scripts.Data;
using UnityEngine;

namespace AINPC.Scripts.Core.AI.Interfaces
{
    public interface ITtsService
    {
        public void Initialize(AiSetting aiSetting);
        public Task<ApiResponse> RequestAudioFor(string text);
    }
}