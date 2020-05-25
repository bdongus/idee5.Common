namespace idee5.Common.Tests {
    public class TestCommand {
        public bool Execute { get; set; }

        public int Counter { get; set; } // return value! In real apps normally commands DON'T return anything!
    }
}