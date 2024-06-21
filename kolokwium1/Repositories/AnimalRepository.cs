using kolokwium1.Models.DTOs;
using Microsoft.Data.SqlClient; 
using System.Transactions;

namespace kolokwium1.Repositories
{
    public class AnimalsRepository : IAnimalsRepository
    {
        private readonly IConfiguration _configuration;
        public AnimalsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> DoesAnimalExist(int id)
        {
            var query = "SELECT 1 FROM Animal WHERE ID = @ID";

            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandText = query;
            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();

            var res = await command.ExecuteScalarAsync();

            return res is not null;
        }

        public async Task<bool> DoesOwnerExist(int id)
        {
            var query = "SELECT 1 FROM Owner WHERE ID = @ID";

            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandText = query;
            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();

            var res = await command.ExecuteScalarAsync();

            return res is not null;
        }

        public async Task<bool> DoesProcedureExist(int id)
        {
            var query = "SELECT 1 FROM [Procedure] WHERE ID = @ID";

            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandText = query;
            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();

            var res = await command.ExecuteScalarAsync();

            return res is not null;
        }

public async Task<AnimalDto> GetAnimal(int id)
{
    var query = @"SELECT 
                    Animal.ID AS AnimalID,
                    Animal.Name AS AnimalName,
                    AdmissionDate,
                    Owner.ID as OwnerID,
                    FirstName,
                    LastName,
                    Animal_Class.Name as AnimalClassName
                  FROM Animal
                  JOIN Owner ON Owner.ID = Animal.OwnerID
                  JOIN Animal_Class ON Animal_Class.ID = Animal.AnimalClassID
                  WHERE Animal.ID = @ID";

    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
    await using SqlCommand command = new SqlCommand();

    command.Connection = connection;
    command.CommandText = query;
    command.Parameters.AddWithValue("@ID", id);

    await connection.OpenAsync();

    var reader = await command.ExecuteReaderAsync();

    var animalIdOrdinal = reader.GetOrdinal("AnimalID");
    var animalNameOrdinal = reader.GetOrdinal("AnimalName");
    var admissionDateOrdinal = reader.GetOrdinal("AdmissionDate");
    var ownerIdOrdinal = reader.GetOrdinal("OwnerID");
    var firstNameOrdinal = reader.GetOrdinal("FirstName");
    var lastNameOrdinal = reader.GetOrdinal("LastName");
    var animalClassNameOrdinal = reader.GetOrdinal("AnimalClassName");

    AnimalDto animalDto = null;

    while (await reader.ReadAsync())
    {
        animalDto = new AnimalDto()
        {
            Id = reader.GetInt32(animalIdOrdinal),
            Name = reader.GetString(animalNameOrdinal),
            AdmissionDate = reader.GetDateTime(admissionDateOrdinal),
            Owner = new OwnerDto()
            {
                Id = reader.GetInt32(ownerIdOrdinal),
                FirstName = reader.GetString(firstNameOrdinal),
                LastName = reader.GetString(lastNameOrdinal),
            },
            AnimalClass = reader.GetString(animalClassNameOrdinal)
        };
    }

    if (animalDto is null) throw new Exception("Animal not found");

    return animalDto;
}

public async Task AddNewAnimalWithProcedures(NewAnimalWithProcedures newAnimalWithProcedures)
{
    var insertAnimalQuery = @"INSERT INTO Animal (Name, AdmissionDate, OwnerID, AnimalClassID) 
                              VALUES(@Name, @AdmissionDate, @OwnerID, @AnimalClassID);
                              SELECT SCOPE_IDENTITY();";

    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
    await using SqlCommand command = new SqlCommand();

    command.Connection = connection;
    command.CommandText = insertAnimalQuery;

    command.Parameters.AddWithValue("@Name", newAnimalWithProcedures.Name);
    command.Parameters.AddWithValue("@AdmissionDate", newAnimalWithProcedures.AdmissionDate);
    command.Parameters.AddWithValue("@OwnerID", newAnimalWithProcedures.OwnerId);
    command.Parameters.AddWithValue("@AnimalClassID", newAnimalWithProcedures.AnimalClassID);

    await connection.OpenAsync();

    var transaction = await connection.BeginTransactionAsync();
    command.Transaction = transaction as SqlTransaction;

    try
    {
        var id = await command.ExecuteScalarAsync();

        foreach (var procedure in newAnimalWithProcedures.Procedures)
        {
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO Procedure_Animal (ProcedureID, AnimalID, Date) VALUES(@ProcedureID, @AnimalID, @Date)";
            command.Parameters.AddWithValue("@ProcedureID", procedure.ProcedureId);
            command.Parameters.AddWithValue("@AnimalID", id);
            command.Parameters.AddWithValue("@Date", procedure.Date);

            await command.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        throw new Exception($"Error inserting animal with procedures: {ex.Message}");
    }
}


        public async Task<int> AddAnimal(NewAnimalDTO animal)
        {
            var insertAnimalQuery = @"INSERT INTO Animal (Name, AdmissionDate, OwnerID, AnimalClassID) 
                                      VALUES(@Name, @AdmissionDate, @OwnerID, @AnimalClassID);
                                      SELECT SCOPE_IDENTITY();";

            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandText = insertAnimalQuery;

            command.Parameters.AddWithValue("@Name", animal.Name);
            command.Parameters.AddWithValue("@AdmissionDate", animal.AdmissionDate);
            command.Parameters.AddWithValue("@OwnerID", animal.OwnerId);
            command.Parameters.AddWithValue("@AnimalClassID", animal.AnimalClassID);

            await connection.OpenAsync();

            var id = await command.ExecuteScalarAsync();

            if (id is null) throw new Exception();

            return Convert.ToInt32(id);
        }

        public async Task AddProcedureAnimal(int animalId, ProcedureWithDate procedure)
        {
            var insertProcedureAnimalQuery = "INSERT INTO Procedure_Animal (ProcedureID, AnimalID, Date) VALUES(@ProcedureID, @AnimalID, @Date)";

            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandText = insertProcedureAnimalQuery;
            command.Parameters.AddWithValue("@ProcedureID", procedure.ProcedureId);
            command.Parameters.AddWithValue("@AnimalID", animalId);
            command.Parameters.AddWithValue("@Date", procedure.Date);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
