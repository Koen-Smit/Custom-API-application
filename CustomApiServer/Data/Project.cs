using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApiServer.Data
{
    internal class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public IList<User> Users { get; set; }
    }

    /// <summary>
    /// In dit geval is de DTO bijna identiek aan het model, maar in de praktijk is dit vaak anders.
    /// Toch is het handig om nu een DTO aan te maken, dan sturen we nooit per ongeluk te veel data
    /// terug. Bijvoorbeeld als een collega ineens een geheim veld toevoegd aan het Project model.
    /// </summary>
    internal class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IList<UserDto> Users { get; set; }
    }
}
