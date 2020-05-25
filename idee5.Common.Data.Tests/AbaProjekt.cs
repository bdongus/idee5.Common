using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace idee5.Common.Data.Tests.Abacus {
    public class AbaProjekt : IEntity<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class AbaProjektQuery : IQuery<AbaProjektResult>
    {
        public string NameFilter { get; set; }
    }

    public class AbaProjektResult
    {
        public IList<AbaProjekt> Projekte { get; set; } = new List<AbaProjekt>();
    }

    public class AbaProjektInputHandler : IQueryHandlerAsync<AbaProjektQuery, AbaProjektResult>
    {
        public Task<AbaProjektResult> HandleAsync(AbaProjektQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return System.Threading.Tasks.Task.Run(() => {
                var result = new AbaProjektResult();
                var serializer = new XmlSerializer(typeof(AbaConnectContainer));
                using (var file = File.OpenText("ProjektePlantafel.xml")) {
                    var abacus = (AbaConnectContainer) serializer.Deserialize(file);
                    result.Projekte = abacus.Transaction.Where(t => t.Name.Contains(query.NameFilter)).Select(t => new AbaProjekt() { Id = long.Parse(t.Number), Name = t.Name }).ToList();
                }
                return result;
            });
        }
    }

    public class AbaProjektOutputHandler : ICommandHandlerAsync<AbaProjektResult>
    {
        public bool Executed { get; set; }

        public System.Threading.Tasks.Task HandleAsync(AbaProjektResult command, CancellationToken cancellationToken)
        {
            Executed = command.Projekte?.Any() == true;
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}