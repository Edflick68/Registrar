using DAL;
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
    }
}
