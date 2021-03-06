using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.UserInterface.Dialogs;
using StoryTeller.UserInterface.Projects;

namespace StoryTeller.Testing.UserInterface.Projects
{
    namespace StoryTeller.Testing.UserInterface.Projects
    {
        [TestFixture]
        public class when_handling_the_project_load_failure_message : InteractionContext<ProjectLoadFailureHandler>
        {
            private ProjectLoadFailureMessage message;

            protected override void beforeEach()
            {
                message = new ProjectLoadFailureMessage
                {
                    ErrorMessage = "Bad Error!",
                    ProjectToken = new ProjectToken
                    {
                        Filename = "somefilename",
                        Name = "somename"
                    }
                };

                ClassUnderTest.HandleMessage(message);
            }

            [Test]
            public void should_launch_a_dialog_with_itself() { MockFor<IDialogLauncher>().AssertWasCalled(x => x.Launch<IProjectLoadFailureHandler>(ClassUnderTest)); }

            [Test]
            public void should_store_the_message() { ClassUnderTest.ErrorText.ShouldEqual(message.ErrorMessage); }

            [Test]
            public void should_store_the_project_token()
            {
                ClassUnderTest.ProjectToken.ShouldEqual(message.ProjectToken);
            }
        }
    }
}