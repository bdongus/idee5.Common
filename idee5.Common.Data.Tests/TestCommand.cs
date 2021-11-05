using System;

namespace idee5.Common.Data.Tests {
    internal class TestCommand {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsSomething { get; set; }
        public float FloatNum { get; set; }
        public double DoubleNum { get; set; }
        public decimal DecimalNum { get; set; }
        public Guid UUID { get; set; }
    }
}