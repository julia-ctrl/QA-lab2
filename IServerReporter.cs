using System;

namespace exception
{
    public interface IServerReporter
    {
        // �������������� ����� �������
        bool Report(String kind);
    }
}