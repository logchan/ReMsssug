using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            var regex = new Regex(@"<TD WIDTH='18%'[^>]*>(?<code>[^<]+)<\/TD>[^>]*>(?<name>[^<]+)", RegexOptions.Singleline);

            foreach (var subject in Subjects)
            {
                var url = $"http://publish.ust.hk/SISCourseCat/ShowUGCourseList.aspx?Subject={subject}&WebSite=Production";
                using (var client = new HttpClient())
                {
                    var page = client.GetStringAsync(url).Result;
                    var match = regex.Match(page);
                    while (match.Success)
                    {
                        list.Add(new Tuple<string, string>(match.Groups["code"].Value.Replace(" ", ""),
                            match.Groups["name"].Value));

                        match = match.NextMatch();
                    }
                }
            }
            return list;
        }

        public static void UpdateCourses(RmContext context)
        {
            var courses = GetCourses();
            foreach (var pair in courses)
            {
                var course = context.Courses.FirstOrDefault(c => c.Code == pair.Item1);
                if (course == null)
                {
                    context.Courses.Add(new Course
                    {
                        Code = pair.Item1,
                        Name = pair.Item2
                    });
                }
                else if (course.Name != pair.Item2)
                {
                    course.Name = pair.Item2;
                }
            }
            context.SaveChanges();
        }
    }
}
