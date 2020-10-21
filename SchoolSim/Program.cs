using System;
using System.Linq;

namespace SchoolSim
{
    class Program
    {
        static void Main(string[] args)
        {
            // Run the simulation 50,000 times.
            const int BATT_COUNT = 50_000;

            Console.WriteLine("Running simulation...");

            int trackedStudentExposureCount = 0;
            CountingDictionary<int> histogram = new CountingDictionary<int>();
            for (int i = 0; i < BATT_COUNT; i++)
            {
                int overexposedStudentCount = ClassroomSimulator.RunSimulationForAllStudents(out bool wasTrackedStudentOverexposed);
                histogram.AddCount(overexposedStudentCount, 1);

                if (wasTrackedStudentOverexposed)
                {
                    trackedStudentExposureCount++;
                }
            }

            Console.WriteLine("Finished.");
            Console.WriteLine();
            Console.WriteLine("Results:");
            foreach (var entry in histogram.OrderBy(entry => entry.Key))
            {
                // Prints results in the format "key: value",
                // where 'value' is the number of times 'key' appears in the result set.
                Console.WriteLine(FormattableString.Invariant($"{entry.Key}: {entry.Value}"));
            }

            Console.WriteLine();
            Console.WriteLine(FormattableString.Invariant($"Tracked student was exposed {(double)trackedStudentExposureCount / BATT_COUNT * 100:F2}% of the time."));
        }
    }
}
