using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace idee5.Common.Data {
    /// <summary>
    /// The abstract event store.
    /// </summary>
    /// <typeparam name="TEvent">Base event type/interface</typeparam>
    public abstract class AbstractEventStore<TEvent> {
        protected readonly Dictionary<Type, Delegate> _registeredTypes = new();

        protected object _locker = new object();

        /// <summary>
        /// Create event object from the stored event entry. an
        /// </summary>
        /// <param name="eventEntry"></param>
        /// <returns>The event created from the event store</returns>
        protected TEvent CreateEvent(EventEntry eventEntry) {
            var eventType = ReflectionUtils.GetTypeFromName(eventEntry.EventName);
            var eventProperyTypes = eventType.GetProperties().Select(ep => ep.PropertyType);
            var cInfo = eventType.GetConstructors().First();
            ParameterInfo[] paramsInfo = cInfo.GetParameters();
            // create the event constructor
            Delegate ctor;
            lock (_locker) {
                // check the cache
                if (!_registeredTypes.TryGetValue(eventType, out ctor)) {
                    var argsExp = new Expression[paramsInfo.Length];
                    //create a single param of type object[] for the lambda expression
                    ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

                    for (int i = 0; i < paramsInfo.Length; i++) {
                        Expression index = Expression.Constant(i);
                        Type paramType = paramsInfo[i].ParameterType;
                        BinaryExpression paramAccessorExp = Expression.ArrayIndex(param, index);
                        argsExp[i] = Expression.Convert(paramAccessorExp, paramType);
                    }

                    NewExpression body = Expression.New(cInfo, argsExp);
                    //create a lambda with the New
                    //Expression as body and our param object[] as arg
                    var expr = Expression.Lambda(body, param);
                    ctor = expr.Compile();
                    _registeredTypes.Add(eventType, ctor);
                }
            }
            // create the argument values
            Dictionary<string, JsonElement> dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(eventEntry.Data);
            object[] ps = new object[paramsInfo.Length];
            for (int i = 0; i < paramsInfo.Length; i++) {
                ParameterInfo item = paramsInfo[i];
                Type paramType = item.ParameterType;
                string paramName = item.Name;
                JsonElement jsonElement = dict[paramName.CamelToPascalCase()];
                object p = null;
                if (jsonElement.ValueKind != JsonValueKind.Null) {
                    if (paramType == typeof(object)) {
                        p = jsonElement.GetRawText();
                    }
                    else if (paramType == typeof(DateTimeRange)) {
                        p = jsonElement.Deserialize(paramType);
                    }
                    else {
                        p = TypeDescriptor.GetConverter(paramType).ConvertFromInvariantString(jsonElement.ToString());
                    }
                }
                ps[i] = p;
            }
            // create the event
            TEvent result = (TEvent)ctor.DynamicInvoke(new object[] { ps });
            AdditionalMappings(ref result, dict);
            return result;
        }
        /// <summary>
        /// Map properties not included in the event constructor. Eg. the timestamp or version
        /// </summary>
        /// <param name="ev">The newly created event.</param>
        /// <param name="dict"><The dictionary of JSON elements/param>
        protected abstract void AdditionalMappings(ref TEvent ev, Dictionary<string, JsonElement> dict);
    }
    /// <summary>
    /// Event store row definition
    /// </summary>
    public class EventEntry {
        /// <summary>
        /// Clustering index.
        /// </summary>
        [Key]
        public int Index { get; set; }

        /// <summary>
        /// Id of the aggregate root.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Version of the aggregate root
        /// </summary>
        [Required]
        public int Version { get; set; }

        /// <summary>
        /// Serialized event data.
        /// </summary>
        [MaxLength]
        public string Data { get; set; }

        /// <summary>
        /// Instant the event was created.
        /// </summary>
        [Required]
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Name of the event
        /// </summary>
        [Required]
        public string EventName { get; set; }
    }

}
