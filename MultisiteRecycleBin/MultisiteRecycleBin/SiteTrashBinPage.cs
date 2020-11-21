using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace MultisiteRecycleBin
{
    [ContentType(DisplayName = "Site Trash Bin Page", GUID = "e6f813c0-d533-42d6-8a0c-35ee4c8a63d2", Description = "", AvailableInEditMode = false)]
    public class SiteTrashBinPage : PageData, ITrashBin
    {
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            this.SortIndex = 99999;
        }
    }
}