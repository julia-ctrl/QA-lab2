using System;
using System.ComponentModel;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace exception.tests
{
    public class TestData
    {
        public static List<Type> CriticalExceptions = new List<Type>()
        {
            typeof(DivideByZeroException),
            typeof(OutOfMemoryException),
            typeof(StackOverflowException),
            typeof(InsufficientMemoryException),
            typeof(InsufficientExecutionStackException)
        };
        public static List<Type> NonCriticalExceptions = new List<Type>()
        {
            typeof(ArgumentNullException),
            typeof(ArgumentOutOfRangeException),
            typeof(NullReferenceException),
            typeof(AccessViolationException),
            typeof(IndexOutOfRangeException),
            typeof(InvalidOperationException)
        };
    }

    [SetUpFixture]
    public class TestSetup
    {
        public static System.ComponentModel.IListSource ListSource;
        public static IServerReporter NormalServerReporter;
        public static IServerReporter FailingServerReporter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ListSource = Substitute.For<IListSource>();
            ListSource.GetList().Returns(TestData.CriticalExceptions);

            NormalServerReporter = Substitute.For<IServerReporter>();
            NormalServerReporter.Report(Arg.Any<String>()).Returns(true);
            FailingServerReporter = Substitute.For<IServerReporter>();
            FailingServerReporter.Report(Arg.Any<String>()).Returns(false);
        }
    }
    [TestFixture]
    public class ExceptionController
    {
        [TestCase(typeof(DivideByZeroException), true)]
        [TestCase(typeof(OutOfMemoryException), true)]
        [TestCase(typeof(StackOverflowException), true)]
        [TestCase(typeof(InsufficientMemoryException), true)]
        [TestCase(typeof(InsufficientExecutionStackException), true)]
        [TestCase(typeof(ArgumentNullException), false)]
        [TestCase(typeof(ArgumentOutOfRangeException), false)]
        [TestCase(typeof(NullReferenceException), false)]
        [TestCase(typeof(AccessViolationException), false)]
        [TestCase(typeof(IndexOutOfRangeException), false)]
        [TestCase(typeof(InvalidOperationException), false)]
        public void IsCritical_CriticalityCheck_CorrectMethodRun(Type exceptionType, bool expectedResult)
        {
            // arrange
            var instance = (Exception)Activator.CreateInstance(exceptionType);
            // 1. Use a LW2Factory
            var controller = new ExceptionControllerFactory()
                .WithList(TestSetup.ListSource)
                .WithServer(TestSetup.NormalServerReporter)
                .Create();

            try
            {
                // act
                throw instance;
            }
            catch (Exception e)
            {
                // assert
                Assert.AreEqual(expectedResult, controller.IsCritical(e));
                return;
            }
        }

        [Test]
        public void CountExceptions_ProvidedListExceptionCount_CorrectCorrect()
        {
            // arrange
            // 2. Use constructor
            var controller = new ExceptionControllerFactory()
                .WithList(TestSetup.ListSource)
                .WithServer(TestSetup.NormalServerReporter)
                .Create();

            // act
            foreach (var item in TestData.CriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                controller.CountExceptions(instance);
            }
            foreach (var item in TestData.NonCriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                controller.CountExceptions(instance);
            }

            // assert
            Assert.AreEqual(controller.CounterCriticalExceptions, TestData.CriticalExceptions.Count);
            Assert.AreEqual(controller.CounterNotCriticalExceptions, TestData.NonCriticalExceptions.Count);
        }

        [Test]
        public void CountExceptions_InitState_Zero()
        {
            // arrange
            // 3. Use property access
            var controller = new ExceptionControllerFactory()
                .WithList(TestSetup.ListSource)
                .WithServer(TestSetup.NormalServerReporter)
                .Create();

            // act: nothing

            // assert
            Assert.AreEqual(controller.CounterCriticalExceptions, 0);
            Assert.AreEqual(controller.CounterNotCriticalExceptions, 0);
        }

        [Test]
        public void IServerReporter_FailureServerAnswersCounter_CorrectCount()
        {
            // arrange
            var controllerNormal = new ExceptionController(TestSetup.ListSource, TestSetup.NormalServerReporter);
            var controllerFailling = new ExceptionController(TestSetup.ListSource, TestSetup.FailingServerReporter);

            // act
            foreach (var item in TestData.CriticalExceptions)
            {
                var instance = (Exception)Activator.CreateInstance(item);
                controllerNormal.CountExceptions(instance);
                controllerFailling.CountExceptions(instance);
            }

            // assert
            Assert.AreEqual(controllerNormal.ReportFailures, 0);
            Assert.AreEqual(controllerFailling.ReportFailures, TestData.CriticalExceptions.Count);
        }

        [Test]
        public void TelemetryReport_InitState_Zero()
        {
            // arrange
            var controller = new ExceptionControllerFactory()
                .WithList(TestSetup.ListSource)
                .WithServer(TestSetup.NormalServerReporter)
                .Create();

            // assert
            Assert.AreEqual(controller.ReportFailures, 0);
        }
    }
}