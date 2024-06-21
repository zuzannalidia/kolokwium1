using System.Collections.Generic;
using kolokwium1.Models.DTOs;

namespace kolokwium1.Repositories
{
    public interface IAnimalsRepository
    {
        Task<bool> DoesAnimalExist(int id);
        Task<bool> DoesOwnerExist(int id);
        Task<bool> DoesProcedureExist(int id);
        Task<AnimalDto> GetAnimal(int id);
        Task AddNewAnimalWithProcedures(NewAnimalWithProcedures newAnimalWithProcedures);
        Task<int> AddAnimal(NewAnimalDTO animal);
        Task AddProcedureAnimal(int animalId, ProcedureWithDate procedure);
    }


}