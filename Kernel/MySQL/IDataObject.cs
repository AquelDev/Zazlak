
namespace Zazlak.Storage
{
    internal interface IDataObject
    {
        bool INSERT(DatabaseClient MySQL);
        bool DELETE(DatabaseClient MySQL);
        bool UPDATE(DatabaseClient MySQL);
    }
}
