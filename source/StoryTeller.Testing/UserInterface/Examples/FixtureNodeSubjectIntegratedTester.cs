using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Examples;
using StoryTeller.Model;
using StoryTeller.UserInterface.Examples;
using StoryTeller.UserInterface.Screens;
using StructureMap;

namespace StoryTeller.Testing.UserInterface.Examples
{
    [TestFixture]
    public class FixtureNodeSubjectIntegratedTester
    {
        [Test]
        public void matches_by_fixture_node()
        {
            var fixture1 = new FixtureGraph("fixture1");
            var fixture2 = new FixtureGraph("fixture2");

            var screen1 = new FixtureNodePresenter(null, fixture1, null);
            var screen2 = new FixtureNodePresenter(null, fixture2, null);

            var subject = new FixtureNodeSubject(fixture1);

            subject.Matches(screen1).ShouldBeTrue();
            subject.Matches(screen2).ShouldBeFalse();

            var randomScreen = MockRepository.GenerateMock<IScreen>();
            subject.Matches(randomScreen).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_creating_the_screen
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IFixtureNodeView>().Use<FixtureNodeView>();
                x.For<IExampleSource>().Use(MockRepository.GenerateMock<IExampleSource>());
                x.For<IScreen<IFixtureNode>>().Use<FixtureNodePresenter>();
            });

            factory = new ScreenFactory(container);

            fixture = new FixtureGraph("fixture1");

            subject = new FixtureNodeSubject(fixture);

            thePresenter = subject.CreateScreen(factory).ShouldBeOfType<FixtureNodePresenter>();
        }

        #endregion

        private Container container;
        private FixtureGraph fixture;
        private FixtureNodeSubject subject;
        private FixtureNodePresenter thePresenter;
        private ScreenFactory factory;

        [Test]
        public void the_presenter_has_the_fixture_as_the_subject()
        {
            thePresenter.Subject.ShouldBeTheSameAs(fixture);
        }
    }
}