using System;

namespace exception
{
    public interface IServerReporter
    {
        // результирующий ответ сервера
        bool Report(String kind);
    }
}