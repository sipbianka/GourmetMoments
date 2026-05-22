using ChefWebAPI.Dtos;
using ChefWebAPI.Models;
using ChefWebAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ChefWebAPI.Controllers
{

    [ApiController]
    [Route("api/berlesek")]
    public class BerlesekController : ControllerBase
    {
        private readonly BerlesRepository _repo;

        public BerlesekController(BerlesRepository repo)
        {
            _repo = repo;
        }

        // GET /api/berlesek
        [HttpGet]
        public IActionResult GetAll()
        {
            var all = _repo.GetAll()
                .Select(b => ToResponse(b))
                .ToList();
            return Ok(all);
        }

        // GET /api/berlesek/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var b = _repo.GetById(id);
            if (b == null) return NotFound();
            return Ok(ToResponse(b));
        }

        // POST /api/berlesek
        [HttpPost]
        public IActionResult Create([FromBody] BerlesCreateDto dto)
        {
            if (dto == null) return BadRequest("Request body is required.");

            var start = dto.StartDate.Date;
            var end = dto.EndDate.Date;

            var tomorrow = DateTime.Now.Date.AddDays(1);
            if (start < tomorrow)
                return BadRequest("A bérlés kezdőnapja nem lehet korábbi, mint holnap.");

            if (end < start)
                return BadRequest("A bérlés zárónapja nem lehet korábbi a kezdőnapnál.");

            var daysInclusive = (end - start).Days + 1;

            if (daysInclusive < 3)
                return BadRequest("A bérlés időtartama legalább 3 napnak kell lennie.");

            if (daysInclusive > 14)
                return BadRequest("A bérlés időtartama legfeljebb 14 nap lehet.");

            if (_repo.HasOverlapForChef(dto.ChefId, start, end))
                return BadRequest("A megadott időszakban a séf már foglalt.");

            var model = new Berles
            {
                Uid = dto.Uid,
                ChefId = dto.ChefId,
                StartDate = start,
                EndDate = end,
                DailyRate = dto.DailyRate,
                BaseFee = dto.BaseFee
            };

            var created = _repo.Add(model);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
        }

        // DELETE /api/berlesek/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _repo.Delete(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        private static BerlesResponseDto ToResponse(Berles b)
        {
            return new BerlesResponseDto
            {
                Id = b.Id,
                Uid = b.Uid,
                ChefId = b.ChefId,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                DailyRate = b.DailyRate,
                BaseFee = b.BaseFee,
                TotalPrice = b.TotalPrice
            };
        }
    }
}