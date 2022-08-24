using MultiTenancy.Entities.Interfaces;

namespace MultiTenancy.Entities {
    public class Country : ICommonEntitie {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
