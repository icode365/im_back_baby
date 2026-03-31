using UnityEngine;

namespace AINPC.Scripts.Data
{
    public class ApiResponse
    {
        public string response = string.Empty;
        public object responseObject = null;
        public string error = string.Empty;
        public EAPIStatus status = EAPIStatus.Error;
        public ResponseType responseType = ResponseType.TEXT;
    }

    public enum EAPIStatus
    {
        Error,
        Success,
        Processing
    }

    public enum ResponseType
    {
        TEXT,
        AUDIO
    }
}