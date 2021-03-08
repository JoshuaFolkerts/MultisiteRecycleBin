using EPiServer.Labs.GridView;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace MultisiteRecycleBin
{
    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class SiteTrashBinGridView : SearchContentView<SiteTrashBinPage>
    {
        public SiteTrashBinGridView()
            : base()
        {
            this.IconClass = "epi-iconTrash";
        }
    }
}