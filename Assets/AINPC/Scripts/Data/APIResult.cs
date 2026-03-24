using UnityEngine;

namespace AINPC.Scripts.Data
{
    public class APIResult
    {
        public string Response = string.Empty;
        public string Error = string.Empty;
        public EAPIStatus Status = EAPIStatus.Error;
    }

    public enum EAPIStatus
    {
        Error,
        Success,
        Processing
    }
}