using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Models;
using ST10263027_PROG6212_POE.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ST10263027_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TrackClaims()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TrackClaim(string claim_number)
        {
            if (string.IsNullOrEmpty(claim_number))
            {
                TempData["ErrorMessage"] = "Claim Number is required."; //displays if the user clicks on the Track Claim button without filling in the Claim Number
                return View("TrackClaims"); //redirects to the Track Claims page
            }

            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimNum == claim_number);

            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found for the provided claim number."; //displays a message if the claim does not exist or the claim number is wrong
                return View("TrackClaims"); //redirects to the Track Claims page
            }

            ViewBag.ClaimStatus = claim.ClaimStatus;
            return View("TrackClaims"); //redirects to the Track Claims page
        }

        public IActionResult VerifyClaims() //method to handle the verification of claims by passing the information of the claim to the table in the Verify Claims page
        {
            var claimViewModels = _context.Claims
                .Where(claim => claim.ClaimStatus == "Pending")
                .Select(claim => new ClaimViewModel
                {
                    ClaimID = claim.ClaimID, //fetches the ClaimID
                    ClaimNum = claim.ClaimNum, //fetches the Claim Number
                    LecturerNum = claim.Lecturer.LecturerNum, //fetches the Lecturer Number
                    SubmissionDate = claim.SubmissionDate, //fetches the submission date of the claim
                    HoursWorked = claim.Lecturer.HoursWorked, //fetches the lecturer's hoursworked
                    HourlyRate = claim.Lecturer.HourlyRate, // fetches the lecturer's hourly rate
                    TotalAmount = claim.Lecturer.HoursWorked * claim.Lecturer.HourlyRate, //calculates the total amount of the contract by multiplying the lecturer's hoursworked and hourly rate
                    Comments = claim.Comments, //fetches the comments submitted by the lecturer
                    Filename = claim.Filename //fetches the name of the file submitted by the lecturer
                })
                .ToList();

            return View(claimViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id) //method for handling the approval of a claim
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.ClaimStatus = "Approved"; 
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Claim has been approved successfully."; //displays if a claim is approved
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found."; //displays if a claim is not found
            }

            return RedirectToAction(nameof(VerifyClaims)); //redirects to the Verify Claims page so a Coordinator or Manager can approve or reject more complains
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id) //method for handling the rejection of a claim
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                claim.ClaimStatus = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Claim has been rejected successfully."; //displays if a claim is rejected successfully
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction(nameof(VerifyClaims));  //redirects to the Verify Claims page so a Coordinator or Manager can approve or reject more complains
        }

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
    }
}
//**************************************************end of file***********************************************//
