using System.Windows.Input;
using FubuCore;
using StoryTeller.Domain;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Screens;

namespace StoryTeller.UserInterface.Tests
{
    public class TestScreen : IScreen<Test>, ICloseable
    {
        private readonly ITestPresenter _presenter;
        private readonly Test _test;
        private readonly ITestStateManager _stateManager;
        private readonly ITestScreenCloser _closer;
        private readonly IEditTestController _controller;
        private readonly ITestView _view;

        public TestScreen(ITestPresenter presenter, ITestView view, Test test, ITestStateManager stateManager, ITestScreenCloser closer, IEditTestController controller)
        {
            _presenter = presenter;
            _view = view;
            _test = test;
            _stateManager = stateManager;
            _closer = closer;
            _controller = controller;
        }


        public Test Test { get { return _test; } }

        #region ICloseable Members

        public void AddCanCloseMessages(CloseToken token)
        {
            if (_stateManager.IsDirty())
            {
                string message = "'{0}' has unsaved changes and will be saved on close".ToFormat(_test.Name);
                token.AddMessage(message);
            }
        }

        public void PerformShutdown()
        {
            _presenter.PerformShutdown();
        }

        #endregion

        #region IScreen<Test> Members

        public Test Subject { get { return _test; } }

        public object View { get { return _view; } }

        public string Title { get { return _test.Name; } }

        public void Activate(IScreenObjectRegistry screenObjects)
        {
            screenObjects.Action("Run").Bind(ModifierKeys.Control, Key.D1)
                .To(_controller.RunCommand).Icon = Icon.Run;

            screenObjects.Action("Cancel").Bind(ModifierKeys.Control, Key.D2)
                .To(_controller.CancelCommand).Icon = Icon.Stop;

            screenObjects.Action("Save").Bind(ModifierKeys.Control, Key.S)
                .To(_controller.SaveCommand).Icon = Icon.Save;

            screenObjects.Action("Preview").Bind(ModifierKeys.Control | ModifierKeys.Shift, Key.P).To(
                _presenter.Modes[Mode.Preview]).Icon = Icon.Unknown;

            screenObjects.Action("Results").Bind(ModifierKeys.Control | ModifierKeys.Shift, Key.R).To(
                _presenter.Modes[Mode.Results]).Icon = Icon.Unknown;

            screenObjects.Action("Xml").Bind(ModifierKeys.Control | ModifierKeys.Shift, Key.X).To(
                _presenter.Modes[Mode.Xml]).
                Icon = Icon.Unknown;

            screenObjects.Action("Edit").Bind(ModifierKeys.Control | ModifierKeys.Shift, Key.E).To(
                _presenter.Modes[Mode.Edit])
                .Icon = Icon.Unknown;

            // TODO -- replace with the test outline
            //screenObjects.PlaceInExplorerPane(_actions, "Actions");

            _presenter.Start();
        }

        public bool CanClose()
        {
            return _closer.CanClose();
        }

        public void Dispose()
        {
        }

        #endregion
    }
}