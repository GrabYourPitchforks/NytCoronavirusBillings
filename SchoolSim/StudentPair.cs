using System;
using System.Diagnostics;

namespace SchoolSim
{
    [DebuggerDisplay("{_studentId1} : {_studentId2}")]
    public readonly struct StudentPair : IEquatable<StudentPair>
    {
        private readonly int _studentId1;
        private readonly int _studentId2;

        public StudentPair(int studentId1, int studentId2)
        {
            if (studentId1 == studentId2)
            {
                throw new ArgumentException("Cannot use the same id for both students.");
            }

            _studentId1 = Math.Min(studentId1, studentId2);
            _studentId2 = Math.Max(studentId1, studentId2);
        }

        public override bool Equals(object obj)
            => obj is StudentPair other && Equals(other);

        public bool Equals(StudentPair other)
            => this._studentId1 == other._studentId1 && this._studentId2 == other._studentId2;

        public override int GetHashCode() => (_studentId1, _studentId2).GetHashCode();

        public int[] GetStudentIds() => new[] { _studentId1, _studentId2 };
    }
}
