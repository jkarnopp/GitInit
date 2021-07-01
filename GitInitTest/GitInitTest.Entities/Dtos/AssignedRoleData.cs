using System;

namespace GitInitTest.Entities.Dtos
{
    public class AssignedRoleData
    {
        public Guid UserRoleId { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}