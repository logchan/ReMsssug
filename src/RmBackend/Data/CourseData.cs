using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RmBackend.Models;

namespace RmBackend.Data
{
    public static class CourseData
    {
        private static readonly string[] Subjects = 
        {
            "ACCT", "IELM", "BIBU", "IIMP", "BIEN", "IROP", "BIPH", "ISOM", "BMED", "LABU", "CENG", "LAGR", "CHEM",
            "LANG", "CIVL", "LIFS", "COMP", "MARK", "CPEG", "MATH", "ECON", "MECH", "ELEC", "MGMT", "ENGG", "PHYS",
            "ENTR", "RMBI", "ENVR", "SBMT", "ENVS", "SCIE", "FINA", "SHSS", "FYTG", "SISP", "GBUS", "SOSC", "GNED",
            "SUST", "HART", "TEMG", "HLTH", "UROP", "HUMA", "WBBA", "IDPO"
        };

        private static List<Tuple<string, string>> GetCourses()
        {
            var list = new List<Tuple<string, string>>();

            foreach (var subject in Subjects)
            {
                // TODO: download and parse courses
            }
            return list;
        }

        public static void UpdateCourses(RmContext context)
        {
            // TODO: update courses
        }
    }
}
