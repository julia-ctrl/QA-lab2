using System;

namespace exception
{
    public interface IServerReporter
    {
        // server answer resault
        bool Report(String kind);
    }
}