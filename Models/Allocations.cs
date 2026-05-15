using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wikimedia.Models
{
    public class Allocations
    {
            public int Id { get; set; }
            public int TeacherId { get; set; }
            public int CourseId { get; set; }
            public int Year { get; set; }
    
            public Teacher Teacher => DB.Teachers.ToList().FirstOrDefault(t => t.Id == TeacherId);
            public Course Course => DB.Courses.ToList().FirstOrDefault(c => c.Id == CourseId);

        [JsonIgnore]
        public bool IsNextSession
        {
            get
            {
                if (Course?.Session == null) return false;

                int sessionNumber = (Course.Session.Contains("Automne") || Course.Session == "A") ? 1 : 2;
                return Year == NextSession.Year && NextSession.ValidSessions.Contains(sessionNumber);
            }
        }
    }
}
