using kolokwium1.Models.DTOs;
using kolokwium1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalsRepository _animalsRepository;
        public AnimalsController(IAnimalsRepository animalsRepository)
        {
            _animalsRepository = animalsRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnimal(int id)
        {
            if (!await _animalsRepository.DoesAnimalExist(id))
                return NotFound($"Animal with ID {id} does not exist");

            try
            {
                var animal = await _animalsRepository.GetAnimal(id);
                var response = new
                {
                    id = animal.Id,
                    name = animal.Name,
                    animalClass = animal.AnimalClass,
                    admissionDate = animal.AdmissionDate.ToString("yyyy-MM-dd"),
                    owner = new
                    {
                        id = animal.Owner.Id,
                        firstName = animal.Owner.FirstName,
                        lastName = animal.Owner.LastName
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddAnimal(NewAnimalWithProcedures newAnimalWithProcedures)
        {
            if (!await _animalsRepository.DoesOwnerExist(newAnimalWithProcedures.OwnerId))
                return NotFound($"Owner with given ID - {newAnimalWithProcedures.OwnerId} doesn't exist");

            if (!await _animalsRepository.DoesAnimalExist(newAnimalWithProcedures.AnimalClassID))
                return NotFound($"Animal Class with given ID - {newAnimalWithProcedures.AnimalClassID} doesn't exist");

            foreach (var procedure in newAnimalWithProcedures.Procedures)
            {
                if (!await _animalsRepository.DoesProcedureExist(procedure.ProcedureId))
                    return NotFound($"Procedure with given ID - {procedure.ProcedureId} doesn't exist");
            }

            try
            {
                await _animalsRepository.AddNewAnimalWithProcedures(newAnimalWithProcedures);
                return Created(Request.Path.Value ?? "api/animals", newAnimalWithProcedures);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
