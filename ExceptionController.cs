using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace exception
{
    public class ExceptionController
    {
        IListSource _exceptionListSource;
        IServerReporter _telemetryReporter;

        public int CounterCriticalExceptions { get; private set; }

        public int CounterNotCriticalExceptions { get; private set; }

        public int ReportFailures { get; private set; }

        public ExceptionController() { }

        static void Main(string[] args)
        {

        }

        public ExceptionController(IListSource src, IServerReporter reporter)
        {
            _exceptionListSource = src;
            _telemetryReporter = reporter;
        }

        public bool IsCritical(Exception exception)
        {
            var criticalExceptions = _exceptionListSource.GetList();
            return criticalExceptions.Contains(exception.GetType());
        }
        
        public void CountExceptions(Exception exception)
        {
            if (IsCritical(exception))
            {
                CounterCriticalExceptions += 1;
                if (!_telemetryReporter.Report(exception.ToString()))
                {
                    ReportFailures += 1;
                }
            }
            else
            {
                CounterNotCriticalExceptions += 1;
            }
        }

        public IListSource ExceptionListSource
        {
            set { _exceptionListSource = value; }
        }

        public IServerReporter TelemetryReporter
        {
            set { _telemetryReporter = value; }
        }
    }
}