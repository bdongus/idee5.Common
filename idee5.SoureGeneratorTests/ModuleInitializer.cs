using System.Runtime.CompilerServices;

namespace idee5.SoureGeneratorTests {
    internal static class ModuleInitializer {
        [ModuleInitializer]
        public static void Init() {
            VerifySourceGenerators.Enable();
        }
    }
}
