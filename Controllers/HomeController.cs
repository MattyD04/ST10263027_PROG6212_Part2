using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ST10263027_PROG6212_POE.Models;
using ST10263027_PROG6212_POE.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ST10263027_PROG6212_POE.Roles;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
            private readonly ILogger<HomeController> _logger;
            private readonly AppDbContext _context;
            private readonly IValidator<ClaimViewModel> _validator;

            public HomeController(ILogger<HomeController> logger, AppDbContext context, IValidator<ClaimViewModel> validator)
            {
                _logger = logger;
                _context = context;
                _validator = validator;
            }


            public IActionResult Index()
        {
            return View();
        }

        [AuthorisingRoles("Lecturer")] //restricts access to the claim submission form for only lecturers
        public IActionResult Privacy()
        {
            return View();
        }

        [AuthorisingRoles("Lecturer")] //restricts access to the tracking of claims page for only lecturers
        public IActionResult TrackClaims()
        {
            return View();
        }
        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> TrackClaim(string claim_number)
        {
            if (string.IsNullOrEmpty(claim_number))
            {
                TempData["ErrorMessage"] = "Claim Number is required.";
                return View("TrackClaims");
            }

            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNum == claim_number);

            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found for the provided claim number.";
                return View("TrackClaims");
            }

            ViewBag.ClaimStatus = claim.ClaimStatus;
            return View("TrackClaims");
        }
        //***************************************************************************************//
        [AuthorisingRoles("Manager", "Coordinator")] //restricts access to the verifying of claims page to only be accessible by managers and coordinators
        public IActionResult VerifyClaims()
        {
            var claimViewModels = _context.Claims
                .Where(claim => claim.ClaimStatus == "Pending")
                .Select(claim => new ClaimViewModel
                {
                    ClaimID = claim.ClaimID,
                    ClaimNum = claim.ClaimNum,
                    LecturerNum = claim.Lecturer.LecturerNum,
                    SubmissionDate = claim.SubmissionDate,
                    HoursWorked = claim.Lecturer.HoursWorked,
                    HourlyRate = claim.Lecturer.HourlyRate,
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate,
                    Comments = claim.Comments,
                    Filename = claim.Filename
                })
                .ToList();

            return View(claimViewModels);
        }
        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                var claimViewModel = new ClaimViewModel
                {
                    ClaimID = claim.ClaimID,
                    ClaimNum = claim.ClaimNum,
                    LecturerNum = claim.Lecturer.LecturerNum,
                    SubmissionDate = claim.SubmissionDate,
                    HoursWorked = claim.Lecturer.HoursWorked,
                    HourlyRate = claim.Lecturer.HourlyRate,
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate,
                    Comments = claim.Comments,
                    Filename = claim.Filename
                };

                // Validate claim
                var validationResult = await _validator.ValidateAsync(claimViewModel);

                if (validationResult.IsValid)
                {
                    // Automatically approve the claim if it meets the criteria
                    claim.ClaimStatus = "Approved";
                    TempData["SuccessMessage"] = "Claim has been automatically approved due to policy.";
                }
                else
                {
                    // Handle cases where claim validation fails
                    TempData["ErrorMessage"] = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return RedirectToAction(nameof(VerifyClaims));
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }
        //***************************************************************************************//
        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.ClaimStatus = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Claim has been rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }
        //***************************************************************************************//
        public IActionResult LecturerLogin()
        {
            return View();
        }

        public IActionResult AcademicManagerLogin()
        {
            return View();
        }

        public IActionResult ProgrammeCoordinatorLogin()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Logout()
        {
            // Clear the session to log the user out
            HttpContext.Session.Clear();

            // Redirect to the home page or login page
            return RedirectToAction("Index", "Home");
        }
    }
}
//*************************************End of file**************************************************//
