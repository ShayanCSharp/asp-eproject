using EProjectFinal.Models;
using EProjectFinal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace EProjectFinal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _Context;

        public AccountController(ApplicationDbContext Db)
        {
            _Context = Db;
        }

        // Candidate Login - GET
        public IActionResult CandidateLogin()
        {
            return View();
        }

        // Candidate Login - POST
        [HttpPost]
        public async Task<IActionResult> CandidateLogin(CandidateLogin user)
        {
            if (ModelState.IsValid)
            {
                var result = await _Context.Candidates
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == user.Email.ToLower() && x.Password == user.Password);

                if (result != null)
                {
                    // Set session for Candidate
                    HttpContext.Session.SetString("Candidate", "True");
                    HttpContext.Session.SetString("C_Id", result.CandidateId.ToString());
                    return RedirectToAction("Dashboard", "Candidate");
                }
                else
                {
                    ViewBag.Message = "Email or Password is incorrect!";
                    return View(user);
                }
            }
            return View(user);
        }

        // Manager Login - GET
        public IActionResult ManagerLogin()
        {
            return View();
        }

        // Manager Login - POST
        [HttpPost]
        public async Task<IActionResult> ManagerLogin(ManagerLogin user)
        {
            if (ModelState.IsValid)
            {
                var result = await _Context.Managers
                    .FirstOrDefaultAsync(x => x.Username == user.Username && x.Password == user.Password);

                if (result != null)
                {
                    // Set session for Manager
                    HttpContext.Session.SetString("Manager", "True");
                    HttpContext.Session.SetString("M_Id", result.ManagerId.ToString());
                    return RedirectToAction("Dashboard", "Manager");
                }
                else
                {
                    ViewBag.Message = "Username or Password is incorrect!";
                    return View(user);
                }
            }
            return View(user);
        }

        // Logout Action
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            return RedirectToAction("ManagerLogin"); // Redirect to CandidateLogin or ManagerLogin based on your needs
        }
    }
}
