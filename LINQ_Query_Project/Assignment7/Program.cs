using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Assignment7
{
    class Program
    {
        static void Main(string[] args)
        {
            //These strings need to be changed to match the correct file path if the applications is ran on a different computer.
            string coursesFilePath = "C:/Users/CODpo/OneDrive/Documents/CSE446/Assignment7/Assignment7/Assignment7/App_Data/Courses.csv";
            string instructorsFilePath = "C:/Users/CODpo/OneDrive/Documents/CSE446/Assignment7/Assignment7/Assignment7/App_Data/Instructors.csv";
            string coursesXMLPath = "C:/Users/CODpo/OneDrive/Documents/CSE446/Assignment7/Assignment7/Assignment7/App_Data/coursesXML.xml";

            //---------------------------------------------------
            //---------------Question 1.1------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 1.1----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");

            //Course Array
            Course[] courses = new Course[212];

            //Read the Courses.csv into an array
            StreamReader reader = null;
            if (File.Exists(coursesFilePath))
            {
                reader = new StreamReader(File.OpenRead(coursesFilePath));
                List<string> listOfStrings = new List<string>();
                int indexCount = -1; //Used to add courses to the course array
                while (!reader.EndOfStream)
                {
                    string subject = null;
                    string code = null;
                    string title = null;
                    string courseID = null;
                    string location = null;
                    string instructor = null;

                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    subject = values[0];
                    code = values[1];
                    title = values[2];
                    courseID = values[3];
                    instructor = values[4];
                    location = values[8];

                    if (indexCount >= 0)
                    {
                        Course newCourse = new Course(subject, code, title, courseID, instructor, location);
                        courses[indexCount] = newCourse;
                    }
                    indexCount++;
                }
            }
            else
            {
                Console.WriteLine("Courses.csv can not be found");
            }

            Console.WriteLine("Uncomment for loop to see all courses in the array.");
            //print all the courses
            /*
            for(int x = 0; x < courses.Length; x++)
            {
                courses[x].printCourse();
            }
            */

            //---------------------------------------------------
            //---------------Question 1.2a------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 1.2a----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");
            IEnumerable<Course> question1_2a =
                from c in courses
                where c.Subject == "IEE" && (Int32.Parse(c.Code) >= 300)
                orderby c.Instructor
                select c;

            foreach (Course item in question1_2a)
            {
                Console.WriteLine("Title: " + item.Title + ", Instructor: " + item.Instructor);
            }

            //---------------------------------------------------
            //---------------Question 1.2b------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 1.2b----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");

            var groups = courses.GroupBy(c => new { c.Subject, c.Code })
                                .GroupBy(g => g.Key.Subject)
                                .Where(g => g.Count() >= 2);
            foreach (var subject in groups)
            {
                Console.WriteLine(subject.Key);
                foreach (var codeSubGroup in subject)
                {
                    int writeCount = 0;
                    if (codeSubGroup.Count() >= 2)
                    {
                        if (writeCount == 0)
                        {
                            Console.WriteLine("    " + codeSubGroup.Key.Code);
                            foreach (var item in codeSubGroup)
                            {
                                Console.WriteLine("       " + item.Title);
                            }
                        }
                    }
                    writeCount++;
                }
            }



            //---------------------------------------------------
            //---------------Question 1.4------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 1.4----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");

            //Instructor List
            List<Instructor> instructors = new List<Instructor>();

            //Read the Instructors.csv into a list
            StreamReader reader2 = null;
            if (File.Exists(instructorsFilePath))
            {
                reader2 = new StreamReader(File.OpenRead(instructorsFilePath));
                List<string> listOfStrings = new List<string>();
                int indexCount = -1; //Used to add courses to the course array
                while (!reader2.EndOfStream)
                {
                    string name = null;
                    string officeNum = null;
                    string email = null;

                    var line = reader2.ReadLine();
                    var values = line.Split(',');

                    name = values[0];
                    officeNum = values[1];
                    email = values[2];

                    if (indexCount >= 0)
                    {
                        Instructor newInstructor = new Instructor(name, officeNum, email);
                        instructors.Add(newInstructor);
                    }
                    indexCount++;
                }
            }
            else
            {
                Console.WriteLine("Instructors.csv can not be found");
            }

            Console.WriteLine("Uncomment foreach loop to see all instructors in the list.");
            /*
            //print all the instuctors
            foreach (Instructor item in instructors)
            {
                item.printInstructor();
            }
            */


            //---------------------------------------------------
            //---------------Question 1.5------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 1.5----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");

            var question1_5 =
                from eachcourse in courses
                join instructor in instructors on eachcourse.Instructor equals instructor.Name into Joined
                from JoinedData in Joined.DefaultIfEmpty()
                where (Int32.Parse(eachcourse.Code) < 300) && (Int32.Parse(eachcourse.Code) >= 200)
                orderby eachcourse.Code
                select new
                {
                    CourseSubject = eachcourse.Subject,
                    CourseCode = eachcourse.Code,
                    InstructorEmail = JoinedData != null ? JoinedData.EmailAddress ?? "" : ""
                };

            foreach (var item in question1_5)
            {
                Console.WriteLine(item.CourseSubject + " " + item.CourseCode + ", " + item.InstructorEmail);
            }



            //---------------------------------------------------
            //---------------Question 2.1a------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 2.1a----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");

            XDocument coursesXML = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("courses"));

            //Make an XML structure for courses
            foreach (Course item in courses)
            {
                XElement newCourse = new XElement("course",
                    new XElement("subject", item.Subject),
                    new XElement("code", item.Code),
                    new XElement("title", item.Title),
                    new XElement("courseID", item.CourseID),
                    new XElement("instructor", item.Instructor),
                    new XElement("location", item.Location));

                coursesXML.Element("courses").Add(newCourse);
            }
            try
            {
                coursesXML.Save(coursesXMLPath);
            }
            catch
            {
                Console.WriteLine("The path used to save the courseXML.xml file is incorrect");
            }

            var question2_1a =
                from XMLcourses in coursesXML.Descendants("course")
                where (string)XMLcourses.Element("subject") == "CPI" && (int)XMLcourses.Element("code") >= 200
                orderby (string)XMLcourses.Element("instructor") ascending
                select new XElement("CPI-course",
                       new XElement("title", (string)XMLcourses.Element("title")),
                       new XElement("instructor", (string)XMLcourses.Element("instructor")));

            foreach (XElement item in question2_1a)
            {
                Console.WriteLine(item);
            }




            //---------------------------------------------------
            //---------------Question 2.1b------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 2.1b----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");
            var question2_1b =
                from XMLcourse in coursesXML.Descendants("course")
                group XMLcourse by new
                {
                    subject = (string)XMLcourse.Element("subject"),
                    code = (int)XMLcourse.Element("code")
                } into subjectGrouping
                orderby subjectGrouping.Key.subject, subjectGrouping.Key.code
                select new
                {
                    Subject = subjectGrouping.Key.subject,
                    Code = subjectGrouping.Key.code,
                    Courses = subjectGrouping.Select(c => (string)c.Element("title")).ToList()
                };

            foreach (var group in question2_1b)
            {
                int writeCount = 0;
                int codeWriteCount = 0;
                
                //Console.WriteLine(group.Subject);
                foreach(string title in group.Courses)
                {

                    if (group.Courses.Count() >= 2)
                    {
                        if (writeCount == 0)
                        {
                            Console.WriteLine(group.Subject);
                            writeCount++;
                        }
                        if(codeWriteCount == 0)
                        {
                            Console.WriteLine("    " + group.Code);
                            codeWriteCount++;
                        }
                        Console.WriteLine($"        {title}");
                        writeCount++;
                    }
                    
                }
            }

            //---------------------------------------------------
            //---------------Question 2.2------------------------
            //---------------------------------------------------
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("-------------------------Question 2.2----------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------");
            var question2_2 = new XElement("CoursesWithEmails",
                from course in courses
                join instructor in instructors on course.Instructor equals instructor.Name into gj
                orderby course.Code
                where (Int32.Parse(course.Code) >= 200) && (Int32.Parse(course.Code) <= 299)
                select new XElement("Course",
                    new XAttribute("Subject", course.Subject),
                    new XAttribute("Code", course.Code),
                    from subset in gj
                    select new XElement("Instructor-Email", subset.EmailAddress)));
            Console.WriteLine(question2_2);
      
        }
    }


    public class Course
    {
        //Object variables
        private string subject;
        private string code;
        private string title;
        private string courseID;
        private string instructor;
        private string location;
        public Course(string subject, string code, string title, string courseID, string instructor, string location)
        {
            this.subject = subject;
            this.code = code;
            this.title = title;
            this.courseID = courseID;
            this.instructor = instructor;
            this.location = location;
        }

        //Get and Set methods
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string CourseID
        {
            get { return courseID; }
            set { courseID = value; }
        }

        public string Instructor
        {
            get { return instructor; }
            set { instructor = value; }
        }

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        // Print method
        public void printCourse()
        {
            Console.WriteLine(subject + "," + code + "," + title + "," + courseID + "," + instructor + "," + location);
        }
    }


        public class Instructor
        {
            //Object variables
            private string name;
            private string officeNumber;
            private string emailAddress;

            public Instructor(string Name, string OfficeNumber, string EmailAddress)
            {
                this.name = Name;
                this.officeNumber = OfficeNumber;
                this.emailAddress = EmailAddress;
            }

            //Get and Set methods
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string OfficeNumber
            {
                get { return officeNumber; }
                set { officeNumber = value; }
            }

            public string EmailAddress
            {
                get { return emailAddress; }
                set { emailAddress = value; }
            }

            // Print method
            public void printInstructor()
            {
                Console.WriteLine(name + "," + officeNumber + "," + emailAddress);
            }
        }
}
