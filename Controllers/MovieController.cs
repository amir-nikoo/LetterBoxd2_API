using LetterBoxd.Data;
using LetterBoxd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetterBoxd.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/movies")]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public MovieController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                Console.WriteLine("Fetching movies from the database...");
                var movies = await _context.Movies.ToListAsync();
                Console.WriteLine("Movies fetched successfully.");
                return Ok(movies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching data.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) 
        {
            var targetMovie = await _context.Movies.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == id);
            if(targetMovie == null)
            {
                return NotFound();
            }

            return Ok(targetMovie);
        }

        [HttpGet("comments/{id:int}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpPost("{movieid:int}/comments")]
        public async Task<IActionResult> CreateComment(CommentViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newComment = new Comment{
                Username = User.Identity.Name,
                Content = model.Content,
                MovieId = model.MovieId
            };

            await _context.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCommentById), new { id = newComment.Id }, newComment);
        }

        [HttpPut("{movieid:int}/comments/{id:int}")]
        public async Task<IActionResult> EditComment([FromRoute] int id, CommentViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var targetComment = await _context.Comments.FirstOrDefaultAsync(s => s.Id == id);
            if(targetComment == null)
            {
                return NotFound();
            }

            if(targetComment.Username != User.Identity.Name)
            {
                return Unauthorized();
            }

            targetComment.Content = model.Content;
            await _context.SaveChangesAsync();
            return RedirectToAction("GetById", new {id = model.MovieId});
        }

        [HttpDelete("{movieid}/comments/{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var targetComment = await _context.Comments.FirstOrDefaultAsync(s => s.Id == id);
            if(targetComment == null)
            {
                return NotFound();
            }

            if(targetComment.Username != User.Identity.Name)
            {
                return Unauthorized();
            }

            _context.Remove(targetComment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("ratings/{id:int}")]
        public async Task<IActionResult> GetRatingById(int id)
        {
            var targetRating = await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            
            if (targetRating == null)
            {
                return NotFound();
            }

            return Ok(targetRating);
        }

        [HttpPost("{movieid}/rating")]
        public async Task<IActionResult> AddRating(RatingViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newRating = new Rating{
                Username = User.Identity.Name,
                Score = model.Score,
                MovieId = model.MovieId
            };

            await _context.AddAsync(newRating);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRatingById), new { id = newRating.Id }, newRating);
        }

        [HttpPut("{movieid}/rating/{id:int}")]
        public async Task<IActionResult> EditRating([FromRoute] int id, RatingViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var targetRating = await _context.Ratings.FirstOrDefaultAsync(s => s.Id == id);
            if(targetRating == null)
            {
                return NotFound();
            }

            if(targetRating.Username != User.Identity.Name)
            {
                return Unauthorized();
            }

            targetRating.Score = model.Score;
            await _context.SaveChangesAsync();
            return RedirectToAction("GetById", new {id = model.MovieId});
        }
    }
}