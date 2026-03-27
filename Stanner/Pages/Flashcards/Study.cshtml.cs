using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stanner.Data;
using Stanner.Models;

namespace Stanner.Pages.Flashcards;

public class StudyModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public StudyModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Subject? Subject { get; set; }
    public List<Flashcard> Flashcards { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? subjectId)
    {
        if (subjectId == null)
        {
            return RedirectToPage("/Portal");
        }

        Subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (Subject == null)
        {
            return NotFound();
        }

        Flashcards = await _context.Flashcards
            .Where(f => f.SubjectId == subjectId)
            .OrderBy(f => f.Id)
            .ToListAsync();

        if (!Flashcards.Any())
        {
            TempData["Message"] = "No flashcards are available for this subject.";
            return RedirectToPage("/Portal");
        }

        return Page();
    }

    // Complete entire study session and save XP
    public async Task<IActionResult> OnPostCompleteStudyAsync(int totalXpEarned)
    {
        string userId = "temp-user-1";  // TODO: Replace with actual user ID

        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (userProfile == null)
        {
            return new JsonResult(new { success = false, message = "User not found" });
        }

        // Add XP to user profile
        userProfile.TotalXP += totalXpEarned;

        // Calculate new level (every 100 XP = 1 level)
        userProfile.Level = (userProfile.TotalXP / 100) + 1;

        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true, xp = totalXpEarned, newLevel = userProfile.Level, newTotalXP = userProfile.TotalXP });
    }
}
