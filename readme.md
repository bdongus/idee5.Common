# Readme.md
## Release Notes idee5.Common
### 2.0.0
* Converted to PCL base library.
### 2.0.6
* Added the IRange interface for checking ranges.
* Move HashCodeCombiner to PCL
  * File hashes neded to be removed due to lacking PCL APIs.
### 2.0.7
* Resolved nuget targeting issues. (portable50 was targeted, not netstandard1.2)
### 2.0.8
* Added intersection check to ranges interface
For package creation use: msbuild /t:pack /p:Configuration=Release on every package.
### 2.0.9
* Added the generic ICopyable interface for deep copies
### 3.0.0
Changed to netstandard20. Made IQuery contra-variant to support "inheritance".
### 3.0.2
Added cancellation token to the async query and command interfaces.
### 3.0.4
Added validation of async command handlers. Renamed the validation decorator.
### 3.0.5
Added .net 4.6.1 tests to cover differences to .net core. IsClrType now covers .net core too.
### 3.0.15
Added the Minutes() and Seconds() TimeSpan extensions
### 3.1.0
Added the ExpandoObject extensions AddProperty and GetPropertyByName.
### 3.2.0
* Added enum extensions, enum comparer, CastTo, ... 
* Added DateTimeOffset to ITimeProvider.
* Added the object.In() extension
### 3.3.1
3.3.0 wasn't possible. Automatic Versions just skipped this.
* Removed the event handler extensions. They are no longer useful. "?." is more concise.
* Replaced object.In by a generic verision (better performance)
### 3.3.3
* Removed redundant IAsyncQueryHandler.
* Added default cancellation tokens to the query and command handlers.
### 3.3.4
* Added SafeFireAndForget as task extension
### 3.3.5
Still 3.x because no offical 3.x version ist released. The version increments are my fault, misusing AutomaticVerions.
* DelegateEqualityComparer and Enumerable.DistinctBy are never used, obsolete and replaced by Enumerable.Distinct.
* Fixed several FxCop warnings
  * Renamed the enum Helper class from Enum To EnumUtils.
  * Tons of NULL checks.
  * Corrected IDisposable handling.
  * Renaming to fullfill naming conventions.
  * Made a bunch of string operations culture aware.
  * Lots of additional documentation
  * Removed ToMD5 due to it being broken. Look for other ways.
* Removed some used prematurely added stuff.
### 3.3.6
Removed the reference to FxCop analyzers.
### 3.3.7
Removed the validated command handler, in favor for the one in idee5.Common.Data.
Moved localizable strings to internal resource file.
### 3.3.9
* Added a "nearly equals" comparison for floats and doubles.
* Documentation updates
### 3.3.10
* Added the Chunk enumerable extension method
* removed the unused "tmp" variable in StringExtension.ToHex
* Code reformatting
### 3.3.11
* Added "as" to the object extensions
* Added "IsTypeOf" to the object extensions
### 3.3.12
* Added "AsString" to the Object extensions.
### 3.3.13
* Added DateTimeRange
### 3.3.14
* Added DateTimeRange equality comparability
### 3.3.15
* Added DateTimeRange equality operators
* Fixed a Base64 issue
### 3.4.0
* Source generator attribute added
* Currency query added
### 3.4.1
* MIT license
### 3.4.2
* Ambient providers marked as obsolete to give a compiler hint on maybe wrong usage outside test scenarios.
### 3.5
* Added ProblemDetailsWithErrors
### 3.6
* Detect interface implementations added. Used to find command and query handlers.
## Release notes idee5.Common.Data
### 1.0.0
Initial release
### 1.0.1
Removed the unit of work stuff.
### 1.0.5
Added recursive data annotations and data conversion support.
### 1.0.6
Added soft delete repositories, renamed IMonitoredEntity to IAuditedEntity, readed .NET 4.6.1 package.
### 1.1.1
Write actions in repositories are now synchronous. The IUnitofWork is the async part.
### 1.1.2
Use Func<IQueryable<T>, IQueryable<T>> instead on Func<IQueryable<T>, IEnumerable<T>> to make real life easier. Not unit testing.
### 1.1.3
Write actions in repositories are now synchronous. The IUnitOfWork is the async part.
### 1.1.4
Added support for the DataDirectory placeholder.
### 1.1.5
Added the IConnectionStringProvider.
### 1.1.6
Added bulk operations to the repository interface.
### 1.2.0
Adjusted the repository interface to the real world asynchrony.
### 1.3.0
Removed the specifications. The NSpecification package now supports netstandard.
### 1.4.0
IRepository now needs a TKey type parameter to handle UpdateOrAdd in ARepository.
### 1.4.1
Added ICompositeKeyRepository to handle cases like idee5.Globalization.
### 1.4.2
Just to have the new idee5.Common 3.2.5 reference.
### 1.4.4
Added base implementation for ACompositeKeyRepository.UpdateOrAddAsync to handle a list of items.
### 1.4.5
Corrected ReplaceDataDirectory.
### 1.4.6
* Fixed several FxCop warnings
  * Tons of NULL checks.
  * Corrected IDisposable handling.
  * Renaming to fullfill naming conventions.
  * Lots of additional documentation
  * Fixed some memory allocations
  * ...
### 1.4.7
Removed the reference to FxCop analyzers.
### 1.4.8
Do a for loop in the UpdateOrAddAsync method of the abstract repositories. Most ORMs don't support parallel execution on the same context.
### 1.4.9
* Added validation result reporter
### 1.4.10 
* Added localization resource.
* Added abstract data converter for multiple used input or output handlers.
### 1.4.11
* DataConverterAsync: Support for different input handlers with the same output handler
* Fixed AudtitingRepository's UpdateOrAddAsync.
### 1.4.12
* ExportToCsv: Escape the quotation mark
### 1.4.13
* Fixed an issue in ARepository.UpdateOrAddAsync
### 1.4.14
* Added the AllowedValues attribute
### 1.4.15
* Added LogValidationReporter
* Fixed the namespace of DebugValidationReporter
### 1.4.16
* Added LoggingCommandHandler, LogValidationReporter
* Minor improvements
### 1.4.17
* package updates, fixed some typos and added more documentation
### 1.5
* AbstractEventstore added
### 1.6
* Enabled NULLABLE
### 1.6.1
Nullable support in IAuditedEntity
### 1.7
* MIT license
* Added RegisterHandlers to add query and command handlers to the DI container
### 1.7.1
* event store test now uses a record type instead of a class
* Added the MasterSystemReference type.
### 1.8
* Configurable master system formatter