using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stanner.Data;
using Stanner.Models;

namespace Stanner.Pages.Notes;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Note Note { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var note = await _context.Notes.FirstOrDefaultAsync(m => m.Id == id);

        if (note == null)
        {
            return NotFound();
        }
        
        Note = note;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var note = await _context.Notes.FindAsync(id);

        if (note != null)
        {
            Note = note;
            _context.Notes.Remove(Note);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Notes/Index", new { subjectId = Note.SubjectId });
    }
}