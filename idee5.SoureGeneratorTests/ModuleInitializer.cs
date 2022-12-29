﻿using System.Runtime.CompilerServices;
using VerifyTests;

namespace idee5.SoureGeneratorTests {
    internal static class ModuleInitializer {
        [ModuleInitializer]
        public static void Init() {
            VerifySourceGenerators.Enable();
        }
    }
}
