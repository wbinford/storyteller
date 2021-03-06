using NUnit.Framework;
using StoryTeller.Domain;

namespace StoryTeller.Testing.Domain
{
    [TestFixture]
    public class TestFilterTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            unknownAcceptanceTest = DataMother.TestWithNoResults().LifecycleIs(Lifecycle.Acceptance);
            unknownRegressionTest = DataMother.TestWithNoResults().LifecycleIs(Lifecycle.Regression);
            failedAcceptanceTest = DataMother.FailedTest().LifecycleIs(Lifecycle.Acceptance);
            failedRegressionTest = DataMother.FailedTest().LifecycleIs(Lifecycle.Regression);
            successfulAcceptanceTest = DataMother.SuccessfulTest().LifecycleIs(Lifecycle.Acceptance);
            successfulRegressionTest = DataMother.SuccessfulTest().LifecycleIs(Lifecycle.Regression);

            filter = new TestFilter();
        }

        #endregion

        private Test unknownAcceptanceTest;
        private Test unknownRegressionTest;
        private Test failedAcceptanceTest;
        private Test failedRegressionTest;
        private Test successfulAcceptanceTest;
        private Test successfulRegressionTest;
        private TestFilter filter;

        [Test]
        public void by_default_the_lifecycle_is_any()
        {
            filter.Lifecycle.ShouldEqual(Lifecycle.Any);
        }

        [Test]
        public void by_default_the_status_is_all()
        {
            filter.ResultStatus.ShouldEqual(ResultStatus.All);
        }

        [Test]
        public void do_not_show_empty_suites_when_a_result_status_is_selected()
        {
            filter.Lifecycle = Lifecycle.Any;
            filter.ResultStatus = ResultStatus.Success;

            filter.ShowEmptySuites().ShouldBeFalse();
        }

        [Test]
        public void do_not_show_empty_suites_when_lifecyle_is_set()
        {
            filter.Lifecycle = Lifecycle.Acceptance;
            filter.ResultStatus = ResultStatus.All;

            filter.ShowEmptySuites().ShouldBeFalse();
        }

        [Test]
        public void look_for_failed_regression_tests()
        {
            filter.Lifecycle = Lifecycle.Regression;
            filter.ResultStatus = ResultStatus.Failed;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void look_for_successful_acceptance_tests()
        {
            filter.Lifecycle = Lifecycle.Acceptance;
            filter.ResultStatus = ResultStatus.Success;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void only_look_for_acceptance_tests()
        {
            filter.Lifecycle = Lifecycle.Acceptance;

            filter.Matches(unknownAcceptanceTest).ShouldBeTrue();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeTrue();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void only_look_for_failed_tests()
        {
            filter.ResultStatus = ResultStatus.Failed;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeTrue();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void only_look_for_regression_tests()
        {
            filter.Lifecycle = Lifecycle.Regression;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeTrue();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeTrue();
        }

        [Test]
        public void only_look_for_successful_tests()
        {
            filter.ResultStatus = ResultStatus.Success;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeTrue();
        }

        [Test]
        public void only_look_for_unknown_tests()
        {
            filter.ResultStatus = ResultStatus.Unknown;

            filter.Matches(unknownAcceptanceTest).ShouldBeTrue();
            filter.Matches(unknownRegressionTest).ShouldBeTrue();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void show_empty_suites_when_any_lifecycle_and_status_is_all()
        {
            filter.Lifecycle = Lifecycle.Any;
            filter.ResultStatus = ResultStatus.All;

            filter.ShowEmptySuites().ShouldBeTrue();
        }

        [Test]
        public void the_initial_filter_should_match_all_tests()
        {
            filter.Matches(unknownAcceptanceTest).ShouldBeTrue();
            filter.Matches(unknownRegressionTest).ShouldBeTrue();
            filter.Matches(failedAcceptanceTest).ShouldBeTrue();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeTrue();
        }
    }
}