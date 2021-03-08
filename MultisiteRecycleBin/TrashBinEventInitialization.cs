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
            events.DeletedContent += DeletedContent;
        }

        private void DeletedContent(object sender, DeleteContentEventArgs e)
        {
            if (e.Content != null)
            {
                ITrashBin siteTrashBin = this.contentRepository.GetChildren<IContent>(SiteDefinition.Current.StartPage)
                    .OfType<ITrashBin>()
                    .FirstOrDefault();
                if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket))
                {
                }
            }
        }

        private void MovedContent(object sender, EPiServer.ContentEventArgs e)
        {
            if (e.Content != null)
            {
                ITrashBin siteTrashBin = this.contentRepository.GetChildren<IContent>(SiteDefinition.Current.StartPage)
                    .OfType<ITrashBin>()
                    .FirstOrDefault();

                var ancestors = this.contentRepository.GetAncestors(e.ContentLink);
                if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket))
                {
                    if (siteTrashBin != null && !ContentReference.IsNullOrEmpty(siteTrashBin.ContentLink))
                    {
                        this.contentRepository.Move(e.ContentLink, siteTrashBin.ContentLink);
                        e.CancelAction = true;
                        e.CancelReason = "Finsihed Moving To Site Trash Bin";
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            var events = context.Locate.Advanced.GetInstance<IContentEvents>();
            events.MovedContent -= MovedContent;
            events.DeletedContent -= DeletedContent;
        }
    }
}