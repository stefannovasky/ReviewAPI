using System;

namespace ReviewApi.Shared.Interfaces
{
    public interface ILogUtils
    {
        void LogError(Exception exception);
    }
}
