using EPiServer.Shell;

namespace MultisiteRecycleBin
{
    [UIDescriptorRegistration]
    public class SiteTrashBinEditorDescription : UIDescriptor<SiteTrashBinPage>
    {
        public SiteTrashBinEditorDescription()
            : base("epi-iconTrash")
        {
            DefaultView = CmsViewNames.ContentListingView;
        }
    }
}