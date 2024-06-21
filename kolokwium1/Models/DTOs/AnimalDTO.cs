namespace kolokwium1.Models.DTOs
{
    public class AnimalDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime AdmissionDate { get; set; }
        public OwnerDto Owner { get; set; } = null!;
        public string AnimalClass { get; set; } = string.Empty;
    }

    public class OwnerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}

public class ProcedureDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class AnimalClassDto 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
