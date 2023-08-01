using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class ValidationTests {
        private readonly IRecursiveAnnotationsValidator _validator;

        public ValidationTests() {
            _validator = new RecursiveAnnotationsValidator();
        }

        [UnitTest, TestMethod]
        public async Task CanFailValidation() {
            // Arrange
            var cancellationToken = new CancellationToken();
            var command = new TestEntityConsoleOutput();
            var reporter = new ConsoleValidationReporter();

            var handler = new DataAnnotationValidationCommandHandlerAsync<TestEntityResult>(_validator, reporter, command);

            var testEntityResult = new TestEntityResult {
                Entities = new PagedCollection<TestEntity>(new List<TestEntity>() { new TestEntity(new DefaultTimeProvider(), new DefaultCurrentUserIdProvider()) }, 1)
            };

            //  Act
            await handler.HandleAsync(testEntityResult, cancellationToken).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(command.Executed);
            Assert.IsFalse(handler.Success);
            Assert.AreEqual(2, handler.ValidationResults.Count);
        }

        #region RecursiveAnnotationsValidator

        [UnitTest, TestMethod]
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public void TryValidateObject_on_valid_parent_returns_no_errors() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObject(parent, validationResults);

            Assert.IsTrue(result);
            Assert.IsTrue(validationResults.Count == 0);
        }

        [UnitTest, TestMethod]
        public void TryValidateObject_when_missing_required_properties_returns_errors() {
            var parent = new Parent { PropertyA = null, PropertyB = null };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObject(parent, validationResults);

            Assert.IsFalse(result);
            Assert.AreEqual(2, validationResults.Count);
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "Parent PropertyA is required"));
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "Parent PropertyB is required"));
        }

        [UnitTest, TestMethod]
        public void TryValidateObject_calls_IValidatableObject_method() {
            var parent = new Parent { PropertyA = 5, PropertyB = 6 };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObject(parent, validationResults);

            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count == 1);
            Assert.AreEqual("Parent PropertyA and PropertyB cannot add up to more than 10", validationResults[0].ErrorMessage);
        }

        [UnitTest, TestMethod]
        public void TryValidateObjectRecursive_returns_errors_when_child_class_has_invalid_properties() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            parent.Child = new Child { Parent = parent, PropertyA = null, PropertyB = 5 };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count == 1);
            Assert.AreEqual("Child PropertyA is required", validationResults[0].ErrorMessage);
        }

        [UnitTest, TestMethod]
        public void TryValidateObjectRecursive_ignored_errors_when_child_class_has_SkipRecursiveValidationProperty() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            parent.Child = new Child { Parent = parent, PropertyA = 1, PropertyB = 1 };
            parent.SkippedChild = new Child { PropertyA = null, PropertyB = 1 };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsTrue(result);
        }

        [UnitTest, TestMethod]
        public void TryValidateObjectRecursive_calls_IValidatableObject_method_on_child_class() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            parent.Child = new Child { Parent = parent, PropertyA = 5, PropertyB = 6 };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count == 1);
            Assert.AreEqual("Child PropertyA and PropertyB cannot add up to more than 10", validationResults[0].ErrorMessage);
        }

        [UnitTest, TestMethod]
        public void TryValidateObjectRecursive_returns_errors_when_grandchild_class_has_invalid_properties() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            parent.Child = new Child {
                Parent = parent,
                PropertyA = 1,
                PropertyB = 1,
                GrandChildren = new[] { new GrandChild { PropertyA = 11, PropertyB = 11 } }
            };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsFalse(result);
            Assert.AreEqual(2, validationResults.Count);
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyA not within range"));
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyB not within range"));
        }

        [UnitTest, TestMethod]
        public void TryValidateObject_calls_grandchild_IValidatableObject_method() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            parent.Child = new Child {
                Parent = parent,
                PropertyA = 1,
                PropertyB = 1,
                GrandChildren = new[] { new GrandChild { PropertyA = 5, PropertyB = 6 } }
            };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count == 1);
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyA and PropertyB cannot add up to more than 10"));
        }

        [UnitTest, TestMethod]
        public void TryValidateObject_includes_errors_from_all_objects() {
            var parent = new Parent { PropertyA = 5, PropertyB = 6 };
            parent.Child = new Child {
                Parent = parent,
                PropertyA = 5,
                PropertyB = 6,
                GrandChildren = new[] { new GrandChild { PropertyA = 5, PropertyB = 6 } }
            };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsFalse(result);
            Assert.AreEqual(3, validationResults.Count);
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "Parent PropertyA and PropertyB cannot add up to more than 10"));
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "Child PropertyA and PropertyB cannot add up to more than 10"));
            Assert.AreEqual(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyA and PropertyB cannot add up to more than 10"));
        }

        [UnitTest, TestMethod]
        public void TryValidateObject_modifies_membernames_for_nested_properties() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            parent.Child = new Child { Parent = parent, PropertyA = null, PropertyB = 5 };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(parent, validationResults);

            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count == 1);
            Assert.AreEqual("Child PropertyA is required", validationResults[0].ErrorMessage);
            Assert.AreEqual("Child.PropertyA", validationResults[0].MemberNames.First());
        }

        [UnitTest, TestMethod]
        public void TryValidateObject_object_with_dictionary_does_not_fail() {
            var parent = new Parent { PropertyA = 1, PropertyB = 1 };
            var classWithDictionary = new ClassWithDictionary {
                Objects = new List<Dictionary<string, Child>>
                {
                    new Dictionary<string, Child>
                    {
                        { "key",
                            new Child
                            {
                                Parent = parent,
                                PropertyA = 1,
                                PropertyB = 2
                            }
                        }
                    }
                }
            };
            var validationResults = new List<ValidationResult>();

            var result = _validator.TryValidateObjectRecursive(classWithDictionary, validationResults);

            Assert.IsTrue(result);
            Assert.IsTrue(validationResults.Count == 0);
        }
#pragma warning restore CA1707 // Identifiers should not contain underscores

        #endregion RecursiveAnnotationsValidator
    }
}