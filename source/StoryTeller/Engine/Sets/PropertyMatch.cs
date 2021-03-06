using FubuCore.Reflection;
using StoryTeller.Domain;

namespace StoryTeller.Engine.Sets
{
    public class PropertyMatch : ISetColumn
    {
        protected readonly Accessor _accessor;

        public PropertyMatch(Accessor accessor)
        {
            _accessor = accessor;
        }

        public Cell Cell { get { return new Cell(_accessor.InnerProperty); } }

        public void ReadExpected(ITestContext context, IStep step, SetRow row)
        {
            Cell.ReadArgument(context, step, x =>
            {
                row.Values[Cell.Key] = x;
            });
        }

        public void ReadActual(object target, SetRow row)
        {
            row.Values[_accessor.InnerProperty.Name] = _accessor.GetValue(target);
        }
    }
}