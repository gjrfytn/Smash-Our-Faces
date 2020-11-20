using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sof.Auxiliary
{
    public class AsyncEvent
    {
        private readonly List<System.Func<Task>> _Subscribers = new List<System.Func<Task>>();

        public Task Invoke() => Task.WhenAll(_Subscribers.Select(s => s()).ToArray());

        public void AddSubscriber(System.Func<Task> action)
        {
            if (action == null)
                throw new System.ArgumentNullException(nameof(action));

            _Subscribers.Add(action);
        }

        public void RemoveSubscriber(System.Func<Task> action)
        {
            if (action == null)
                throw new System.ArgumentNullException(nameof(action));

            _Subscribers.Remove(action);
        }
    }

    public class AsyncEvent<T>
    {
        private readonly List<System.Func<T, Task>> _Subscribers = new List<System.Func<T, Task>>();

        public Task Invoke(T arg) => Task.WhenAll(_Subscribers.Select(s => s(arg)));

        public void AddSubscriber(System.Func<T, Task> action)
        {
            if (action == null)
                throw new System.ArgumentNullException(nameof(action));

            _Subscribers.Add(action);
        }

        public void RemoveSubscriber(System.Func<T, Task> action)
        {
            if (action == null)
                throw new System.ArgumentNullException(nameof(action));

            _Subscribers.Remove(action);
        }
    }
}
