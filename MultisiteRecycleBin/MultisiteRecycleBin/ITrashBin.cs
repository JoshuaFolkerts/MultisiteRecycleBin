using EPiServer.Core;

namespace MultisiteRecycleBin
{
    public interface ITrashBin
    {
        ContentReference ContentLink { get; set; }
    }
}