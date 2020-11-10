using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnitLib
{
    /// <summary>
    /// Represents a collection of class methods needed for testing.
    /// </summary>
    public class ClassMethods
    {
        public ConcurrentQueue<MethodInfo> BeforeClassTestMethods;
        public ConcurrentQueue<MethodInfo> BeforeTestMethods;
        public ConcurrentQueue<MethodInfo> TestMethods;
        public ConcurrentQueue<MethodInfo> AfterTestMethods;
        public ConcurrentQueue<MethodInfo> AfterClassTestMethods;

        /// <summary>
        /// Amount of methods to test in a class.
        /// </summary>
        public int TestsCount
            => TestMethods.Count;

        /// <summary>
        /// Builds a <see cref="ClassMethods"/> istance from a type using reflexion.
        /// </summary>
        public ClassMethods(Type type)
        {
            BeforeClassTestMethods = new ConcurrentQueue<MethodInfo>();
            BeforeTestMethods = new ConcurrentQueue<MethodInfo>();
            TestMethods = new ConcurrentQueue<MethodInfo>();
            AfterTestMethods = new ConcurrentQueue<MethodInfo>();
            AfterClassTestMethods = new ConcurrentQueue<MethodInfo>();

            FillMethodQueues(type);
        }

        /// <summary>
        /// Gets all the methods from the type and fills the queues according to the method's attributes.
        /// </summary>
        public void FillMethodQueues(Type type)
        {
            Parallel.ForEach(type.GetMethods(), method =>
            {
                if (method.GetCustomAttribute<TestAttribute>() != null)
                {
                    TryToEnqueueMethod(method, TestMethods);
                }
                else if (method.GetCustomAttribute<BeforeClassAttribute>() != null)
                {
                    if (!method.IsStatic)
                    {
                        throw new FormatException("Methods invoked before testing the class must be static.");
                    }

                    TryToEnqueueMethod(method, BeforeClassTestMethods);
                }
                else if (method.GetCustomAttribute<BeforeAttribute>() != null)
                {
                    TryToEnqueueMethod(method, BeforeTestMethods);
                }
                else if (method.GetCustomAttribute<AfterAttribute>() != null)
                {
                    TryToEnqueueMethod(method, AfterTestMethods);
                }
                else if (method.GetCustomAttribute<AfterClassAttribute>() != null)
                {
                    if (!method.IsStatic)
                    {
                        throw new FormatException("Methods invoked after testing the class must be static.");
                    }

                    TryToEnqueueMethod(method, AfterClassTestMethods);
                }
            });
        }

        /// <summary>
        /// Enqueues a method to the queue.
        /// </summary>
        private void TryToEnqueueMethod(MethodInfo method, ConcurrentQueue<MethodInfo> queue)
        {
            if (!IsMethodAppropriate(method))
            {
                throw new FormatException("Method shouldn't return value or get parameters");
            }

            queue.Enqueue(method);
        }

        /// <summary>
        /// Checks if a method has no parameters and doesn't return anything.
        /// </summary>
        private bool IsMethodAppropriate(MethodInfo method)
            => (method.GetParameters().Length == 0) && (method.ReturnType == typeof(void));
    }
}

