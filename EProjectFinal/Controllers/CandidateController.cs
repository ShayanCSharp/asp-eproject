using EProjectFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace EProjectFinal.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidateController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            int candidateId = int.Parse(HttpContext.Session.GetString("C_Id"));

            // Check if the candidate has already completed the test or sections
            var result = _context.Results.FirstOrDefault(r => r.CandidateId == candidateId);
            if (result != null && result.IsPassed != null)
            {
                return RedirectToAction("ViewResults");
            }

            // Check if the candidate has started a section and resume or start from the first section
            string currentSection = HttpContext.Session.GetString("CurrentSection") ?? "General Knowledge";
            HttpContext.Session.SetString("CurrentSection", currentSection);

            // Set ViewBag variables
            ViewBag.TestStatus = result != null; // Boolean: true if test has been started/completed, false otherwise
            ViewBag.CurrentSection = currentSection; // The name of the current section for display

            return View();
        }



        // Action to start the test (initializes the test process)
        [HttpGet]
        public async Task<IActionResult> StartTest()
        {
            var candidateIdString = HttpContext.Session.GetString("C_Id");
            if (string.IsNullOrEmpty(candidateIdString))
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            int candidateId = int.Parse(candidateIdString);

            // Set the candidate's test status to "in progress"
            var candidate = await _context.Candidates.FindAsync(candidateId);
            if (candidate == null)
            {
                return RedirectToAction("Dashboard");
            }

            candidate.IsTestCompleted = false;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("CurrentSection", "General Knowledge");
            HttpContext.Session.SetInt32("CurrentQuestionIndex", 0);

            return RedirectToAction("LoadQuestion");
        }

        // Action to load a question in the current section
        [HttpGet]
        public async Task<IActionResult> LoadQuestion()
        {
            string currentSection = HttpContext.Session.GetString("CurrentSection") ?? "General Knowledge";
            int questionIndex = HttpContext.Session.GetInt32("CurrentQuestionIndex") ?? 0;

            ViewBag.CurrentSection = currentSection;

            var questions = await _context.Questions
                .Where(q => q.Test.TestName == currentSection)
                .OrderBy(q => q.QuestionId)
                .ToListAsync();

            if (questions == null || !questions.Any())
            {
                ViewBag.ErrorMessage = "No questions available for the current section.";
                return View("Error"); // Display error view
            }

            if (questionIndex >= questions.Count)
            {
                return RedirectToAction("CompleteSection");
            }

            // Check if this is the last question
            ViewBag.IsLastQuestion = questionIndex == questions.Count - 1;

            var question = questions[questionIndex];
            return View(question);
        }



        // Action to submit the answer for the current question
        [HttpPost]
        public async Task<IActionResult> SubmitAnswer(int questionId, string selectedAnswer)
        {
            var candidateIdString = HttpContext.Session.GetString("C_Id");
            if (string.IsNullOrEmpty(candidateIdString))
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            int candidateId = int.Parse(candidateIdString);

            // Retrieve the question to check if the answer is correct and to get the TestId
            var question = await _context.Questions.Include(q => q.Test).FirstOrDefaultAsync(q => q.QuestionId == questionId);
            if (question == null)
            {
                ViewBag.ErrorMessage = "Question not found.";
                return View("Error");
            }

            // Check that the TestId exists
            if (question.Test == null)
            {
                ViewBag.ErrorMessage = "Test associated with this question does not exist.";
                return View("Error");
            }

            bool isCorrect = question.CorrectAnswer == selectedAnswer;

            // Save the candidate's answer
            var candidateAnswer = new CandidateAnswer
            {
                CandidateId = candidateId,
                QuestionId = questionId,
                SelectedAnswer = selectedAnswer,
                IsCorrect = isCorrect,
                MarksAwarded = isCorrect ? question.Marks : 0,
                TestId = question.Test.TestId // Set the TestId from the question's test
            };

            _context.CandidateAnswers.Add(candidateAnswer);
            await _context.SaveChangesAsync();

            // Move to the next question
            int questionIndex = HttpContext.Session.GetInt32("CurrentQuestionIndex") ?? 0;
            HttpContext.Session.SetInt32("CurrentQuestionIndex", questionIndex + 1);

            return RedirectToAction("LoadQuestion");
        }


        // Action to complete the current section and move to the next section
        [HttpPost]
        public IActionResult CompleteSection()
        {
            // Logic to complete the section
            // For example, updating the session to proceed to the next section
            string currentSection = HttpContext.Session.GetString("CurrentSection");

            // Determine the next section
            string nextSection = currentSection switch
            {
                "General Knowledge" => "Mathematics",
                "Mathematics" => "Computer Technology",
                "Computer Technology" => null, // end of sections
                _ => null
            };

            if (nextSection == null)
            {
                // All sections complete, redirect to complete test
                return View("CompleteTestRedirect");
            }

            // Update session for next section
            HttpContext.Session.SetString("CurrentSection", nextSection);
            HttpContext.Session.SetInt32("CurrentQuestionIndex", 0);

            return RedirectToAction("LoadQuestion");
        }




        // Action to complete the test, calculate the results, and save them
        [HttpPost]
        public async Task<IActionResult> CompleteTest()
        {
            var candidateIdString = HttpContext.Session.GetString("C_Id");
            if (string.IsNullOrEmpty(candidateIdString))
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            int candidateId = int.Parse(candidateIdString);

            // Get all tests (sections) for this candidate
            var tests = await _context.Tests.ToListAsync();
            var answers = await _context.CandidateAnswers
                .Where(a => a.CandidateId == candidateId)
                .ToListAsync();

            // Loop through each test to calculate individual test results
            foreach (var test in tests)
            {
                // Calculate the total marks obtained in the current test
                int totalMarksObtained = answers
                    .Where(a => a.TestId == test.TestId)
                    .Sum(a => a.MarksAwarded);

                // Determine if the candidate passed this test
                bool isPassed = totalMarksObtained >= (test.MaxMarks * 0.5);

                // Save result for each test section
                var result = new Result
                {
                    CandidateId = candidateId,
                    TestId = test.TestId,
                    TotalMarks = totalMarksObtained,
                    IsPassed = isPassed,
                    TestDate = DateTime.Now
                };

                _context.Results.Add(result);
            }

            // Mark candidate test as completed
            var candidate = await _context.Candidates.FindAsync(candidateId);
            candidate.IsTestCompleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewResults");
        }



        // Action to view the candidate's test results
        [HttpGet]
        public async Task<IActionResult> ViewResults()
        {
            var candidateIdString = HttpContext.Session.GetString("C_Id");
            if (string.IsNullOrEmpty(candidateIdString))
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            int candidateId = int.Parse(candidateIdString);

            var results = await _context.Results
                .Include(r => r.Test) // Ensure the Test details are loaded
                .Where(r => r.CandidateId == candidateId)
                .ToListAsync();

            if (!results.Any())
            {
                ViewBag.Message = "Results are not available yet.";
                return View("NoResults");
            }

            return View(results); // Pass list of results to the view
        }

    }
}
