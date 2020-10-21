using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SchoolSim
{
    public static class ClassroomSimulator
    {
        // Change these values to update the simulation parameters.

        // Simulate a 24-student classroom with seats arranged in a 6 x 4 grid.
        private const int RowCount = 6;
        private const int ColumnCount = 4;

        // The total number of iterations to run, and the time spent in each iteration.
        // 4 iterations * 15 minutes per iteration = 60 minutes total per simulation,
        // with a shuffle occurring between each iteration.
        private const int IterationCount = 4;
        private readonly static TimeSpan IterationTime = TimeSpan.FromMinutes(15);

        // The exposure factor between immediately adjacent (up-down / left-right)
        // seats or diagonal seats. If each iteration is 15 minutes, then the default
        // exposure factors mean that adjacent seats record 15 minutes equivalent
        // exposure per iteration, and diagonal seats record 7.5 minutes equivalent
        // exposure per iteration. No exposure beyond one seat in any direction.
        // Basically, assume that only adjacent seats are within 6 ft. of each other.
        private const double AdjacentExposureFactor = 1.0;
        private const double DiagonalExposureFactor = 0.5;

        // The total exposure required before crossing the "risky" threshold.
        // CDC suggests 15 minutes, but for this simulation we'll use 20 minutes.
        private readonly static TimeSpan ExposureRiskThreshold = TimeSpan.FromMinutes(20);

        /// <summary>
        /// Runs the simulation, returning the number of students (not student pairs)
        /// whose total exposure to any other single student exceeds the specified
        /// CDC risk leel.
        /// </summary>
        public static int RunSimulationForAllStudents(out bool wasTrackedStudentOverexposed)
        {
            // Run the simulation (several iterations)
            TimekeepingDictionary<StudentPair> tkDict = new TimekeepingDictionary<StudentPair>();
            for (int i = 0; i < IterationCount; i++)
            {
                RunSingleIteration(tkDict);
            }

            // Now count the number of students who exceeded risky exposure level
            HashSet<int> idsOfStudentsAtRisk = tkDict
                .Where(entry => entry.Value >= ExposureRiskThreshold)
                .SelectMany(entry => entry.Key.GetStudentIds())
                .ToHashSet();

            // Assuming we're tracking a single student (say, student id #1),
            // out a value stating whether this particular student was overexposed
            // to any other student.
            wasTrackedStudentOverexposed = idsOfStudentsAtRisk.Contains(1);
            return idsOfStudentsAtRisk.Count;
        }

        /// <summary>
        /// Runs the simulation, returning the number of students who were over-exposed
        /// to a particular student (for simplicity, student id #1).
        /// </summary>
        public static int RunSimulationForOneStudent()
        {
            // Run the simulation (several iterations)
            TimekeepingDictionary<StudentPair> tkDict = new TimekeepingDictionary<StudentPair>();
            for (int i = 0; i < IterationCount; i++)
            {
                RunSingleIteration(tkDict);
            }

            // Find each pair who exceeded "risky" exposure level, and count the
            // number of pairs which involve student #1.

            return tkDict
                .Where(entry => entry.Value >= ExposureRiskThreshold)
                .Where(entry => entry.Key.GetStudentIds().Contains(1))
                .Select(entry => entry.Key)
                .Distinct()
                .Count();
        }

        private static void RunSingleIteration(TimekeepingDictionary<StudentPair> tkDict)
        {
            // Generate a list of all students
            List<int> allStudentIds = Enumerable.Range(1, checked(RowCount * ColumnCount)).ToList();

            // Randomly place them throughout the classroom
            int[,] studentPlacements = new int[RowCount, ColumnCount];
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    studentPlacements[i, j] = allStudentIds.ChooseAndRemoveRandom();
                }
            }

            Debug.Assert(allStudentIds.Count == 0, "Should've placed all students.");

            // Now calculate exposure levels

            TimeSpan adjacentExposure = TimeSpan.FromTicks((long)(IterationTime.Ticks * AdjacentExposureFactor));
            TimeSpan diagonalExposure = TimeSpan.FromTicks((long)(IterationTime.Ticks * DiagonalExposureFactor));

            // Calculate exposure left-right

            for (int i = 0; i < RowCount - 1; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    tkDict.AddTime(new StudentPair(studentPlacements[i, j], studentPlacements[i + 1, j]), adjacentExposure);
                }
            }

            // Calculate exposure up-down

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount - 1; j++)
                {
                    tkDict.AddTime(new StudentPair(studentPlacements[i, j], studentPlacements[i, j + 1]), adjacentExposure);
                }
            }

            // Calculate exposure diagonally

            for (int i = 0; i < RowCount - 1; i++)
            {
                for (int j = 0; j < ColumnCount - 1; j++)
                {
                    tkDict.AddTime(new StudentPair(studentPlacements[i, j], studentPlacements[i + 1, j + 1]), diagonalExposure);
                    tkDict.AddTime(new StudentPair(studentPlacements[i + 1, j], studentPlacements[i, j + 1]), diagonalExposure);
                }
            }
        }
    }
}
