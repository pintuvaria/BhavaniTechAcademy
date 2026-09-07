using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public class UniversityCurriculumService
    {
        public record DegreeModule(string DegreeType, string Subject, string Content);
        public record DegreeExamQuestion(string DegreeType, string Question, string[] Options, int CorrectIndex);
        
        public List<DegreeModule> GetModules()
        {
            return new List<DegreeModule>
            {
                // BCA Modules
                new DegreeModule("BCA", "Programming in C", "C is a foundational procedural programming language. Key concepts include pointers (direct memory address manipulation), structs (custom composite data types), and dynamic memory allocation using malloc() and free()."),
                new DegreeModule("BCA", "Data Structures", "Core data structures define how data is stored. Arrays (contiguous memory), Linked Lists (node-based sequential pointers), Stacks (LIFO - Last In First Out), and Queues (FIFO - First In First Out) are fundamental."),
                new DegreeModule("BCA", "Database Management Systems (DBMS)", "DBMS revolves around the Relational Data Model. Normalization rules (1NF, 2NF, 3NF) reduce data redundancy. SQL commands are categorized into DDL (Create, Alter), DML (Insert, Update), and DCL (Grant, Revoke)."),
                new DegreeModule("BCA", "Web Technologies", "Client-side web development uses HTML for structural semantics, CSS for visual styling, and JavaScript for client-side interactivity. The DOM (Document Object Model) API allows JavaScript to dynamically update page content."),
                
                // MCA Modules
                new DegreeModule("MCA", "Advanced Software Engineering", "Agile methodologies (Scrum, Kanban) focus on fast, iterative development cycles. Software Design Patterns (Creational, Structural, Behavioral) provide reusable, scalable solutions to common object-oriented design problems."),
                new DegreeModule("MCA", "Artificial Intelligence & Expert Systems", "AI encompasses Machine Learning (Supervised vs Unsupervised) and Deep Neural Networks. Expert Systems utilize complex inference engines querying against a domain-specific knowledge base. Search algorithms like A* and Minimax are critical for AI agents."),
                new DegreeModule("MCA", "Cloud Computing Architectures", "Enterprise cloud service models include IaaS (Infrastructure as a Service), PaaS (Platform), and SaaS (Software). Virtualization hypervisors and OS-level containerization (Docker, Kubernetes) are the backbone of modern cloud deployments."),
                new DegreeModule("MCA", "Advanced Database & Big Data", "NoSQL databases (Document, Key-Value, Graph) handle massive scale and unstructured data. Big Data processing frameworks like Apache Hadoop (HDFS & MapReduce) process petabytes of data across distributed commodity hardware clusters.")
            };
        }

        public List<DegreeExamQuestion> GetExamQuestions()
        {
            return new List<DegreeExamQuestion>
            {
                // BCA Exam
                new DegreeExamQuestion("BCA", "In C programming, what function is used to allocate memory dynamically?", new[] { "malloc()", "alloc()", "memget()", "assign()" }, 0),
                new DegreeExamQuestion("BCA", "Which data structure rigidly follows the Last-In-First-Out (LIFO) principle?", new[] { "Queue", "Linked List", "Tree", "Stack" }, 3),
                new DegreeExamQuestion("BCA", "What does 1NF (First Normal Form) strictly require in a database table?", new[] { "No transitive dependencies", "Atomic (indivisible) values", "Foreign keys in every table", "No partial dependencies" }, 1),
                
                // MCA Exam
                new DegreeExamQuestion("MCA", "In Software Engineering, which design pattern guarantees a class has only one instance?", new[] { "Factory", "Observer", "Singleton", "Decorator" }, 2),
                new DegreeExamQuestion("MCA", "Which Cloud Computing model provides a fully managed, complete software application to the end-user?", new[] { "IaaS", "PaaS", "SaaS", "DaaS" }, 2),
                new DegreeExamQuestion("MCA", "In Big Data, what are the two core fundamental components of the Hadoop framework?", new[] { "Spark and Kafka", "HDFS and MapReduce", "Cassandra and Hive", "SQL and NoSQL" }, 1)
            };
        }

        public record DegreeGradeResult(string DegreeType, int Score, int TotalQuestions, bool Passed, double Percentage);

        public DegreeGradeResult GradeExam(string degreeType, List<int> studentAnswers)
        {
            var allQuestions = GetExamQuestions();
            var questions = allQuestions.FindAll(q => q.DegreeType == degreeType);
            
            int score = 0;
            for (int i = 0; i < studentAnswers.Count && i < questions.Count; i++)
            {
                if (studentAnswers[i] == questions[i].CorrectIndex) score++;
            }
            
            double percentage = questions.Count > 0 ? ((double)score / questions.Count) * 100 : 0;
            return new DegreeGradeResult(degreeType, score, questions.Count, percentage >= 75.0, percentage); // 75% required to pass university exams
        }
    }
}
