
namespace ReadApi.Mapping
{
    public enum EntityType
    {
        Undefined = 0,  // Note: A value of 0 actually corresponds to 'System' level' but we are not supposed to store data on that level
        Client = 15,
        Company = 17,
        Employee = 21,
        Contract = 49,
        Assignment = 800,
        OrganizationalUnit = 6000
    }
}
