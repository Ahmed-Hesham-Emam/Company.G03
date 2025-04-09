namespace Company.G03.PL.Dtos
    {
    public class ReturnRoleDto
        {
        public string? Id { get; set; }
        public string Name { get; set; }

        public List<string>? AssignedPermissions { get; set; } = new();
        public List<string>? UnassignedPermissions { get; set; } = new();
        }
    }
