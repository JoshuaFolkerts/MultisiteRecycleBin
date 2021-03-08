using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Web;
using System;
using System.Linq;

namespace MultisiteRecycleBin
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class TrashBinEventInitialization : IInitializableModule
    {
        private IContentRepository contentRepository;

        public void Initialize(InitializationEngine context)
        {
            var events = context.Locate.Advanced.GetInstance<IContentEvents>();
            this.contentRepository = context.Locate.ContentRepository();
            events.MovedContent += MovedContent;
        }

        private void MovedContent(object sender, ContentEventArgs e)
        {
            if (e.Content != null)
            {
                ITrashBin siteTrashBin = this.contentRepository.GetChildren<IContent>(SiteDefinition.Current.StartPage)
                    .OfType<ITrashBin>()
                    .FirstOrDefault();

                if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket))
                {
                    if (siteTrashBin != null && !ContentReference.IsNullOrEmpty(siteTrashBin?.ContentLink))
                    {
                        var originalParent = ((MoveContentEventArgs)e).OriginalParent;
                        if (!originalParent.CompareToIgnoreWorkID(siteTrashBin?.ContentLink))
                        {
                            this.contentRepository.Move(e.ContentLink, siteTrashBin.ContentLink);
                        }
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            var events = context.Locate.Advanced.GetInstance<IContentEvents>();
            events.MovedContent -= MovedContent;
        }
    }
}