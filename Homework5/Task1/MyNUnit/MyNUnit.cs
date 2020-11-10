using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyNUnitLib
{
    /// <summary>
    /// Main testing class.
    /// </summary>
    public static class MyNUnit
    {
        private static ConcurrentDictionary<Type, ClassMethods> methodsToTest;
        private static ConcurrentDictionary<Type, ConcurrentBag<TestInfo>> testResults;

        /// <summary>
        /// Default constructor.
        /// </summary>
        static MyNUnit()
        {
            methodsToTest = new ConcurrentDictionary<Type, ClassMethods>();
            testResults = new ConcurrentDictionary<Type, ConcurrentBag<TestInfo>>();
        }

        /// <summary>
        /// Runs all the tests from the specified path prints the results to the console.
        /// </summary>
        public static void RunTestsAndPrintReport(string path)
        {
            AnalyzePathAndExecuteTests(path);

            PrintResults();
        }

        /// <summary>
        /// Runs all the test and returns the results.
        /// </summary>
        public static Dictionary<Type, List<TestInfo>> RunTestsAndGetReport(string path)
        {
            AnalyzePathAndExecuteTests(path);

            return GetDictionaryOfReports();
        }

        /// <summary>
        /// Prepares the assemblies and runs all the tests.
        /// </summary>
        private static void AnalyzePathAndExecuteTests(string path)
        {
            var classes = GetTypes(path);

            Parallel.ForEach(classes, someClass =>
            {
                EnqueueClassForTesting(someClass);
            });

            ExecuteAllTests();
        }

        /// <summary>
        /// Transforms the results of testing.
        /// </summary>
        private static Dictionary<Type, List<TestInfo>> GetDictionaryOfReports()
        {
            var result = new Dictionary<Type, List<TestInfo>>();

            foreach (var type in testResults.Keys)
            {
                result.Add(type, new List<TestInfo>());

                foreach (var testInfo in testResults[type])
                {
                    result[type].Add(testInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Loads all the types from the assemblies inside the specified directory.
        /// </summary>
        private static IEnumerable<Type> GetTypes(string path)
            => GetAssemblyPaths(path).Select(Assembly.LoadFrom).SelectMany(a => a.ExportedTypes).Where(t => t.IsClass);

        /// <summary>
        /// Finds all the assemblies except for the testing library within a directory and gets their paths.
        /// </summary>
        /// <returns>A collcection of paths.</returns>
        private static IEnumerable<string> GetAssemblyPaths(string path)
        {
            var result = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(path, "*.exe", SearchOption.AllDirectories)).ToList();
            result.RemoveAll(assemblyPath => assemblyPath.Contains("\\MyNUnit.exe"));
            return result;
        }

        /// <summary>
        /// Adds a class to the testing dictionary.
        /// </summary>
        private static void EnqueueClassForTesting(Type type)
            => methodsToTest.TryAdd(type, new ClassMethods(type));

        /// <summary>
        /// Sets up an execution of all the test methods from their corresponding classes.
        /// </summary>
        private static void ExecuteAllTests()
        {
            Parallel.ForEach(methodsToTest.Keys, type =>
            {
                testResults.TryAdd(type, new ConcurrentBag<TestInfo>());

                foreach (var beforeClassMethod in methodsToTest[type].BeforeClassTestMethods)
                {
                    ExecuteUtilityMethod(beforeClassMethod, null);
                }

                foreach (var testMethod in methodsToTest[type].TestMethods)
                {
                    ExecuteTestMethod(type, testMethod);
                }

                foreach (var afterClassMethod in methodsToTest[type].AfterClassTestMethods)
                {
                    ExecuteUtilityMethod(afterClassMethod, null);
                }
            });
        }

        /// <summary>
        /// Executes a test method and gets the testing info.
        /// </summary>
        private static void ExecuteTestMethod(Type type, MethodInfo method)
        {
            var attribute = method.GetCustomAttribute<TestAttribute>();
            var isSuccessful = false;
            Type thrownException = null;

            var emptyConstructor = type.GetConstructor(Type.EmptyTypes);

            if (emptyConstructor == null)
            {
                throw new FormatException($"{type.Name} must have parameterless constructor");
            }

            var instance = emptyConstructor.Invoke(null);

            if (attribute.IsIgnored)
            {
                testResults[type].Add(new TestInfo(method.Name, attribute.IgnoreMessage));
                return;
            }

            foreach (var beforeTestMethod in methodsToTest[type].BeforeTestMethods)
            {
                ExecuteUtilityMethod(beforeTestMethod, instance);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                method.Invoke(instance, null);

                if (attribute.ExpectedException == null)
                {
                    isSuccessful = true;
                    stopwatch.Stop();
                }
            }
            catch (Exception testException)
            {
                thrownException = testException.InnerException.GetType();

                if (thrownException == attribute.ExpectedException)
                {
                    isSuccessful = true;
                    stopwatch.Stop();
                }
            }
            finally
            {
                stopwatch.Stop();
                var ellapsedTime = stopwatch.Elapsed;
                testResults[type].Add(new TestInfo(method.Name, isSuccessful, attribute.ExpectedException, thrownException, ellapsedTime));
            }

            foreach (var afterTestMethod in methodsToTest[type].AfterTestMethods)
            {
                ExecuteUtilityMethod(afterTestMethod, instance);
            }
        }

        /// <summary>
        /// Used for execution of methods that must be run before & after test methods.
        /// </summary>
        private static void ExecuteUtilityMethod(MethodInfo method, object instance)
            => method.Invoke(instance, null);

        /// <summary>
        /// Prints the results of the test.
        /// </summary>
        private static void PrintResults()
        {
            Console.WriteLine("<!> Testing result <!>");
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"Total test classes amount: {methodsToTest.Keys.Count}.");

            var totalTested = 0;

            foreach (var testedClass in methodsToTest.Keys)
            {
                totalTested += methodsToTest[testedClass].TestsCount;
            }

            Console.WriteLine($"Total test methods amount: {totalTested}.");

            foreach (var someClass in testResults.Keys)
            {
                Console.WriteLine("-----------------------------");
                Console.WriteLine($"Class: {someClass}.");

                var test = testResults;

                foreach (var testInfo in testResults[someClass])
                {
                    Console.WriteLine();
                    Console.WriteLine($"Tested method: {testInfo.MethodName}().");

                    if (testInfo.IsIgnored == true)
                    {
                        Console.WriteLine($"Ignored {testInfo.MethodName}() with message: {testInfo.IgnoranceReason}.");
                        Console.ResetColor();
                        continue;
                    }

                    if (testInfo.ExpectedException != null || testInfo.ActualException != null)
                    {
                        if (testInfo.ExpectedException == null)
                        {
                            Console.WriteLine($"Unexpected exception: {testInfo.ActualException}.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"Expected exception: {testInfo.ExpectedException}.");
                            Console.WriteLine($"Thrown exception: {testInfo.ActualException}.");

                            Console.ResetColor();
                        }
                    }

                    Console.WriteLine($"Ellapsed time: {testInfo.Time}.");

                    if (testInfo.IsSuccessful)
                    {
                        Console.WriteLine($"{testInfo.MethodName}() test has passed.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{testInfo.MethodName}() test has failed.");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}