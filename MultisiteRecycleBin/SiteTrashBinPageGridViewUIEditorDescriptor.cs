using EPiServer.Labs.GridView;
using EPiServer.Labs.GridView.GridConfiguration;
using EPiServer.Shell;

namespace MultisiteRecycleBin
{
    [UIDescriptorRegistration]
    public class SiteTrashBinPageGridViewUIEditorDescriptor : ExtendedUIDescriptor<SiteTrashBinPage>
    {
        public SiteTrashBinPageGridViewUIEditorDescriptor()
            : base("epi-iconTrash")
        {
            DefaultView = SearchContentViewContentData.ViewKey;
            this.IconClass = "epi-iconTrash";
            this.CommandIconClass = "epi-iconTrash";

            GridSettings = new GridSettings
            {
                AllowShowDescendants = true,
                ContentStatusVisible = true,
                Columns = new ColumnsListBuilder()
                    .WithContentName()
                    .WithContentStatus()
                    .WithContentTypeName()
                    .WithCreatedBy()
                    .WithPublishDate()
                    .WithCurrentLanguageBranch()
                    .Build()
            };
        }
    }
}